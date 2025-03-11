using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchMakingController : PanelController
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
        //Canel MatchMaking
        GameManager.Instance.matchController.CloseMatchMaking();
        Hide();
    }
}
