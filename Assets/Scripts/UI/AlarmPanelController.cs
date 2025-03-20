using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class AlarmPanelController : PopupController
{
    [SerializeField] private TMP_Text messageText;

    public delegate void OnCloseButtonClick();
    private OnCloseButtonClick onCloseButtonClick;

    public void Show(string message, OnCloseButtonClick onCloseButtonClick)
    {
        transform.SetAsLastSibling();

        messageText.text = message;
        this.onCloseButtonClick = onCloseButtonClick;
        base.Show();
    }
    
    /// <summary>
    /// Confirm 버튼 클릭시 호출되는 함수
    /// </summary>
    public void OnClickCloseButton()
    {
        UIManager.Instance.HideUI<AlarmPanelController>(UI_TYPE.Alarm, () =>
        {
            onCloseButtonClick?.Invoke();
        });
    }
}
