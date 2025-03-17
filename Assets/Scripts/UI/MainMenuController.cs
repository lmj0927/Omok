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

        //TODO: ~로 부터 userinfo를 받아온다.
        //_userInfo = GameManger.Instance.GetUserInfo();
        
        //Initialize(_userInfo);
    }

    void Initialize(UserInfo userInfo)
    {
        //userInfoPanel.SetUserInfo(userInfo);
        UpdateUserInfo();
    }

    void UpdateUserInfo()
    {
        //_userInfo = GameManger.Instance.GetUserInfo();
        //coinPanel.SetCoinText(userInfo.text);
    }

    void OnClickStartButton()
    {
        if (GameManager.Instance.coinController.Coin < Constants.CostPerGame)
        {
            Debug.Log("코인이 부족합니다.");
            var confirmPanel = UIManager.Instance.GetUI<ConfirmPanelController>(UI_TYPE.Confirm);
            confirmPanel.Show("코인이 부족합니다.", () =>
            {
                //상점 UI 열기??
                UIManager.Instance.ShowUI<ShopPanelController>(UI_TYPE.Shop);
            });            
            return;
        }
        
        GameManager.Instance.coinController.ConsumeCoin(Constants.CostPerGame);
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