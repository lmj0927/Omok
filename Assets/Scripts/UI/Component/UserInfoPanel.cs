using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInfoPanel : MonoBehaviour
{
    [SerializeField] Image userImage;
    [SerializeField] Sprite[] profileSprites;
    [SerializeField] TMP_Text infoText;
    private UserInfo _userInfo;
    
    public void SetUserInfo(UserInfo userInfo){
        _userInfo = userInfo;
        
        string infoStr = $"{_userInfo.tier} {_userInfo.nickname}";
        infoText.text = infoStr;
        
        userImage.sprite = profileSprites[_userInfo.profileIndex];
    }
}