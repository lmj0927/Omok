using UnityEngine;
using UnityEngine.UI;

public class ReplayGameBoardUIController : MonoBehaviour
{
    public Button exitButton;
    public Button nextButton;
    public Button prevbutton;

    public UserInfoPanel userInfoPanel;
    public UserInfoPanel opponentInfoPanel;

    void Start()
    {
        exitButton.onClick.AddListener(OnClickExitButton);
        nextButton.onClick.AddListener(OnClickNextButton);
        prevbutton.onClick.AddListener(OnClickPrevButton);
    }

    void Initialize(UserInfo userInfo, UserInfo opponentInfo)
    {
        userInfoPanel.SetUserInfo(userInfo);
        opponentInfoPanel.SetUserInfo(opponentInfo);    
    }

    public void OnClickExitButton()
    {
        Hide();
    }

    public void OnClickNextButton()
    {
    }

    public void OnClickPrevButton()
    {
    }


    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }   
}