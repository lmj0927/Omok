using UnityEngine;
using UnityEngine.UI;

public class GameBoardUIController : MonoBehaviour
{
    public Button giveUpButton;
    public Button executeButton;
    
    void Start()
    {
        giveUpButton.onClick.AddListener(OnClickGiveUpButton);
        executeButton.onClick.AddListener(OnClickExecuteButton);
    }

    public void OnClickGiveUpButton()
    {
        UIManager.Instance.GetUI<ConfirmPanelController>(UI_TYPE.Confirm).Show("정말로 게임을 포기하시겠습니까?", () =>
        {
            GameManager.Instance.GiveUpGame();
            Debug.Log(GameManager.Instance.userInfo.nickname + "님이 게임을 포기하였습니다.");
            Hide();
        });
    }

    public void OnClickExecuteButton()
    {
        GameManager.Instance.matchController.SetTurn();
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