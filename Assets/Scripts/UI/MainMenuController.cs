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
        GameManager.Instance.StartGame(PLAY_TYPE.Multi);
    }

    void OnClickReplayButton()
    {
        UIManager.Instance.ShowUI<NotationUIController>(UI_TYPE.Notation);        
    }

    void OnClickLeaderboardButton()
    {
        UIManager.Instance.ShowUI<LeaderBoardPanelController>(UI_TYPE.Leaderboard);        
    }

    void OnClickShopButton()
    {
        UIManager.Instance.ShowUI<ShopPanelController>(UI_TYPE.Shop);
    }

    void OnClickSettingButton()
    {
        UIManager.Instance.ShowUI<SettingsPanelController>(UI_TYPE.Setting);        
    }

    public void Show()
    {
        throw new System.NotImplementedException();
    }

    public void Hide()
    {
        throw new System.NotImplementedException();
    }
}