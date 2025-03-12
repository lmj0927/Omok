using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct ProfileIndex
{
    [JsonProperty("profileIndex")]
    public int profileIndex;
}

public class EditProfilePanel : PanelController
{
    [SerializeField] Button[] profileImage;
    [SerializeField] private int _profileIndex;
    
    UserInfo _userInfo;
    
    void Start()
    {
        Show();
        
        //TODO: GetScore는 임시로 넣음.
        StartCoroutine(NetworkManage.Instance.GetScore((userInfo) =>
        {
            UIManager.Instance.GetUI<MainMenuController>(UI_TYPE.MainMenu);
        }, () =>
        {
        }));
    }
    
    public void OnClickProfile(int index)
    {
        _profileIndex = index;
        
        //TODO: 변경된 정보 업데이트.
        var userInfoPanel = FindObjectOfType<UserInfoPanel>();
        userInfoPanel.SetUserInfo(_userInfo);
        
        Hide();
    }

    public void OnClickCloseButton()
    {
        Hide();
    }
}
