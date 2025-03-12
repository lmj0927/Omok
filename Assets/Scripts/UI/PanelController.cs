using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;


[RequireComponent(typeof(CanvasGroup))]
public class PanelController : MonoBehaviour, IGameUI
{
    [SerializeField] private RectTransform panelRectTransform;      // 팝업창
    
    private CanvasGroup _backgroundCanvasGroup;                     // 뒤에 시커먼 배경
    
    public delegate void PanelControllerHideDelegate();
    
    private void Awake()
    {
        _backgroundCanvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// Panel 표시 함수
    /// </summary>
    public void Show()
    {
        _backgroundCanvasGroup.alpha = 0;
        panelRectTransform.localScale = Vector3.zero;
        
        _backgroundCanvasGroup.DOFade(1, 0.3f).SetEase(Ease.Linear);
        panelRectTransform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
    }
    
    /// <summary>
    /// Panel 숨기기 함수
    /// </summary>
    public void Hide()
    {
        HideAsync().Forget();
    }
    
    public async void Hide(PanelControllerHideDelegate hideDelegate)
    {
        await HideAsync();
        hideDelegate?.Invoke();
    }
    
    protected async UniTask HideAsync()
    {
        _backgroundCanvasGroup.alpha = 1;
        panelRectTransform.localScale = Vector3.one;
    
        var sequence = DOTween.Sequence();
        sequence.Join(_backgroundCanvasGroup.DOFade(0, 0.3f).SetEase(Ease.Linear))
            .Join(panelRectTransform.DOScale(0, 0.3f).SetEase(Ease.InBack))
            .OnComplete(() =>
            {
                Destroy(gameObject);
            });

        await sequence.AsyncWaitForCompletion();
    }
}
