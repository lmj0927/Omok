using System.Collections;
using System.Collections.Generic;
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
    
    
    List<List<(int, int)>> directions = new List<List<(int, int)>>
    {
        new List<(int, int)>{ (0, 1), (0, -1) },
        new List<(int, int)>{ (1, 0), (-1, 0) },
        new List<(int, int)>{ (1, 1), (-1, -1) },
        new List<(int, int)>{ (1, -1), (-1, 1) }
    };

    private List<(int, int)> forbidden;
    
    // void Start()
    // {
    //     Initialize();
    // }
    void OnEnable()
    {
        Initialize();
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
        GameManager.Instance.matchController.onNextTest = TestRenjuOperate;
    }

    private void OnDrawCell(TurnData turnData, CELL_TYPE type)
    {
        var row = turnData.row;
        var col = turnData.col;
        cells[row, col].SetCellType(type);
    }

    private void EndTurn(TurnData turnData, MATCH_STATE state)
    {
        OnDrawCell(turnData, state == MATCH_STATE.BlackTurn ? CELL_TYPE.Black : CELL_TYPE.White);
        var row = turnData.row;
        var col = turnData.col;
        if (CheckGameResult(row, col))
        {
            GameManager.Instance.matchController.EndMatch(MATCH_STATE.BlackTurn == state, false);
            return;
        }

        if (state == MATCH_STATE.BlackTurn)
        {
            if (forbidden != null)
            {
                foreach (var forbid in forbidden)
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
            forbidden = GetForbiddenPoints();
            foreach (var forbid in forbidden)
            {
                TurnData t = new TurnData()
                {
                    row = forbid.Item1,
                    col = forbid.Item2
                };
                OnDrawCell(t, CELL_TYPE.Warning);
            }
        }
    }

    #region GameResult
    
    private bool CheckGameResult(int row, int col)
    {
        var cellType = cells[row, col].GetCellType();
        
        int count = 0; //count가 4이상이면 오목완성(SetTurn후 불리기에 현재 위치는 자기자신)
       
        foreach (var dirs in directions)
        {
            foreach (var dir in dirs)
            {
                for (int i = 1; i < 5; i++)
                {
                    if (CheckMark(row + dir.Item1 * i, col + dir.Item2 * i, cellType))
                    {
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
    
    #endregion

    #region RenjunRule
    //
    // private List<(int, int)> GetForbiddenPoints()
    // {
    //     List<(int, int)> forbidden = new List<(int, int)>();
    //
    //     return forbidden;
    // }
    //
    // private int GetStoneCount()
    // {
    //     
    //     return 0;
    // }
    //
    // private bool FindEmpty(int row, int col, CELL_TYPE cellType, (int , int) direction)
    // {
    //     int newRow = row + direction.Item1;
    //     int newCol = col + direction.Item2;
    //     while (true)
    //     {
    //         if (!IsValidPosition(newRow, newCol) || cells[newRow, newCol].GetCellType() == cellType)
    //         {
    //             break;
    //         }
    //     }
    //     if (cells[newRow, newCol].GetCellType() == CELL_TYPE.None)
    //     {
    //         return true;
    //     }
    //     return false;
    // }
    //
    // private bool CheckOpenThree()
    // {
    //     return false;
    // }
    //
    // private bool CheckOpenFour()
    // {
    //     return false;
    // }
    //
    // private bool CheckFive()
    // {
    //     return false;    
    // }
    //
    // private bool CheckDoubleThree()
    // {
    //     return false;
    // }
    //
    // private bool CheckDoubleFour()
    // {
    //     return false;
    // }
    
    
    private List<(int, int)> GetForbiddenPoints()
    {
        List<(int, int)> forbiddenPoints = new List<(int, int)>();
        
        
        for (int row = 0; row < width; row++)
        {
            for (int col = 0; col < height; col++)
            {
                if (cells[row, col].GetCellType() != CELL_TYPE.None)
                    continue;
    
                if (CheckDoubleThrees(row, col))
                {
                    forbiddenPoints.Add((row, col));
                }
    
                if (CheckDoubleFours(row, col))
                {
                    forbiddenPoints.Add((row, col));
                }
    
                if (CheckLong(row, col))
                {
                    forbiddenPoints.Add((row, col));
                }
            }
        }
        return forbiddenPoints;
    }
    
    private bool CheckDoubleThrees(int row, int col)
    {
        int threeCount = 0;
        foreach (var dirs in directions)
        {
            int noneCount = 0;
            int lineCount = 1;
            bool isFirstNone = false;
            bool isOpen = true;
            foreach (var dir in dirs)
            {
                bool isBlacked = true;
                for (int i = 1; i < 5; i++)
                {
                    int newRow = row + dir.Item1 * i;
                    int newCol = col + dir.Item2 * i;
    
                    if (!IsValidPosition(newRow, newCol) || cells[newRow, newCol].GetCellType() == CELL_TYPE.White || cells[newRow, newCol].GetCellType() == CELL_TYPE.Warning)
                    {
                        if(isBlacked)
                            isOpen = false;
                        break;
                    }
    
                    if (cells[newRow, newCol].GetCellType() == CELL_TYPE.None )
                    {
                        isBlacked = false;
                        
                        if(i == 1 && isFirstNone == false)
                            isFirstNone = true;
                        if(i == 1 && isFirstNone)
                            break;
                        
                        noneCount++;
                        if (noneCount == 2)
                        {
                            noneCount = 0;
                            break;
                        }
                    }
                    
                    if (cells[newRow, newCol].GetCellType() == CELL_TYPE.Black)
                    {
                        lineCount++;
                        isBlacked = true;
                    }
                }
    
                if (lineCount == 3 && isOpen) // 이때 거짓 금수 판별
                {
                    threeCount++;
                    lineCount = 1;
                } 
            }
        }
        if (threeCount >= 2 )
        {
            return true;
        }
        return false;
    }
    
    private bool CheckDoubleFours(int row, int col)
    {
        int fourCount = 0;
        foreach (var dirs in directions)
        {
            int lineCount = 1;
            int isOpen = 0;
            foreach (var dir in dirs)
            {
                bool isBlacked = true;
                for (int i = 1; i < 5; i++)
                {
                    int newRow = row + dir.Item1 * i;
                    int newCol = col + dir.Item2 * i;
    
                    if (!IsValidPosition(newRow, newCol) || cells[newRow, newCol].GetCellType() == CELL_TYPE.White)
                    {
                        if(isBlacked)
                            isOpen++;
                        break;
                    }
    
                    isBlacked = false;
                    
                    if (cells[newRow, newCol].GetCellType() == CELL_TYPE.Black)
                    {
                        isBlacked = true;
                        lineCount++;
                    }
                }
            }
            if (lineCount == 4 && isOpen < 2)
                fourCount++;
        }
        if (fourCount >= 2 )
        {
            return true;
        }
        return false;
    }
    
    private bool CheckLong(int row, int col)
    {
        foreach (var dirs in directions)
        {
            int lineCount = 1;
            foreach (var dir in dirs)
            {
                for (int i = 1; i < 5; i++)
                {
                    int newRow = row + dir.Item1 * i;
                    int newCol = col + dir.Item2 * i;
                    if(IsValidPosition(newRow, newCol) && cells[newRow, newCol].GetCellType() == CELL_TYPE.Black)
                        lineCount++;
                    else
                    {
                        break;
                    }
                }
            }
            if(lineCount >= 6)
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


    //for test
    public void TestRenjuOperate(){
        RenjuRuleLogic.SetBoard(cells);

        List<(int, int)> forbiddenPoints = new List<(int, int)>();

        // for (int row = 0; row < width; row++)
        // {
        //     for (int col = 0; col < height; col++)
        //     {
        //         bool isForbidden = RenjuRuleLogic.IsRenjuRuleViolation(row, col);
        //         if (isForbidden)
        //         {
        //             forbiddenPoints.Add((row, col));
        //         }      
        //     }
        // }

        forbiddenPoints = GetForbiddenPoints();

        foreach (var point in forbiddenPoints)
        {
            TurnData t = new TurnData()
            {
                row = point.Item1,
                col = point.Item2
            };
            OnDrawCell(t, CELL_TYPE.Warning);
        }
    }
}
