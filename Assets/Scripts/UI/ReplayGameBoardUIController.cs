using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ReplayGameBoardUIController : MonoBehaviour, IGameUI
{
    public Button exitButton;
    public Button nextButton;
    public Button prevButton;
    public Button firstButton;
    public Button lastButton;

    public UserInfoPanel userInfoPanel;
    public UserInfoPanel opponentInfoPanel;

    void Start()
    {
        exitButton.onClick.AddListener(OnClickExitButton);
        nextButton.onClick.AddListener(OnClickNextButton);
        prevButton.onClick.AddListener(OnClickPrevButton);
        firstButton.onClick.AddListener(OnClickFirstButton);
        lastButton.onClick.AddListener(OnClickLastButton);
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
    
    private void OnClickFirstButton()
    {
        GameManager.Instance.matchController.Operate(new OperateCommand() { operateType = OperateType.RemoveAll });
    }

    private void OnClickLastButton()
    {
        GameManager.Instance.matchController.Operate(new OperateCommand() { operateType = OperateType.DrawAll });
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