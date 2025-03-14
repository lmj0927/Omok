using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInfoPanel : MonoBehaviour
{
    [SerializeField] TMP_Text infoText;
    [SerializeField] ProfilePanel profilePanel;
    private UserInfo _userInfo;

    private void Awake()
    {
        SetUserInfo(GameManager.Instance.GetUserInfo());
    }

    public void SetUserInfo(UserInfo userInfo)
    {
        _userInfo = userInfo;
        
        string infoStr = $"{_userInfo.tier}급 {_userInfo.nickname}";
        infoText.text = infoStr;
        
        profilePanel.SetProfileImage(_userInfo.profileIndex);
    }
}