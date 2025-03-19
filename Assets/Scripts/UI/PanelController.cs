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
    
    /// <summary>
    /// Panel 표시 함수
    /// </summary>
    public virtual UniTask Show()
    {
        var original = panelRectTransform.anchoredPosition;
        panelRectTransform.anchoredPosition = new Vector2(Screen.width, original.y);

        var tcs = new UniTaskCompletionSource();
        
        gameObject.SetActive(true);
        panelRectTransform.DOAnchorPosX(original.x, .3f).SetEase(Ease.InQuint).OnComplete(() =>
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
