using UnityEngine;
using UnityEngine.UI;

public class ReplayGameBoardUIController : MonoBehaviour, IGameUI
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

    public void Initialize(UserInfo userInfo, UserInfo opponentInfo)
    {
        userInfoPanel.SetUserInfo(userInfo);
        opponentInfoPanel.SetUserInfo(opponentInfo);    
    }

    private void OnClickExitButton()
    {
        Hide();
    }

    private void OnClickNextButton()
    {
        GameManager.Instance.matchController.Operate(new OperateCommand() { operateType = OperateType.Draw });
    }

    private void OnClickPrevButton()
    {
        GameManager.Instance.matchController.Operate(new OperateCommand() { operateType = OperateType.Remove });
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