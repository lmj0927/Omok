using UnityEngine;
using UnityEngine.UI;

public class GameBoardUIController : MonoBehaviour
{
    public Button giveUpButton;
    public Button executeButton;

    void Start()
    {
        giveUpButton.onClick.AddListener(OnClickGiveUpButton);
    }

    public void OnClickGiveUpButton()
    {
        UIManager.Instance.GetUI<ConfirmPanelController>(UI_TYPE.Confirm).Show("정말로 게임을 포기하시겠습니까?", () =>
        {
            //GameManager.Instance.GiveUpGame();
            Hide();
        });
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