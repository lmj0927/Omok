using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SettingsPanelController : PanelController
{
    public Button closeButton;
    public Button logoutButton;


    void Start()
    {
        closeButton.onClick.AddListener(OnClickCloseButton);   
        logoutButton.onClick.AddListener(OnClickLogoutButton);
    }


    /// <summary>
    /// SFX On/Off시 호출되는 함수
    /// </summary>
    /// <param name="value">On/Off 값</param>
    public void OnSFXToggleValueChanged(bool value)
    {
        
    }
    
    /// <summary>
    /// BGM On/Off시 호출되는 함수
    /// </summary>
    /// <param name="value">On/Off 값</param>
    public void OnBGMToggleValueChanged(bool value)
    {
        
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
        PlayerPrefs.SetString("sid", null); 
        Hide();
        
        UIManager.Instance.GetUI<SigninPanelController>(UI_TYPE.SignIn);
    }
}
