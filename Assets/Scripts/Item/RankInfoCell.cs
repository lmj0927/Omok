using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankInfoCell : MonoBehaviour, IReloadableCell<UserInfo>
{
    [Header("Cell Components")]
    [SerializeField] TMP_Text rankText;
    [SerializeField] TMP_Text infoText;
    [SerializeField] TMP_Text winRateText;
    [SerializeField] Image profileImage;
    [SerializeField] Sprite[] profileSprites;
    private UserInfo _userInfo;
    public void SetData(UserInfo userInfo)
    {
        _userInfo = userInfo;
        
        int index = _userInfo.profileIndex;
        profileImage.sprite = profileSprites[index];
        
        float winRate = (float)_userInfo.winCount / (_userInfo.winCount + _userInfo.loseCount) * 100f;
        infoText.text = $"{_userInfo.tier}급 {_userInfo.nickname}";
        winRateText.text = $"{_userInfo.winCount}승 {_userInfo.loseCount}패 ({winRate:F0}%)";
    }

    public void SetRank(int rankIndex)
    {
        rankText.text = rankIndex.ToString();
    }

    public UserInfo GetInfo()
    {
        return _userInfo;
    }
}
