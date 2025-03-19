
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class PopupController: MonoBehaviour, IGameUI
{
    [SerializeField] private RectTransform panelRectTransform;      // 팝업창
    private CanvasGroup _backgroundCanvasGroup;                     // 뒤에 시커먼 배경

    public delegate void PanelControllerHideDelegate();
    
    protected virtual void Awake()
    {
        _backgroundCanvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show()
    {
        _backgroundCanvasGroup.alpha = 0;
        panelRectTransform.localScale = Vector3.zero;
        gameObject.SetActive(true);
        
        _backgroundCanvasGroup.DOFade(1, 0.3f).SetEase(Ease.Linear);
        panelRectTransform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
    }

    public virtual void Hide()
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
                gameObject.SetActive(false);
            });

        await sequence.AsyncWaitForCompletion();
    }
}
