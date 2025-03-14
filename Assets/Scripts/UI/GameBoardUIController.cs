using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Constants;
public class GameBoardUIController : MonoBehaviour
{
    public Button giveUpButton;
    public Button executeButton;
    //상단 한줄 정보란
    [SerializeField] private Image descriptionPanel;
    [SerializeField] private TMP_Text descriptionText;

    //턴 전환
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
    [SerializeField] private RectTransform boardOutlineRect;
    private CanvasGroup boardOutlineFade;
    [SerializeField] private RectTransform circleEffectRect;
    private CanvasGroup circleEffectFade;
    
    private Image _excuteButtonImage;
    private TMP_Text _excuteButtonText;

    private delegate void OnRepeatEffect();
    private OnRepeatEffect _onRepeatEffect;
    
    [SerializeField]MATCH_STATE _currentState = MATCH_STATE.BlackTurn;
    public bool _isBlack = true;
    
    //유저 프로필
    [SerializeField] private RectTransform userinfoPanel;
    [SerializeField] private RectTransform opponentinfoPanel;
    private Vector3 _downScaleInfo = new Vector3(0.75f, 0.75f, 0.75f);
    
    //타이머
    [SerializeField] private Image timerCircleImage;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Color32[] timerColors;
    private RectTransform _timerHeadRect;
    private Image _timerHead;
    private Image _timerSeed;
    
    void Start()
    {
        giveUpButton.onClick.AddListener(OnClickGiveUpButton);
        executeButton.onClick.AddListener(OnClickExecuteButton);
        
        GameManager.Instance.matchController.OnTurnEndUI = SetChangedTurn;
        GameManager.Instance.matchController.OnInitBoardUI = Initialize;
        
        _onRepeatEffect += OnRepeatBoardEffect;
        _onRepeatEffect += OnRepeatCircleEffect;
        
        _excuteButtonImage = executeButton.GetComponent<Image>();
        _excuteButtonText = executeButton.GetComponentInChildren<TMP_Text>();
        
        _timerSeed = timerCircleImage.GetComponentsInChildren<Image>()[1];
        _timerHead = timerCircleImage.GetComponentsInChildren<Image>()[^1];
        _timerHeadRect = timerCircleImage.GetComponentsInChildren<RectTransform>()[^1];
        
        boardOutlineFade = boardOutlineRect.GetComponent<CanvasGroup>();
        circleEffectFade = circleEffectRect.GetComponent<CanvasGroup>();
        
        _blackOriginWidth = blackTurnPanel.sizeDelta.x;
        _whiteOriginWidth = whiteTurnPanel.sizeDelta.x;
    }
    
    //임시 타이머 차감용
    void Update()
    {
        GameManager.Instance.matchController.TurnTime -= Time.deltaTime;
        OnTimerCircle(); 
    }

    private void Initialize()
    {
        SetChangedTurn();
        
        //
        OnTimerCircle();
        
        //
        if (GameManager.Instance.matchController.IsClientBlack()) _isBlack = true;
        else _isBlack = false;
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

    public void SetChangedTurn()
    {
        //시간 초기화
        GameManager.Instance.matchController.TurnTime = 30f;
        
        //내가 흑돌인가?
        if (GameManager.Instance.matchController.IsClientBlack()) _isBlack = true;
        else _isBlack = false;
        
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
                if (_isBlack)
                {
                    executeButton.interactable = true;
                    descriptionText.text = "당신의 턴";
                    _excuteButtonText.text = "착수";
                    _excuteButtonText.DOColor(Color.white, Duration);
                    _excuteButtonImage.DOColor(Color.black, Duration);
                    timerCircleImage.DOColor(timerColors[0], Duration);
                    _timerSeed.DOColor(timerColors[0], Duration);
                    _timerHead.DOColor(timerColors[0], Duration);
                }
                else
                {
                    executeButton.interactable = false;
                    descriptionText.text = "상대방의 턴";
                    _excuteButtonText.text = "대기중";
                    _excuteButtonText.DOColor(Color.black, Duration);
                    _excuteButtonImage.DOColor(timerColors[0], Duration);
                    timerCircleImage.DOColor(timerColors[1], Duration);
                    _timerSeed.DOColor(timerColors[1], Duration);
                    _timerHead.DOColor(timerColors[1], Duration);
                }
                descriptionPanel.DOColor(Color.black, Duration);
                descriptionText.DOColor(Color.white, Duration);
                
                blackTurnPanel.DOSizeDelta(new Vector2(_blackOriginWidth*UpScalePanel, blackTurnPanel.sizeDelta.y), Duration)
                    .OnUpdate(() => LayoutRebuilder.ForceRebuildLayoutImmediate(blackTurnPanel.parent as RectTransform));
                whiteTurnPanel.DOSizeDelta(new Vector2(_whiteOriginWidth*DownScalePanel, whiteTurnPanel.sizeDelta.y), Duration)
                    .OnUpdate(() => LayoutRebuilder.ForceRebuildLayoutImmediate(whiteTurnPanel.parent as RectTransform));;
                
                blackTurnRect.DOScale(new Vector3(UpScalePanel, 1f,1f), Duration);
                whiteTurnRect.DOScale(new Vector3(DownScalePanel, 1f,1f), Duration);
                
                userinfoPanel.DOScale(Vector3.one, Duration);
                opponentinfoPanel.DOScale(_downScaleInfo, Duration);
                
                blackTurnFadeRect.DOFade(0, Duration);
                whiteTurnFadeRect.DOFade(1, Duration);
                break;
            case MATCH_STATE.WhiteTurn:
                if (!_isBlack)
                {
                    executeButton.interactable = true;
                    descriptionText.text = "당신의 턴";
                    _excuteButtonText.text = "착수";
                    _excuteButtonText.DOColor(Color.black, Duration);
                    _excuteButtonImage.DOColor(Color.white, Duration);
                    timerCircleImage.DOColor(timerColors[0], Duration);
                    _timerSeed.DOColor(timerColors[0], Duration);
                    _timerHead.DOColor(timerColors[0], Duration);
                }
                else
                {
                    executeButton.interactable = false;
                    descriptionText.text = "상대방의 턴";
                    _excuteButtonText.text = "대기중";
                    _excuteButtonText.DOColor(Color.black, Duration);
                    _excuteButtonImage.DOColor(timerColors[0], Duration);
                    timerCircleImage.DOColor(timerColors[1], Duration);
                    _timerSeed.DOColor(timerColors[1], Duration);
                    _timerHead.DOColor(timerColors[1], Duration);
                }
                
                descriptionPanel.DOColor(Color.white, Duration);
                descriptionText.DOColor(Color.black, Duration);
                
                blackTurnPanel.DOSizeDelta(new Vector2(_blackOriginWidth*DownScalePanel, blackTurnPanel.sizeDelta.y), Duration)
                    .OnUpdate(() => LayoutRebuilder.ForceRebuildLayoutImmediate(blackTurnPanel.parent as RectTransform));
                whiteTurnPanel.DOSizeDelta(new Vector2(_whiteOriginWidth*UpScalePanel, whiteTurnPanel.sizeDelta.y), Duration)
                    .OnUpdate(() => LayoutRebuilder.ForceRebuildLayoutImmediate(whiteTurnPanel.parent as RectTransform));;
                
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
            // });
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