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
    public virtual void Show()
    {
        var original = panelRectTransform.anchoredPosition;
        panelRectTransform.anchoredPosition = new Vector2(Screen.width, original.y);

        gameObject.SetActive(true);
        panelRectTransform.DOAnchorPosX(original.x, .3f).SetEase(Ease.InQuint);
    }
    
    /// <summary>
    /// Panel 숨기기 함수
    /// </summary>
    public virtual void Hide()
    {
        var original = panelRectTransform.anchoredPosition;

        panelRectTransform.DOAnchorPosX(Screen.width, .3f).SetEase(Ease.InQuint).OnComplete(() =>
        {
            gameObject.SetActive(false);
            panelRectTransform.anchoredPosition = original;
        });
    }
}
