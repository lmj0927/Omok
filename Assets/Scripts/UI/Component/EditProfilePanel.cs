using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class EditProfilePanel : PanelController
{
    [SerializeField] Button[] profileImage;
    [SerializeField] private int _profileIndex;
    
    UserInfo _userInfo;
    
    void Start()
    {
        Show();
        
        //TODO: userinfo 받기.
        
        // StartCoroutine(NetworkManage.Instance.GetUserInfo((userInfo) =>
        // {
        //     _userinfo = userinfo;
        // }, () =>
        // {
        // }));
    }
    
    public void OnClickProfile(int index)
    {
        _profileIndex = index;
        
        //TODO: 변경된 정보 업데이트.
        
        //_userInfo.profileIndex = index;
        //userInfoPanel.SetUserInfo(_userInfo);
        
        Hide();
    }

    public void OnClickCloseButton()
    {
        Hide();
    }
}
