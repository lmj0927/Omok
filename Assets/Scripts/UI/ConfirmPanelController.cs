using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using System;

public class ConfirmPanelController : PopupController
{
    [SerializeField] private TMP_Text messageText;
    Action onConfirmButtonClick;
    Action onCloseButtonClick;

    public void Show(string message, Action onConfirmButtonClick, Action onCloseButtonClick = null)
    {
        transform.SetAsLastSibling();

        messageText.text = message;
        this.onConfirmButtonClick = onConfirmButtonClick;
        this.onCloseButtonClick = onCloseButtonClick;
        base.Show();
    }
    
    /// <summary>
    /// Confirm 버튼 클릭시 호출되는 함수
    /// </summary>
    public void OnClickConfirmButton()
    {
        AudioManager.Instance.PlaySFX("buttonClick");
        UIManager.Instance.HideUI<ConfirmPanelController>(UI_TYPE.Confirm, () =>
        {
            AudioManager.Instance.PlaySFX("buttonClick");
            onConfirmButtonClick?.Invoke();
        });
    }

    /// <summary>
    /// X 버튼 클릭시 호출되는 함수
    /// </summary>
    public void OnClickCloseButton()
    {
        AudioManager.Instance.PlaySFX("buttonClick");
        Hide();
        onCloseButtonClick?.Invoke();
    }
}
