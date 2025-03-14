using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
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

    private RectTransform _settingsPanel;

    protected override void Awake()
    {
        base.Awake();
        _settingsPanel = GetComponent<RectTransform>();
    }

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
        UIManager.Instance.ShowUI<EditProfilePanel>(UI_TYPE.EditProfile);
    }

    /// <summary>
    /// X 버튼 클릭시 호출되는 함수
    /// </summary>
    public void OnClickCloseButton()
    {
        Hide();
    }

    public void OnClickLogoutButton()
    {
        
        UnityWebRequest.ClearCookieCache();
        PlayerPrefs.SetString(Constants.SID, null); 
        Hide();
        
        UIManager.Instance.GetUI<SigninPanelController>(UI_TYPE.SignIn);
    }

    public override void Show()
    {
        var original = _settingsPanel.anchoredPosition;
        _settingsPanel.anchoredPosition = new Vector2(Screen.width, original.y);

        gameObject.SetActive(true);
        _settingsPanel.DOAnchorPosX(original.x, .3f).SetEase(Ease.InQuint);
    }
    
    public override void Hide()
    {
        var original = _settingsPanel.anchoredPosition;

        _settingsPanel.DOAnchorPosX(Screen.width, .3f).SetEase(Ease.InQuint).OnComplete(() =>
        {
            gameObject.SetActive(false);
            _settingsPanel.anchoredPosition = original;
        });
    }
}
