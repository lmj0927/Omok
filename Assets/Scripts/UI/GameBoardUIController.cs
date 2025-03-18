using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Constants;
public class GameBoardUIController : MonoBehaviour, IGameUI
{
    //조작 버튼
    [Header("Interaction Buttons")]
    public Button giveUpButton;
    public Button executeButton;
    
    //상단 한줄 정보란
    [Header("Head Description")]
    [SerializeField] private Image descriptionPanel;
    [SerializeField] private TMP_Text descriptionText;

    //턴 전환
    [Header("Turn Change Components")]
    [SerializeField] private RectTransform blackTurnPanel;
    [SerializeField] private RectTransform blackTurnRect;
    [SerializeField] private RectTransform whiteTurnPanel;
    [SerializeField] private RectTransform whiteTurnRect;
    [SerializeField] private CanvasGroup blackTurnFadeRect;
    [SerializeField] private CanvasGroup whiteTurnFadeRect;
    
    private float _blackOriginWidth;
    private float _whiteOriginWidth;
    private const float Duration = 0.2f;
    private const float UpScalePanel = 1.5f;
    private const float DownScalePanel = 0.5f;

    //강조 효과
    [Header("Highlight Effect")]
    [SerializeField] private RectTransform boardOutlineRect;
    private CanvasGroup boardOutlineFade;
    [SerializeField] private RectTransform circleEffectRect;
    private CanvasGroup circleEffectFade;
    
    private Image _excuteButtonImage;
    private TMP_Text _excuteButtonText;

    private Action _onRepeatEffect;
    
    MATCH_STATE _currentState = MATCH_STATE.BlackTurn;
    public bool _isBlack = true;
    
    private List<GameObject> _endMarkerObjects = new();
    
    //유저 프로필
    [Header("UserProfiles")]
    [SerializeField] private RectTransform userinfoPanel;
    [SerializeField] private RectTransform opponentinfoPanel;
    private UserInfoPanel _opponentInfo;
    private Vector3 _downScaleInfo = new Vector3(0.75f, 0.75f, 0.75f);
    
    //타이머
    [Header("Timer")]
    [SerializeField] private Image timerCircleImage;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Color32[] timerColors;
    private RectTransform _timerHeadRect;
    private Image _timerHead;
    private Image _timerSeed;
    private bool _isStartMatch = false;
    
    void Start()
    {
        
    }

    //임시 타이머 차감용
    void FixedUpdate()
    {
        if(_isStartMatch)
        {
            GameManager.Instance.matchController.TurnTime -= Time.deltaTime;
            OnTimerCircle(); 
            
            if (GameManager.Instance.matchController.TurnTime <= 0)
            {
                bool isMyTurn = false;
                if(_isBlack && _currentState == MATCH_STATE.BlackTurn)
                {
                    isMyTurn = true;
                }
                else if(!_isBlack && _currentState == MATCH_STATE.WhiteTurn)
                {
                    isMyTurn = true;
                }

                if(isMyTurn)
                {
                    GameManager.Instance.matchController.Surrender();
                }
            }
        }
    }

    // void OnEnable()
    // {
    //     Initialize();
    // }

    public void Initialize()
    {
        giveUpButton.onClick.AddListener(OnClickGiveUpButton);
        executeButton.onClick.AddListener(OnClickExecuteButton);
        
        GameManager.Instance.matchController.OnTurnEndUI = SetChangedTurn;
        GameManager.Instance.matchController.OnGameEndUI = GameEnded;
        
        _opponentInfo = opponentinfoPanel.GetComponent<UserInfoPanel>();
        
        _onRepeatEffect += OnRepeatBoardEffect;
        _onRepeatEffect += OnRepeatCircleEffect;
        
        _excuteButtonImage = executeButton.GetComponent<Image>();
        _excuteButtonText = executeButton.GetComponentInChildren<TMP_Text>();
        
        _timerSeed = timerCircleImage.GetComponentsInChildren<Image>()[1];
        _timerHead = timerCircleImage.GetComponentsInChildren<Image>()[^1];
        _timerHeadRect = timerCircleImage.GetComponentsInChildren<RectTransform>()[^1];
        
        boardOutlineFade = boardOutlineRect.GetComponent<CanvasGroup>();
        circleEffectFade = circleEffectRect.GetComponent<CanvasGroup>();
        
        if(_blackOriginWidth == 0)
        {
            _blackOriginWidth = blackTurnPanel.sizeDelta.x;
            _whiteOriginWidth = whiteTurnPanel.sizeDelta.x;
        }
        
        _isStartMatch = false;
    }

    public void StartMatch()
    {
        _isBlack = GameManager.Instance.matchController.IsClientBlack();
        _isStartMatch = true;
        
        SetChangedTurn();
        OnTimerCircle();
    }
    
    public void OnClickGiveUpButton()
    {
        UIManager.Instance.GetUI<ConfirmPanelController>(UI_TYPE.Confirm).Show("정말로 게임을 포기하시겠습니까?", () =>
        {
            GameManager.Instance.GiveUpGame();
            Debug.Log(GameManager.Instance.GetUserInfo().nickname + "님이 게임을 포기하였습니다.");
        });
    }
    
    //착수버튼
    public void OnClickExecuteButton()
    {
        GameManager.Instance.matchController.SetTurn();
    }
    
    //매치 정상 완료 후, 퇴장 버튼
    void OnClickEndButton(Action onComplete)
    {
        UIManager.Instance.GetUI<ConfirmPanelController>(UI_TYPE.Confirm).Show("로비로 돌아갑니다.", () =>
        {
            onComplete?.Invoke();
            UIManager.Instance.GetUI<GameBoardUIController>(UI_TYPE.Game).Hide();
        });
    }
    
    void GameEnded(bool isBlackWin, Action onComplete)
    {
        //TODO: 게임이 바로 종료 되지 않고 완성된 오목 한줄을 강조함.
        if (_endMarkerObjects != null)
        {
            foreach (var cell in GameManager.Instance.matchController.FiveCells)
            {
                var cellTr = cell.GetComponent<RectTransform>();
                var markerBg = new GameObject("OmokDot", typeof(Image));
                markerBg.transform.SetParent(cellTr, false);
                markerBg.GetComponent<Image>().sprite = ResourceManager.Instance.endMarker;
                markerBg.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);
                _endMarkerObjects.Add(markerBg);
            }
        }
        else
        {
            int i = 0;
            foreach (var cell in GameManager.Instance.matchController.FiveCells)
            {
                var cellTr = cell.GetComponent<RectTransform>();
                _endMarkerObjects[i].SetActive(true);
                _endMarkerObjects[i].transform.SetParent(cellTr);
                _endMarkerObjects[i].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                i++;
            }
        }
    
        //TODO: 게임 종료 후에는 종료 버튼을 제외한 어떤 버튼도 눌리지 않도록 처리. 재시작 시 이 사항 모두 초기화.
        _isStartMatch = false;
        giveUpButton.interactable = false;
        
        descriptionPanel.DOColor(isBlackWin ? Color.black : Color.white, Duration);
        descriptionText.DOColor(isBlackWin ? Color.white : Color.black, Duration);
        descriptionText.text = isBlackWin ? "흑의 승리 입니다!" : "백의  승리입니다!";
    
        //TODO: EndMatch가 호출되면 기권 버튼을 Disable 하고 착수 버튼을 퇴장 버튼으로 바꾼다.
    
        _excuteButtonText.text = "퇴장";
        _excuteButtonText.DOColor(Color.white, Duration);
        _excuteButtonImage.DOColor(timerColors[1], Duration);
    
        executeButton.onClick.RemoveAllListeners();
        executeButton.onClick.AddListener(() => OnClickEndButton(onComplete));
        executeButton.interactable = true;
    }

    public void SetChangedTurn()
    {
        //시간 초기화
        GameManager.Instance.matchController.TurnTime = 30f;

        //내가 흑돌인가?
        _isBlack = GameManager.Instance.matchController.IsClientBlack();

        //현재 어느 턴인지?
        _currentState = GameManager.Instance.matchController.GetMatchState();

        //플레이어 턴이면 강조 이펙트 켜기
        _onRepeatEffect?.Invoke();

        //턴 전환
        OnChangedTurn(_currentState);
    }

    public void OnChangedTurn(MATCH_STATE currentState)
    {
        //턴 전환 애니메이션
        switch (currentState)
        {
            case MATCH_STATE.BlackTurn:
                descriptionText.text = _isBlack ? "당신의 턴" : "상대방의 턴";
                descriptionText.DOColor(Color.white, Duration);
                descriptionPanel.DOColor(Color.black, Duration);

                executeButton.interactable = _isBlack;
                _excuteButtonText.text = _isBlack ? "착수" : "대기중";
                _excuteButtonText.DOColor(_isBlack ? Color.white : Color.black, Duration);
                _excuteButtonImage.DOColor(_isBlack ? Color.black : timerColors[0], Duration);

                giveUpButton.interactable = _isBlack;

                timerCircleImage.DOColor(_isBlack ? timerColors[0] : timerColors[1], Duration);
                _timerSeed.DOColor(_isBlack ? timerColors[0] : timerColors[1], Duration);
                _timerHead.DOColor(_isBlack ? timerColors[0] : timerColors[1], Duration);

                blackTurnPanel.DOSizeDelta(new Vector2(_blackOriginWidth * UpScalePanel, blackTurnPanel.sizeDelta.y),
                        Duration)
                    .OnUpdate(() =>
                        LayoutRebuilder.ForceRebuildLayoutImmediate(blackTurnPanel.parent as RectTransform));
                whiteTurnPanel.DOSizeDelta(new Vector2(_whiteOriginWidth * DownScalePanel, whiteTurnPanel.sizeDelta.y),
                        Duration)
                    .OnUpdate(() =>
                        LayoutRebuilder.ForceRebuildLayoutImmediate(whiteTurnPanel.parent as RectTransform));

                blackTurnRect.DOScale(new Vector3(UpScalePanel, 1f, 1f), Duration);
                whiteTurnRect.DOScale(new Vector3(DownScalePanel, 1f, 1f), Duration);

                userinfoPanel.DOScale(Vector3.one, Duration);
                opponentinfoPanel.DOScale(_downScaleInfo, Duration);

                blackTurnFadeRect.DOFade(0, Duration);
                whiteTurnFadeRect.DOFade(1, Duration);
                break;
            case MATCH_STATE.WhiteTurn:
                descriptionText.text = !_isBlack ? "당신의 턴" : "상대방의 턴";
                descriptionText.DOColor(Color.white, Duration);
                descriptionPanel.DOColor(Color.black, Duration);

                executeButton.interactable = !_isBlack ? true : false;
                _excuteButtonText.text = !_isBlack ? "착수" : "대기중";
                _excuteButtonText.DOColor(!_isBlack ? Color.black : Color.black, Duration);
                _excuteButtonImage.DOColor(!_isBlack ? Color.white : timerColors[0], Duration);

                giveUpButton.interactable = !_isBlack;

                timerCircleImage.DOColor(!_isBlack ? timerColors[0] : timerColors[1], Duration);
                _timerSeed.DOColor(!_isBlack ? timerColors[0] : timerColors[1], Duration);
                _timerHead.DOColor(!_isBlack ? timerColors[0] : timerColors[1], Duration);

                blackTurnPanel.DOSizeDelta(new Vector2(_blackOriginWidth * DownScalePanel, blackTurnPanel.sizeDelta.y),
                        Duration)
                    .OnUpdate(() =>
                        LayoutRebuilder.ForceRebuildLayoutImmediate(blackTurnPanel.parent as RectTransform));
                whiteTurnPanel.DOSizeDelta(new Vector2(_whiteOriginWidth * UpScalePanel, whiteTurnPanel.sizeDelta.y),
                        Duration)
                    .OnUpdate(() =>
                        LayoutRebuilder.ForceRebuildLayoutImmediate(whiteTurnPanel.parent as RectTransform));

                blackTurnRect.DOScale(new Vector3(DownScalePanel, 1f), Duration);
                whiteTurnRect.DOScale(new Vector3(UpScalePanel, 1f), Duration);
                userinfoPanel.DOScale(_downScaleInfo, Duration);
                opponentinfoPanel.DOScale(Vector3.one, Duration);

                blackTurnFadeRect.DOFade(1, Duration);
                whiteTurnFadeRect.DOFade(0, Duration);
                break;
        }
    }

    void OnTimerCircle()
    {
        float fillValue = Mathf.InverseLerp(0f,30f,GameManager.Instance.matchController.TurnTime);
        float rotateValue = Mathf.Lerp(0f,360f,fillValue);
        
        timerCircleImage.fillAmount = fillValue;
        _timerHeadRect.rotation = Quaternion.Euler(0f, 0f, rotateValue);
        
        timerText.text = $"{GameManager.Instance.matchController.TurnTime:F2}";
    }

    void OnRepeatCircleEffect()
    {
        if ((_currentState == MATCH_STATE.BlackTurn && _isBlack) ||
            (_currentState == MATCH_STATE.WhiteTurn && !_isBlack))
        {
            circleEffectRect.DOScale(Vector3.one, 0);
            circleEffectFade.DOFade(1, 0);

            circleEffectRect.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 1);
            circleEffectFade.DOFade(0, 2).OnComplete(OnRepeatCircleEffect);
        }
    }

    void OnRepeatBoardEffect()
    {
        if ((_currentState == MATCH_STATE.BlackTurn && _isBlack) ||
            (_currentState == MATCH_STATE.WhiteTurn && !_isBlack))
        {
            boardOutlineRect.DOScale(Vector3.one, 0);
            boardOutlineFade.DOFade(1, 0);

            boardOutlineRect.DOScale(new Vector3(1.05f, 1.05f, 1.05f), 1);
            boardOutlineFade.DOFade(0, 2).OnComplete(OnRepeatBoardEffect);
            //     () =>
            // {
            //     boardOutlineRect.DOScale(Vector3.one, 1);
            //     boardOutlineFade.DOFade(1, 2).OnComplete(OnRepeatBoardEffect);
            // });]
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}