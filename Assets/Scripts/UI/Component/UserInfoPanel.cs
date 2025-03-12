using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInfoPanel : MonoBehaviour
{
    UserInfo _userInfo;
    [SerializeField] Image userImage;
    [SerializeField] Sprite[] profileSprites;
    [SerializeField] TMP_Text infoText;

    public void SetUserInfo(UserInfo userInfo){
        string infoStr = $"{_userInfo.tier} {_userInfo.nickname}";
        infoText.text = infoStr;
        
        _userInfo = userInfo;
        
        userImage.sprite = profileSprites[_userInfo.profileIndex];
    }
}