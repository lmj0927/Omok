using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SettingsPanelController : PanelController
{
    [SerializeField] private HoverEventButton profileButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button logoutButton;
    [SerializeField] private GameObject editIconPanel;
    [SerializeField] private UserInfoPanel userInfoPanel;

    void Start()
    {
        profileButton.onMouseEnter += () => { editIconPanel.SetActive(true); };
        profileButton.onMouseExit += ()=> { editIconPanel.SetActive(false); };
        
        profileButton.onClick.AddListener(OnClickProfileButton);   
        closeButton.onClick.AddListener(OnClickCloseButton);   
        logoutButton.onClick.AddListener(OnClickLogoutButton);
    }
    
    private void OnClickProfileButton()
    {
        UIManager.Instance.ShowUI<EditProfilePanel>(UI_TYPE.EditProfile).Forget();
    }

    /// <summary>
    /// X 버튼 클릭시 호출되는 함수
    /// </summary>
    private void OnClickCloseButton()
    {
        Hide();
    }

    private void OnClickLogoutButton()
    {
        UnityWebRequest.ClearCookieCache();
        PlayerPrefs.SetString(Constants.SID, null); 
        Hide();
        
        UIManager.Instance.HideUI<MainMenuController>(UI_TYPE.MainMenu);
        UIManager.Instance.ShowUI<SigninPanelController>(UI_TYPE.SignIn).Forget();
    }
}
