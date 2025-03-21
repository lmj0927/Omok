using Cysharp.Threading.Tasks;
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
        AudioManager.Instance.PlaySFX("buttonClick");

        UIManager.Instance.ShowUI<NotationUIController>(UI_TYPE.Notation, () =>
        {
            UIManager.Instance.ShowUI<MainMenuController>(UI_TYPE.MainMenu).Forget();
            Hide();
        }).Forget();
    }

    private void OnClickNextButton()
    {
        AudioManager.Instance.PlaySFX("buttonClick");
        GameManager.Instance.matchController.Operate(new OperateCommand() { operateType = OperateType.Draw });
    }

    private void OnClickPrevButton()
    {
        AudioManager.Instance.PlaySFX("buttonClick");
        GameManager.Instance.matchController.Operate(new OperateCommand() { operateType = OperateType.Remove });
    }
    
    private void OnClickFirstButton()
    {
        AudioManager.Instance.PlaySFX("buttonClick");
        GameManager.Instance.matchController.Operate(new OperateCommand() { operateType = OperateType.RemoveAll });
    }

    private void OnClickLastButton()
    {
        AudioManager.Instance.PlaySFX("putStone");
        GameManager.Instance.matchController.Operate(new OperateCommand() { operateType = OperateType.DrawAll });
    }

    public UniTask Show()
    {
        gameObject.SetActive(true);
        return UniTask.CompletedTask;
    }

    public UniTask Hide()
    {
        gameObject.SetActive(false);
        return UniTask.CompletedTask;
    }   
}