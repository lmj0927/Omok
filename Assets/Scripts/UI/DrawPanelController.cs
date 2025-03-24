using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DrawPanelController : PopupController
{
    [Header("Message")]
    [SerializeField] private TMP_Text messageText;
    
    [Header("Timer")]
    [SerializeField] private GameObject timerObj;
    [SerializeField] private Image fillRect;
    private float _timerMaxValue = 15f;
    private float _timerValue;

    [Header("Buttons")]
    [SerializeField] private Button onConfirmButton;
    [SerializeField] private Button onCloseButton;
    
    Action onConfirmButtonClick;
    Action onCloseButtonClick;

    private void Initialize(bool isRequester)
    {
        if (isRequester) GameManager.Instance.matchController.OnReceiveDraw = OnClickCloseButton;
        timerObj.SetActive(!isRequester);
        _timerValue = Time.realtimeSinceStartup;
        fillRect.fillAmount = 1;
        
        onConfirmButton.gameObject.SetActive(!isRequester);
        onCloseButton.gameObject.SetActive(!isRequester);
        
        if (!isRequester) StartCoroutine(OnDrawTimer());
        
        Time.timeScale = 0;
    }

    private IEnumerator OnDrawTimer()
    {
        Debug.Log("OnDrawTimer");
        while (Time.realtimeSinceStartup - _timerValue < _timerMaxValue)
        {
            var currentTime = _timerMaxValue - (Time.realtimeSinceStartup - _timerValue);
            var fillValue = Mathf.InverseLerp(0f,_timerMaxValue, currentTime);
            fillRect.fillAmount = fillValue;
            Debug.Log(currentTime);
            yield return null;
        }
        OnClickCloseButton();
    }

    public void Show(bool isRequester, string message, Action onConfirmButtonClick = null, Action onCloseButtonClick = null)
    {
        transform.SetAsLastSibling();
        
        messageText.text = message;
        this.onConfirmButtonClick = onConfirmButtonClick;
        this.onCloseButtonClick = onCloseButtonClick;
        
        base.Show();
        
        Initialize(isRequester);
    }
    
    /// <summary>
    /// Confirm 버튼 클릭시 호출되는 함수
    /// </summary>
    public void OnClickConfirmButton()
    {
        StopAllCoroutines();
        
        AudioManager.Instance.PlaySFX("buttonClick");
        UIManager.Instance.HideUI<DrawPanelController>(UI_TYPE.Draw, () =>
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
