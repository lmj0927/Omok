using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;


[RequireComponent(typeof(CanvasGroup))]
public class PanelController : MonoBehaviour, IGameUI
{
    [SerializeField] protected RectTransform panelRectTransform;
    protected List<IGameUI> childPanels = new List<IGameUI>();

    readonly static Vector2 _InvalidPosition = new Vector2(-999, -999);
    Vector2 _originalPosition = _InvalidPosition;
    
    /// <summary>
    /// Panel 표시 함수
    /// </summary>
    public virtual UniTask Show()
    {
        if(_originalPosition == _InvalidPosition)
            _originalPosition = panelRectTransform.anchoredPosition;
        
        panelRectTransform.anchoredPosition = new Vector2(Screen.width, _originalPosition.y);

        var tcs = new UniTaskCompletionSource();
        
        gameObject.SetActive(true);
        panelRectTransform.DOAnchorPosX(_originalPosition.x, .3f).SetEase(Ease.InQuint).OnComplete(() =>
        {
            tcs.TrySetResult();
        });
        
        return tcs.Task;
    }
    
    /// <summary>
    /// Panel 숨기기 함수
    /// </summary>
    public virtual UniTask Hide()
    {
        if (childPanels is { Count: > 0 })
        {
            foreach (var childPanel in childPanels)
            {
                childPanel.Hide();
            }
        }
        
        var original = panelRectTransform.anchoredPosition;
        
        var tcs = new UniTaskCompletionSource();

        panelRectTransform.DOAnchorPosX(Screen.width, .3f).SetEase(Ease.InQuint).OnComplete(() =>
        {
            gameObject.SetActive(false);
            panelRectTransform.anchoredPosition = original;
            tcs.TrySetResult();
        });
        
        return tcs.Task;
    }
}
