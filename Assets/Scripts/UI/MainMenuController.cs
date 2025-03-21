using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Constants;

public class MainMenuController : MonoBehaviour, IGameUI
{
    [SerializeField] UserInfoPanel userInfoPanel;
    [SerializeField] CoinPanel coinPanel;
    
    [SerializeField] Button startButton;
    [SerializeField] Button replayButton;
    [SerializeField] Button leaderboardButton;
    [SerializeField] Button shopButton;
    [SerializeField] Button settingButton;
    UserInfo _userInfo;

    CancellationTokenSource _introCts;

    void Start()
    {
        startButton.onClick.AddListener(OnClickStartButton);
        replayButton.onClick.AddListener(OnClickReplayButton);
        leaderboardButton.onClick.AddListener(OnClickLeaderboardButton);
        shopButton.onClick.AddListener(OnClickShopButton);
        settingButton.onClick.AddListener(OnClickSettingButton);
        
        GameManager.Instance.mainUIUpdate = UpdateUserInfo;
        Initialize();
    }

    void OnEnable()
    {
        GameManager.Instance.StoneSpawner.StartSpawn();
        GameManager.Instance.cameraMover.ResetCamera(cameraMoverInitDistance, cameraMoverInitRotateX, cameraMoverInitRotateY); 

        PlayIntro().Forget();
    }

    void OnDisable()
    {   
        _introCts?.Cancel();
        GameManager.Instance.StoneSpawner?.DisableSpawner();
    }

    async UniTask PlayIntro()
    {        
        _introCts?.Cancel();
        _introCts = new CancellationTokenSource();
        await UniTask.Delay(5000, cancellationToken: _introCts.Token);
        GameManager.Instance.cameraMover.SetCamera(new Vector3(cameraMoverIntroRotateX, cameraMoverIntroRotateY, 0), cameraMoverIntroDistance);
    }

    void Initialize()
    {
        UpdateUserInfo();
    }

    void UpdateUserInfo()
    {
        userInfoPanel.RefreshInfo().Forget();
    }

    void OnClickStartButton()
    {
        if (GameManager.Instance.coinController.Coin < Constants.CostPerGame)
        {
            Debug.Log("코인이 부족합니다.");
            var confirmPanel = UIManager.Instance.GetUI<AlarmPanelController>(UI_TYPE.Alarm);
            confirmPanel.Show("코인이 부족합니다.", () =>
            {
                UIManager.Instance.ShowUI<ShopPanelController>(UI_TYPE.Shop).Forget();
            });            
            return;
        }

        AudioManager.Instance.PlaySFX("matchMaking");
        GameManager.Instance.coinController.ConsumeCoin(Constants.CostPerGame);
        GameManager.Instance.StartGame(PLAY_TYPE.Multi);
    }

    void OnClickReplayButton()
    {
        AudioManager.Instance.PlaySFX("buttonClick");
        UIManager.Instance.ShowUI<NotationUIController>(UI_TYPE.Notation).Forget();
        AudioManager.Instance.PlaySFX("panelOpen");
    }

    void OnClickLeaderboardButton()
    {
        AudioManager.Instance.PlaySFX("buttonClick");
        UIManager.Instance.ShowUI<LeaderBoardPanelController>(UI_TYPE.Leaderboard).Forget();
        AudioManager.Instance.PlaySFX("panelOpen");
    }

    void OnClickShopButton()
    {
        AudioManager.Instance.PlaySFX("buttonClick");
        UIManager.Instance.ShowUI<ShopPanelController>(UI_TYPE.Shop).Forget();
        AudioManager.Instance.PlaySFX("panelOpen");
    }

    void OnClickSettingButton()
    {
        AudioManager.Instance.PlaySFX("buttonClick");
        UIManager.Instance.ShowUI<SettingsPanelController>(UI_TYPE.Setting).Forget();
        AudioManager.Instance.PlaySFX("panelOpen");
    }

    public UniTask Show()
    {
        gameObject.SetActive(true);
        return UniTask.CompletedTask;
    }

    public UniTask Hide()
    {
        Dispose();
        gameObject.SetActive(false);        
        return UniTask.CompletedTask;
    }

    void Dispose()
    {
        
    }
}