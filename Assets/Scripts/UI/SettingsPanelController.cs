using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelController : PanelController
{
    public Button profileButton;
    public Button closeButton;
    public Button logoutButton;

    void Start()
    {
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
        PlayerPrefs.SetString("sid", null); 
        Hide();
        
        UIManager.Instance.GetUI<SigninPanelController>(UI_TYPE.SignIn);
    }
}
