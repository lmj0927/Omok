using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Constants;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] UserInfoPanel userInfoPanel;
    [SerializeField] CoinPanel coinPanel;
    
    [SerializeField] Button startButton;
    [SerializeField] Button replayButton;
    [SerializeField] Button leaderboardButton;
    [SerializeField] Button shopButton;
    [SerializeField] Button settingButton;
    [SerializeField] Button editProfileButton;
    
    UserInfo _userInfo;

    void Start()
    {
        startButton.onClick.AddListener(OnClickStartButton);
        replayButton.onClick.AddListener(OnClickReplayButton);
        leaderboardButton.onClick.AddListener(OnClickLeaderboardButton);
        shopButton.onClick.AddListener(OnClickShopButton);
        settingButton.onClick.AddListener(OnClickSettingButton);
        editProfileButton.onClick.AddListener(OnClickEditProfileButton);

        //TODO: ~로 부터 userinfo를 받아온다.
        //_userinfo = GameManager.Instance.GetUserInfo
        
        Initialize(_userInfo);
    }

    void Initialize(UserInfo userInfo)
    {
        userInfoPanel.SetUserInfo(userInfo);
        //coinPanel.SetCoinText(userInfo.coin);
    }

    void OnClickEditProfileButton()
    {
        UIManager.Instance.GetUI<RectTransform>(UI_TYPE.EditProfile);
    }

    void OnClickStartButton()
    {
        GameManager.Instance.StartGame(PLAY_TYPE.Multi);
    }

    void OnClickReplayButton()
    {
        GameManager.Instance.StartGame(PLAY_TYPE.Replay);
    }

    void OnClickLeaderboardButton()
    {
        UIManager.Instance.GetUI<RectTransform>(UI_TYPE.Leaderboard);        
    }

    void OnClickShopButton()
    {
        UIManager.Instance.GetUI<RectTransform>(UI_TYPE.Shop);
    }

    void OnClickSettingButton()
    {
        Debug.Log("SettingButton Clicked");
        UIManager.Instance.GetUI<RectTransform>(UI_TYPE.Setting);        
    }
}