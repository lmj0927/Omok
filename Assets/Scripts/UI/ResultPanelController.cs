using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ResultPanelController : PopupController
{
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private Button closeButton;
   
    [SerializeField] private GameObject pointCell;
    [SerializeField] private RectTransform pointCellParent;
    [SerializeField] private TMP_Text plusPointText;
    [SerializeField] private TMP_Text minusPointText; 
    [SerializeField] private TMP_Text tierText; 
    
    private int _tier;
    private int _score;
    
    private RectTransform _rectTransform;
    private bool _isWin;

    private List<GameObject> _cells;
    private UserInfo _userInfo;

    protected new void Awake()
    {
        base.Awake();
        _rectTransform = gameObject.GetComponent<RectTransform>();
        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    public void Show(bool isWin, UserInfo userInfo)
    {
        base.Show();

        _isWin = isWin;
        _userInfo = userInfo;
        
        _score = _userInfo.score;
        _tier = _userInfo.tier == 0 ? 18 : _userInfo.tier;
        
        // var original = _rectTransform.anchoredPosition;
        // _rectTransform.anchoredPosition = new Vector2(Screen.width, original.y);

        //gameObject.SetActive(true);
        // _rectTransform.DOAnchorPosX(original.x, .3f).SetEase(Ease.InQuint);

        if (_isWin)
        {
            resultText.text = "게임에서 승리하셨습니다";
        }
        else
        {
            resultText.text = "게임에서 패배하셨습니다";
        }
        
        SetCell();
        DrawResultCell();
        ChangeTier();
    }

    // public override void Hide()
    // {
    //     var original = _rectTransform.anchoredPosition;

    //     _rectTransform.DOAnchorPosX(Screen.width, .3f).SetEase(Ease.InQuint).OnComplete(() =>
    //     {
    //         gameObject.SetActive(false);
    //         _rectTransform.anchoredPosition = original;
    //     });
    // }
    
    // 초기 점수 세팅 함수
    private void SetCell()
    { 
        ClearCell();
        
        _cells = new List<GameObject>();
        
        if (_tier is > 0 and <= 4)
        {
            for (int i = -10; i < 10; i++)
            {
                GameObject cell = Instantiate(pointCell, pointCellParent);
                cell.GetComponent<RectTransform>().sizeDelta = new Vector2(pointCellParent.rect.width / 20 - 15, 100);
                DrawCell(i, cell);
            }
            plusPointText.text = "100";
            minusPointText.text = "-100";
        }
        else if (_tier is > 4 and <= 9)
        {
            for (int i = -5; i < 5; i++)
            {
                GameObject cell = Instantiate(pointCell, pointCellParent);
                cell.GetComponent<RectTransform>().sizeDelta = new Vector2(pointCellParent.rect.width / 10 - 30, 100);
                DrawCell(i, cell);
            }
            plusPointText.text = "50";
            minusPointText.text = "-50";
        }
        else
        {
            for (int i = -3; i < 3; i++)
            {
                GameObject cell = Instantiate(pointCell, pointCellParent);
                cell.GetComponent<RectTransform>().sizeDelta = new Vector2(pointCellParent.rect.width / 6 - 30 , 100);
                DrawCell(i, cell);
            }

            plusPointText.text = "30";
            minusPointText.text = "-30";
        }
        tierText.text = _tier + "급";
        
    }

    // 이기거나 질 때 그려지는 Cell을 그리는 함수
    void DrawResultCell()
    {
        if (_isWin)
        {
            if((_tier == 18 && _score < -3) || (_tier == 1 && _score >= 10))
                return;
            
            _score++;
            if (_tier is > 0 and <= 4)
            {
                if(_score > 0)
                {
                    _cells[_score + 9].gameObject.GetComponent<Image>().DOColor(Color.red, 0.5f).SetEase(Ease.InQuint);
                }
                else
                {
                    _cells[_score + 9].gameObject.GetComponent<Image>().DOColor(new Color(0,0,0,0), 0.5f).SetEase(Ease.InQuint);
                }
            }
            else if (_tier is > 4 and <= 9)
            {
                if(_score > 0)
                {
                    _cells[_score + 4].gameObject.GetComponent<Image>().DOColor(Color.red, 0.5f).SetEase(Ease.InQuint);
                }
                else
                {
                    _cells[_score + 4].gameObject.GetComponent<Image>().DOColor(new Color(0,0,0,0), 0.5f).SetEase(Ease.InQuint);
                }
            }
            else
            {
                if(_score > 0)
                {
                    _cells[_score + 2].gameObject.GetComponent<Image>().DOColor(Color.red, 0.5f).SetEase(Ease.InQuint);
                }
                else
                {
                    _cells[_score + 2].gameObject.GetComponent<Image>().DOColor(new Color(0,0,0,0), 0.5f).SetEase(Ease.InQuint);
                }
            }
        }
        else
        {
            if ((_tier == 1 && _score > 10) || (_tier == 18 && _score <= -3))
                return;
            
            if (_tier is > 0 and <= 4)
            {
                if(_score > 0)
                {
                    _cells[_score + 9].gameObject.GetComponent<Image>().DOColor(new Color(0,0,0,0), 0.5f).SetEase(Ease.InQuint);
                }
                else
                {
                    _cells[_score + 9].gameObject.GetComponent<Image>().DOColor(Color.red, 0.5f).SetEase(Ease.InQuint);
                }
            }
            else if (_tier is > 4 and <= 9)
            {
                if(_score > 0)
                {
                    _cells[_score + 4].gameObject.GetComponent<Image>().DOColor(new Color(0,0,0,0), 0.5f).SetEase(Ease.InQuint);
                }
                else
                {
                    _cells[_score + 4].gameObject.GetComponent<Image>().DOColor(Color.red, 0.5f).SetEase(Ease.InQuint);
                }
            }
            else
            {
                if(_score > 0)
                {
                    _cells[_score + 2].gameObject.GetComponent<Image>().DOColor(new Color(0,0,0,0), 0.5f).SetEase(Ease.InQuint);
                }
                else
                {
                    _cells[_score + 2].gameObject.GetComponent<Image>().DOColor(Color.red, 0.5f).SetEase(Ease.InQuint);
                }
            }

            _score--;
        }
    }

    // 승급 또는 강등을 판별
    void ChangeTier()
    {
        if (_tier is > 0 and <= 4)
        {
            if (_score == -10)
            {
                Demotion();
            }
            else if (_score == 10 && _tier != 1)
            {
                Promotion();
            }
        }
        else if (_tier is > 4 and <= 9)
        {
            if (_score == -5)
            {
                Demotion();
            }
            else if (_score == 5)
            {
                Promotion();
            }
        }
        else if(_tier is > 9 and <= 18)
        {
            if (_score == -3 && _tier != 18)
            {
                Demotion();
            }
            else if (_score == 3)
            {
                Promotion();
            }
        }
    }

    // 강등
    void Demotion()
    {
        _tier++;
        _score = 0;
        UIManager.Instance.GetUI<ConfirmPanelController>(UI_TYPE.Confirm).Show("강등하셨습니다!", () =>
        {
            //TODO: 승급 애니메이션
            SetCell();
            tierText.text = _tier + "급";
        });
    }

    // 승급
    void Promotion()
    {
        _tier--;
        _score = 0;
        UIManager.Instance.GetUI<ConfirmPanelController>(UI_TYPE.Confirm).Show("승급하셨습니다!", () =>
        {
            //TODO: 승급 애니메이션
            SetCell();
            tierText.text = _tier + "급";
        });
    }
    
    // 기존 점수 그리는 함수
    void DrawCell(int index, GameObject cell)
    {
        if (_score > 0)
        {
            if (index + 1 <= _score && index >= 0)
            {
                cell.GetComponent<Image>().color = Color.white;
            }
        }
        if (_score < 0)
        {
            if(index >= _score &&  index < 0)
            {
                cell.GetComponent<Image>().color = Color.white;
            }
        }
        _cells.Add(cell);
    }

    // Cell 초기화
    void ClearCell()
    {
        if (_cells != null)
        {
            foreach (var cell in _cells)
            {
                Destroy(cell.gameObject);
            }
        }
    }
}
