using UnityEngine;
using UnityEngine.UI;
using static Constants;

public class MainMenuController : MonoBehaviour, IGameUI
{
    [SerializeField] Button startButton;
    [SerializeField] Button replayButton;
    [SerializeField] Button leaderboardButton;
    [SerializeField] Button shopButton;
    [SerializeField] Button settingButton;


    void Start()
    {
        startButton.onClick.AddListener(OnClickStartButton);
        replayButton.onClick.AddListener(OnClickReplayButton);
        leaderboardButton.onClick.AddListener(OnClickLeaderboardButton);
        shopButton.onClick.AddListener(OnClickShopButton);
        settingButton.onClick.AddListener(OnClickSettingButton);
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
        UIManager.Instance.ShowUI<ShopPanelController>(UI_TYPE.Shop);
    }

    void OnClickSettingButton()
    {
        Debug.Log("SettingButton Clicked");
        UIManager.Instance.GetUI<RectTransform>(UI_TYPE.Setting);        
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