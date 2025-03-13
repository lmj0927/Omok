using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelController : PanelController
{
    [SerializeField] private HoverEventButton profileButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button logoutButton;
    [SerializeField] private GameObject editIconPanel;
    [SerializeField] private ProfilePanel profilePanel;

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
        var editProfilePanel = UIManager.Instance.ShowUI<EditProfilePanel>(UI_TYPE.EditProfile);
        
        editProfilePanel.onChangedProfile -= OnProfileChanged;
        editProfilePanel.onChangedProfile += OnProfileChanged;
    }

    private void OnProfileChanged(int idx)
    {
        profilePanel.SetProfileImage(idx);
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
        PlayerPrefs.SetString("sid", null); 
        Hide();
        
        UIManager.Instance.GetUI<SigninPanelController>(UI_TYPE.SignIn);
    }
}
