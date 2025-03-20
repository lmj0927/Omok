using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Constants;

public class BoardController : MonoBehaviour
{
    public Cell[,] cells;
    public int width;
    public int height;
    public Vector2 padding;
    public GameObject cellPrefab;
    public RectTransform cellParent;
    public RectTransform lastCellFlag;
    [SerializeField] GameObject gameSystemPrefab;
    GameObject _gameSystem;
    GridPlacementSystem _gridPlacementSystem;

    [SerializeField] List<Cell> fiveCells = new();
    [SerializeField] List<Cell> placeCellList = new();
    [SerializeField] Queue<int> placeCellIndexes = new();

    readonly List<List<(int, int)>> _directionsList = new List<List<(int, int)>>
    {
        new List<(int, int)>{ (0, 1), (0, -1) },
        new List<(int, int)>{ (1, 0), (-1, 0) },
        new List<(int, int)>{ (1, 1), (-1, -1) },
        new List<(int, int)>{ (1, -1), (-1, 1) }
    };

    private List<(int, int)> _forbiddenPoints;
    
    // void Start()
    // {
    //     Initialize();
    // }
    void OnEnable()
    {
        Initialize();

        if(gameSystemPrefab != null)
        {
            if(_gameSystem == null)
            {
                _gameSystem = Instantiate(gameSystemPrefab, new Vector3(0, 2f, 0), Quaternion.identity);
            }
            _gridPlacementSystem = _gameSystem.GetComponent<GridPlacementSystem>();
            GameManager.Instance.cameraMover.SetTarget(_gridPlacementSystem.cameraTarget);
            _gridPlacementSystem.OnSetCurrentCell += SetCurrentCell;
            GameManager.Instance.matchController.OnEndGridOmok += _gridPlacementSystem.EndOmok;
        }
    }

    void OnDisable()
    {
        if(_gameSystem != null)
        {
            if(GameManager.Instance.cameraMover != null)
                GameManager.Instance.cameraMover.SetTarget(null);
            _gridPlacementSystem.ClearStones();
            placeCellList.Clear();
            placeCellIndexes.Clear();
            Destroy(_gameSystem);
        }
    }

    public void Initialize()
    {
        cellParent.GetComponent<GridLayoutGroup>().cellSize = new Vector2((cellParent.rect.width - padding.x) / width, (cellParent.rect.height - padding.y) / height);
        cellParent.GetComponent<GridLayoutGroup>().padding = new RectOffset((int)padding.x / 2, (int)padding.x / 2, (int)padding.y / 2, (int)padding.y / 2);

        if(cells != null)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Destroy(cells[i, j].gameObject);
                }
            }            
        }
        
        cells = new Cell[width, height];
        
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject cell = Instantiate(cellPrefab, cellParent);
                cells[i, j] = cell.GetComponent<Cell>();
                cells[i, j].Initialize(i, j);
            }
        }
        GameManager.Instance.matchController.OnDrawCell = OnDrawCell;
        GameManager.Instance.matchController.TurnEnded = EndTurn;
        
        lastCellFlag.gameObject.SetActive(false);
    }

    private void OnDrawCell(TurnData turnData, CELL_TYPE type)
    {
        var row = turnData.row;
        var col = turnData.col;
        cells[row, col].SetCellType(type);
        
        if(_gridPlacementSystem != null)
        {
            _gridPlacementSystem.PlaceStone(row, col, type);
        }
    }
    
    void SetCurrentCell(TurnData data)
    {
        Cell cell = cells[data.row, data.col];

        GameManager.Instance.matchController.SetCurrentCell(cell);
    }
    

    private void EndTurn(TurnData turnData, MATCH_STATE state)
    {
        OnDrawCell(turnData, state == MATCH_STATE.BlackTurn ? CELL_TYPE.Black : CELL_TYPE.White);
        var row = turnData.row;
        var col = turnData.col;
        
        //마지막에 둔 Cell위에 표시 - 2D
        MoveToLastCellFlag(row, col, state);
        
        placeCellList.Add(cells[row, col]);

        AudioManager.Instance.PlaySFX("putStone");
        
        if (CheckGameResult(row, col))
        {
            CheckEndOmok();
            GameManager.Instance.matchController.EndMatch(MATCH_STATE.BlackTurn == state ? END_TYPE.BlackWin : END_TYPE.WhiteWin, false);
            return;
        }

        if (state == MATCH_STATE.BlackTurn)
        {
            if (_forbiddenPoints != null)
            {
                foreach (var forbid in _forbiddenPoints)
                {
                    TurnData t = new TurnData()
                    {
                        row = forbid.Item1,
                        col = forbid.Item2
                    };
                    OnDrawCell(t, CELL_TYPE.None);
                }   
            }
        }
        else if (state == MATCH_STATE.WhiteTurn)
        {
            _forbiddenPoints = GetForbiddenPoints();
            foreach (var forbid in _forbiddenPoints)
            {
                TurnData t = new TurnData()
                {
                    row = forbid.Item1,
                    col = forbid.Item2
                };
                OnDrawCell(t, CELL_TYPE.Warning);
            }
        }

        if (CheckDraw())
        {
            // TODO: 무승부 ui처리
            GameManager.Instance.matchController.DrawMatch();
        }
    }

    private void MoveToLastCellFlag(int row, int col, MATCH_STATE state)
    {
        if(!lastCellFlag.gameObject.activeSelf) lastCellFlag.gameObject.SetActive(true);
        
        var flagColor =  lastCellFlag.GetComponentInChildren<Image>();
        flagColor.DOColor(state == MATCH_STATE.BlackTurn ? Color.white : Color.black, 0);

        var currentCellPosition = cells[row,col].GetComponent<RectTransform>().anchoredPosition;
        lastCellFlag.anchoredPosition = currentCellPosition;
    }

    #region GameResult
    
    private bool CheckGameResult(int row, int col)
    {
        var cellType = cells[row, col].GetCellType();
        
        int count = 0; //count가 4이상이면 오목완성(SetTurn후 불리기에 현재 위치는 자기자신)
       
        foreach (var dirs in _directionsList)
        {
            fiveCells.Add(cells[row, col]);
            foreach (var dir in dirs)
            {
                for (int i = 1; i < 5; i++)
                {
                    if (CheckMark(row + dir.Item1 * i, col + dir.Item2 * i, cellType))
                    {
                        fiveCells.Add(cells[row + dir.Item1 * i, col + dir.Item2 * i]);
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
                
                if(count >= 4)
                    return true;
            }
            fiveCells.Clear();
            count = 0;
        }
        
        return false;
    }

    private bool CheckMark(int row, int col, CELL_TYPE cellType)
    {
        if (IsValidPosition(row, col))
        {
            if (cells[row, col].GetCellType() == cellType)
            {
                return true;
            };
        }
        
        return false;
    }
    
    private void CheckEndOmok()
    {
        if (fiveCells != null)
        {
            //2D 게임판
            GameManager.Instance.matchController.FiveCells.AddRange(fiveCells);
                
            //3D 게임판
            for (int i = 0; i < placeCellList.Count; i++)
            {
                foreach (var cell in fiveCells)
                {
                    if (placeCellList[i] == cell)
                    {
                        //오목이 된 오브젝트들의 위치정보 전달.
                        GameManager.Instance.matchController.EndStones.Add(_gridPlacementSystem.placedStoneList[i]);
                    }
                }
            }
        }
    }

    private bool CheckDraw()
    {
        for (int row = 0; row < width; row++)
        {
            for (int col = 0; col < height; col++)
            {
                if (cells[row, col].GetCellType() == CELL_TYPE.None)
                    return false;
            }
        }
        return true;
    }
    
    #endregion

    #region RenjuRule

    // 금수 찾기
    private List<(int, int)> GetForbiddenPoints()
    {
        List<(int, int)> forbidden = new List<(int, int)>();
        
        //금수를 찾아 리스트에 저장, 이때 거짓금수를 고려하여서 뽑아냄
        for (int row = 0; row < width; row++)
        {
            for (int col = 0; col < height; col++)
            {
                if (cells[row, col].GetCellType() != CELL_TYPE.None)
                     continue;
                if(CheckForbidden(row, col))
                    forbidden.Add((row, col));
            }
        }
        
        // 확실한 금수만 처리
        foreach (var point in forbidden)
        {
            cells[point.Item1, point.Item2].SetCellTypeTemporary(CELL_TYPE.Warning);
        }
        
        // 위의 금수로 인해 거짓 금수가 아닌 금수 판별
        for (int row = 0; row < width; row++)
        {
            for (int col = 0; col < height; col++)
            {
                if (cells[row, col].GetCellType() != CELL_TYPE.None)
                    continue;
                if (CheckDoubleThree(row, col) >= 2 || CheckDoubleFour(row, col) >= 2)
                    forbidden.Add((row, col));
            }
        }
        return forbidden;
    }

    // 해당 위치의 설정한 색의 돌이 놓아져있다고 가정하고 이어져있는 돌의 개수 반환
    private int GetCellCount(int row, int col, List<(int, int)> direction, CELL_TYPE cellType)
    {
        int count = 1;
        
        foreach (var dir in direction)
        {
            int newRow = row + dir.Item1;
            int newCol = col + dir.Item2;
            while (true)
            {
                if (!IsValidPosition(newRow, newCol) || cells[newRow, newCol].GetCellType() != cellType)
                {
                    break;
                }
                if (cells[newRow, newCol].GetCellType() == cellType)
                {
                    count++;
                }
                newRow += dir.Item1;
                newCol += dir.Item2;
            }
        }
        return count;
    }
    
    // 방향에 대해 빈 공간 찾는 함수
    private (int, int) FindEmpty(int row, int col, (int , int) direction)
    {
        var cellType = cells[row, col].GetCellType();
        
        int newRow = row + direction.Item1;
        int newCol = col + direction.Item2;
        while (true)
        {
            if (!IsValidPosition(newRow, newCol) || cells[newRow, newCol].GetCellType() != cellType)
            {
                break;
            }
            newRow += direction.Item1;
            newCol += direction.Item2;
        }
        if (IsValidPosition(newRow, newCol) && cells[newRow, newCol].GetCellType() == CELL_TYPE.None)
        {
            return (newRow, newCol);
        }
        return (-1, -1);
    }
    
    // 열린 3을 찾기
    private bool CheckOpenThree(int row, int col, List<(int, int)> direction)
    {
        foreach (var dir in direction)
        {
            var noneLocate = FindEmpty(row, col, dir);
            if (noneLocate.Item1 != -1)
            {
                int newRow = noneLocate.Item1;
                int newCol = noneLocate.Item2;
                
                // 빈 공간이 금수로 처리 되면 막혀있음, 거짓 금수 판별
                if (CheckDoubleFour(newRow, newCol)>=2)
                {
                    return false;
                }
                
                // 일시적으로 검은 돌을 두고
                cells[newRow, newCol].SetCellTypeTemporary(CELL_TYPE.Black);
                
                // 열린 4가 되는 지 판별
                if (1 == CheckOpenFour(newRow, newCol, direction))
                {
                    cells[newRow, newCol].SetCellTypeTemporary(CELL_TYPE.None);
                    return true;
                }
                cells[newRow, newCol].SetCellTypeTemporary(CELL_TYPE.None);
            }
        }
        
        return false;
    }

    // 열린 4 판별 함수
    private int CheckOpenFour(int row, int col, List<(int, int)> direction)
    {
        int count = 0;
        foreach (var dir in direction)
        {
            var noneLocate = FindEmpty(row, col, dir);
            if (noneLocate.Item1 != -1)
            {
                int newRow = noneLocate.Item1;
                int newCol = noneLocate.Item2;
                if (CheckFive(newRow, newCol, direction))
                {
                    count++;
                }
            }
        }

        // 한 줄의 44 금수가 발생할 수 있음, 이런 경우 4가 이어져있지 않고 떨어져있음
        // 따라서 4개가 이어져있는지 판별해서 이어져있으면 열린 4는 1개, 아니면 2개(44금수)
        if (count == 2)
        {
            if (GetCellCount(row, col, direction, CELL_TYPE.Black) == 4)
                count = 1;
        }
        else
        {
            count = 0;
        }
        
        return count;
    }
    
    //44는 한쪽이 닫힌 4에 대해서도 44가 발생하므로 4를 판별
    private bool CheckFour(int row, int col, List<(int, int)> direction)
    {
        foreach (var dir in direction)
        {
            var noneLocate = FindEmpty(row, col, dir);
            if (noneLocate.Item1 != -1)
            {
                int newRow = noneLocate.Item1;
                int newCol = noneLocate.Item2;
                if (CheckFive(newRow, newCol, direction))
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    // 한 방향에 대해 5목이 되는지 확인
    private bool CheckFive(int row, int col, List<(int, int)> direction)
    {
        var count = GetCellCount(row, col, direction, CELL_TYPE.Black);
        if (count == 5)
            return true;
        return false;    
    }

    // 모든 방향에 대해 5목이 되는지 확인
    private bool IsFive(int row, int col)
    {
        foreach (var dirs in _directionsList)
        {
            if(CheckFive(row, col, dirs))
                return true;
        }
        return false;
    }
    
    // 33 금수 확인
    private int CheckDoubleThree(int row, int col)
    {
        int count = 0;
        cells[row, col].SetCellTypeTemporary(CELL_TYPE.Black);
        foreach (var dir in _directionsList)
        {
            if (CheckOpenThree(row, col, dir))
            {
                count++;
            }
        }
        cells[row, col].SetCellTypeTemporary(CELL_TYPE.None);
        
        return count;
    }

    // 44 금수 확인
    private int CheckDoubleFour(int row, int col)
    {
        if(IsFive(row, col))
            return 0;
        int count = 0;
        cells[row, col].SetCellTypeTemporary(CELL_TYPE.Black);
        foreach (var dirs in _directionsList)
        {
            if(CheckOpenFour(row, col, dirs) >= 1)
                count += CheckOpenFour(row, col, dirs);
            else if (CheckFour(row, col, dirs))
            {
                count++;
            }
        }
        cells[row, col].SetCellTypeTemporary(CELL_TYPE.None);
        
        return count;
    }

    // 장목 확인
    private bool CheckLong(int row, int col)
    {
        foreach (var dirs in _directionsList)
        {
            var count = GetCellCount(row, col, dirs, CELL_TYPE.Black);
            if(count > 5)
                return true;
        }
        return false;
    }
    
    // 금수 확인
    private bool CheckForbidden(int row, int col)
    {
        // 만약 돌을 둠으로써 5목이 된다면 그 수는 금수가 아님
        if (IsFive(row, col))
        {
            return false;
        }
        
        // 장목 확인
        if(CheckLong(row, col))
            return true;

        // 우선적으로 확실한 금수(거짓 금수가 아닌 금수) 확인
        if (CheckDoubleThree(row, col) + CheckDoubleFour(row, col) >= 3)
        {
            return true;
        }
        
        return false;
    }
    
    private bool IsValidPosition(int row, int col)
    {
        return row >= 0 && row < width && col >= 0 && col < height;
    }

    #endregion
    
    public void SetCellType(int x, int y, CELL_TYPE cellType)
    {
        cells[x, y].SetCellType(cellType);
    }

    public void SetCellType(Vector2Int pos, CELL_TYPE cellType)
    {
        SetCellType(pos.x, pos.y, cellType);
    }

    public CELL_TYPE GetCellType(int x, int y)
    {
        return cells[x, y].GetCellType();
    }

    public CELL_TYPE GetCellType(Vector2Int pos)
    {
        return GetCellType(pos.x, pos.y);
    }

    public void Clear()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                cells[i, j].SetCellType(CELL_TYPE.None);
            }
        }
    }
}
