using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AsnycButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private GameObject _progressSpinner;

    private Func<UniTask> _taskFunc;
    
    private void Start()
    {
        _button.onClick.AddListener(OnClickButton);
    }

    private void OnClickButton()
    {
        if (_taskFunc == null) return;
        ExecuteTask().Forget();
    }

    private async UniTaskVoid ExecuteTask()
    {
        SetButtonPressed();
        
        try
        {
            await _taskFunc().AttachExternalCancellation(this.GetCancellationTokenOnDestroy());
        }
        finally
        {
            if (this != null && this.gameObject != null)
            {
                ResetButtonState();
            }
        }
    }

    private void SetButtonPressed()
    {
        _button.interactable = false;
        _text.gameObject.SetActive(false);
        _progressSpinner.SetActive(true);
    }
    
    private void ResetButtonState()
    {
        _button.interactable = true;
        _text.gameObject.SetActive(true);
        _progressSpinner.SetActive(false);
    }

    public void AddListener(Func<UniTask> taskFunc)
    {
        _taskFunc = taskFunc;
    }
}