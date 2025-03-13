using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Constants;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;
using Button = UnityEngine.UI.Button;
using AYellowpaper.SerializedCollections;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour
{
    CELL_TYPE _cellType;
    Image _image;
    public SerializedDictionary<CELL_TYPE, Texture2D> _cellSprite = new SerializedDictionary<CELL_TYPE, Texture2D>();

    public int row;
    public int col;
    
    
    void Start()
    {
        _image = GetComponent<Image>();
        GetComponent<Button>().onClick.AddListener(OnClickCell);
        _image.color = new Color(0, 0, 0, 0);
    }
    
    void OnClickCell()
    {
        
        GameManager.Instance.matchController.SetCurrentCell(this);
    }

    public void Initialize(int row, int col)
    {
        this.row = row;
        this.col = col;
        _cellType = CELL_TYPE.None;
        
    }
    
    public void SetCellType(CELL_TYPE cellType)
    {
        _cellType = cellType;

        var cellTexture = _cellSprite[cellType];

        if(cellTexture == null)
        {
            _image.color = new Color(0, 0, 0, 0);
        }
        else
        {
            _image.sprite = Sprite.Create(cellTexture, new Rect(0, 0, cellTexture.width, cellTexture.height), new Vector2(0.5f, 0.5f));
            if(cellType == CELL_TYPE.PreviewBlack || cellType == CELL_TYPE.PreviewWhite)
            {
                _image.color = new Color(1, 1, 1, 0.5f);
            }
            else
            {
                _image.color = new Color(1, 1, 1, 1);
            }
        }

        if (cellType == CELL_TYPE.Black || cellType == CELL_TYPE.White || cellType == CELL_TYPE.Warning)
        {
            GetComponent<Button>().onClick.RemoveListener(OnClickCell);
        }
        else
        {
            GetComponent<Button>().onClick.AddListener(OnClickCell);
        }
    }

    public CELL_TYPE GetCellType()
    {
        return _cellType;
    }
}
