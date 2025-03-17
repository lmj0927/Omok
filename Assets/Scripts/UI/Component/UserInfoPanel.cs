using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInfoPanel : MonoBehaviour
{
    [SerializeField] TMP_Text infoText;
    [SerializeField] Image profileImage;
    [SerializeField] List<Sprite> profileSprites;
    private UserInfo _userInfo;

    private void Awake()
    {
        SetUserInfo(GameManager.Instance.GetUserInfo());
    }

    private void Start()
    {
        GameManager.Instance.playerDataController.OnChangedProfile += SetProfileImage;
    }

    //설정창에서 갱신이 안되어서 활성화 될때 갱신시킴.
    public void OnEnable()
    {
        //계정 변경 시 playerDataController가 초기화 될 경우 재등록
        GameManager.Instance.playerDataController.OnChangedProfile ??= SetProfileImage;

        SetUserInfo(GameManager.Instance.GetUserInfo());
    }

    public void SetUserInfo(UserInfo userInfo)
    {
        _userInfo = userInfo;
        
        string infoStr = $"{_userInfo.tier}급 {_userInfo.nickname}";
        infoText.text = infoStr;
        
        SetProfileImage(_userInfo.profileIndex);
    }
    
    public void SetProfileImage(int idx)
    {
        if (idx < 0 || idx >= profileSprites.Count)
        {
            Debug.Log("PRCNT"+profileSprites.Count);
            Debug.LogError("Profile Index out of range." + idx);
            return;
        }
        
        profileImage.sprite = profileSprites[idx];
    }
}