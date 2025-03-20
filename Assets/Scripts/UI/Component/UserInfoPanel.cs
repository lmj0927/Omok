using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static Constants;

public class UserInfoPanel : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] TMP_Text infoText;
    [SerializeField] Image profileImage;
    [SerializeField] private List<Sprite> _profileSprites = new List<Sprite>();
    
    private PROFILE_STATE _profileState;
    private UserInfo _userInfo;
    private UserInfo? _opponentInfo;

    private void Awake()
    {
        _profileSprites.AddRange(ResourceManager.Instance.ProfileSprites);
    }
    
    private void OnEnable()
    {
        Initialize();
    }

    private void Initialize()
    {
        GameManager.Instance.playerDataController.OnChangedProfile = SetProfileImage;
        SetDefaultInfo();
    }

    //로드 되지않은 상태의 프로필 정보
    private void SetDefaultInfo()
    {
        _userInfo.userId = "";
        _userInfo.nickname = "로드 중..";
        _userInfo.tier = 0;
        _userInfo.score = 0;
        _userInfo.profileIndex = 0;
        _userInfo.winCount = 0;
        _userInfo.loseCount = 0;
        _userInfo.drawCount = 0;
        _profileState = PROFILE_STATE.Unload;
        
        infoText.text = $"{_userInfo.nickname}";
        SetProfileState(_profileState);
        SetProfileImage(_userInfo.profileIndex);
    }

    public void SetProfileState(PROFILE_STATE state)
    {
        switch (state)
        {
            case PROFILE_STATE.Unload:
                profileImage.color = new Color32(80,80,80,255);
                break;
            case PROFILE_STATE.Load:
                profileImage.DOColor(Color.white, 0.2f);
                break;
        }
    }

    public PROFILE_STATE GetProfileState()
    {
        return _profileState;
    }

   
    
    ///기존 userinfo를 갱신하는데 사용합니다. opponentInfo가 있는 경우 인자를 넣어 갱신합니다.
    public async UniTask RefreshInfo(UserInfo? opponentInfo = null)
    {
        await UniTask.WaitUntil(()=>_profileSprites != null);
        
        if (opponentInfo == null) _userInfo =  GameManager.Instance.GetUserInfo();
        else _opponentInfo ??= opponentInfo.Value;

        SetUserInfo(_opponentInfo ?? _userInfo);
    }
    
    ///Userinfo를 새로 할당합니다.
    public void SetUserInfo(UserInfo userInfo)
    {
        _userInfo = userInfo;
        
        string infoStr = $"{_userInfo.tier}급 {_userInfo.nickname}";
        infoText.text = infoStr;
        
        SetProfileImage(_userInfo.profileIndex);
        
        _profileState = PROFILE_STATE.Load;
        SetProfileState(_profileState);
    }
    
    public void SetProfileImage(int idx)
    {
        if (idx < 0 || idx >= _profileSprites.Count)
        {
            Debug.LogError("Profile Index out of range." + idx);
            return;
        }
        
        profileImage.sprite = _profileSprites[idx];
    }
}