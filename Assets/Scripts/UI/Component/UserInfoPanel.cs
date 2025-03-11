using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInfoPanel : MonoBehaviour
{
    UserInfo _userInfo;
    [SerializeField] Image userImage;
    [SerializeField] TextMeshProUGUI infoText;

    public void SetUserInfo(UserInfo userInfo){
        string infoStr = $"{_userInfo.tier} {_userInfo.nickname}";
        infoText.text = infoStr;

        //userInfo.profileIndex
        //userImage.sprite = 

        _userInfo = userInfo;
    }
}