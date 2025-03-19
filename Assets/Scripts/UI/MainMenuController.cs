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
        GameManager.Instance.StoneSpawner?.DisableSpawner();
    }

    async UniTask PlayIntro(){
        await UniTask.Delay(5000);
        GameManager.Instance.cameraMover.SetCamera(new Vector3(cameraMoverIntroRotateX, cameraMoverIntroRotateY, 0), cameraMoverIntroDistance);
    }

    void Initialize()
    {
        UpdateUserInfo();
    }

    void UpdateUserInfo()
    {
        _userInfo = GameManager.Instance.GetUserInfo();
        userInfoPanel.SetUserInfo(_userInfo);
    }

    void OnClickStartButton()
    {
        if (GameManager.Instance.coinController.Coin < Constants.CostPerGame)
        {
            Debug.Log("코인이 부족합니다.");
            var confirmPanel = UIManager.Instance.GetUI<ConfirmPanelController>(UI_TYPE.Confirm);
            confirmPanel.Show("코인이 부족합니다.", () =>
            {
                UIManager.Instance.ShowUI<ShopPanelController>(UI_TYPE.Shop).Forget();
            });            
            return;
        }
        
        GameManager.Instance.coinController.ConsumeCoin(Constants.CostPerGame);
        GameManager.Instance.StartGame(PLAY_TYPE.Multi);
    }

    void OnClickReplayButton()
    {
        UIManager.Instance.ShowUI<NotationUIController>(UI_TYPE.Notation).Forget();
    }

    void OnClickLeaderboardButton()
    {
        UIManager.Instance.ShowUI<LeaderBoardPanelController>(UI_TYPE.Leaderboard).Forget();
    }

    void OnClickShopButton()
    {
        UIManager.Instance.ShowUI<ShopPanelController>(UI_TYPE.Shop).Forget();
    }

    void OnClickSettingButton()
    {
        UIManager.Instance.ShowUI<SettingsPanelController>(UI_TYPE.Setting).Forget();
    }

    public UniTask Show()
    {
        gameObject.SetActive(true);
        return UniTask.CompletedTask;
    }

    public UniTask Hide()
    {
        gameObject.SetActive(false);        
        return UniTask.CompletedTask;
    }

    void Dispose()
    {

    }
}