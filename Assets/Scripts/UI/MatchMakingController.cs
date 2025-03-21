using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchMakingController : PopupController
{
    public Button closeButton;

    void Start()
    {
        closeButton.onClick.AddListener(OnClickCloseButton);
    }

    /// <summary>
    /// X 버튼 클릭시 호출되는 함수
    /// </summary>
    public void OnClickCloseButton()
    {
        AudioManager.Instance.PlaySFX("buttonClick");
        //Canel MatchMaking
        GameManager.Instance.matchController.CloseMatchMaking();
        Hide();
    }
}
