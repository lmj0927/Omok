using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Constants;

public class BoardController : MonoBehaviour
{
    public Cell[,] cells;
    public int width;
    public int height;
    public GameObject cellPrefab;
    public RectTransform cellParent;
    
    
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
        cellParent.GetComponent<GridLayoutGroup>().cellSize = new Vector2(cellParent.rect.width / width, cellParent.rect.height / height);

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
    }

    private void OnDrawCell(TurnData turnData, CELL_TYPE type)
    {
        var row = turnData.row;
        var col = turnData.col;
        cells[row, col].SetCellType(type);
        if (type == CELL_TYPE.Black || type == CELL_TYPE.White)
        {
            cells[row, col].GetComponent<Button>().onClick.RemoveAllListeners();
            Debug.Log(CheckGameResult(row, col));
        }
    }


    #region GameResult
    
    public bool CheckGameResult(int row, int col)
    {
        var matchState = cells[row, col].GetCellType() == CELL_TYPE.Black ? MATCH_STATE.BlackTurn : MATCH_STATE.WhiteTurn;
        
        int count = 0; //count가 4이상이면 오목완성(SetTurn후 불리기에 현재 위치는 자기자신)

        List<(int, int)> dira = new List<(int, int)>{ (0, 1), (0, -1) };
        List<(int, int)> dirb = new List<(int, int)>{ (1, 0), (-1, 0) };
        List<(int, int)> dirc = new List<(int, int)>{ (1, 1), (-1, -1) };
        List<(int, int)> dird = new List<(int, int)>{ (1, -1), (-1, 1) };

        List<List<(int, int)>> directions = new List<List<(int, int)>>
        {
            dira,
            dirb,
            dirc,
            dird
        };
        
        foreach (var dirs in directions)
        {
            foreach (var dir in dirs)
            {
                for (int i = 1; i < 5; i++)
                {
                    if (CheckMark(row + dir.Item1 * i, col + dir.Item2 * i, matchState))
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

    private bool CheckMark(int row, int col, MATCH_STATE matchState)
    {
        if(row < 0 || row >= width || col < 0 || col >= height)
            return false;
        
        if (cells[row, col].GetCellType() == CELL_TYPE.Black && matchState == MATCH_STATE.BlackTurn)
        {
            return true;
        };
        if (cells[row, col].GetCellType() == CELL_TYPE.White && matchState == MATCH_STATE.WhiteTurn)
        {
            return true;
        }
        
        return false;
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
