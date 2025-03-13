using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class EditProfilePanel : PanelController
{
    [SerializeField] Button[] profileImage;
    private int _profileIndex;
    
    UserInfo _userInfo;
    
    void Start()
    {
        Show();
        
        //TODO: userinfo 받기.
        
        //_userInfo = GameManger.Instance.GetUserInfo();
    }
    
    public void OnClickProfile(int index)
    {
        _profileIndex = index;
        GameManager.Instance.playerDataController.SetProfile(index);
        
        Hide();
    }

    public void OnClickCloseButton()
    {
        Hide();
    }
}
