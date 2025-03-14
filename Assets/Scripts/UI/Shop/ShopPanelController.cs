using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ShopPanelController: MonoBehaviour,IGameUI
{
    [Header("UI Elements")]
    [SerializeField] private RectTransform content;
    [SerializeField] private GameObject itemCellPrefab;
    [SerializeField] private Button backButton;
    [SerializeField] private ReloadableScrollRect scrollRect;
    
    [Header("Data")]
    [SerializeField] private List<ProductData> products;

    private RectTransform _shopPanel;
    
    private void Awake()
    {
        _shopPanel = gameObject.GetComponent<RectTransform>();
        
        //Product 설정
        scrollRect.Reload(products, itemCellPrefab);
        backButton.onClick.AddListener(Hide);
        
        //TODO: Coin 설정
    }

    public void Show()
    {
        var original = _shopPanel.anchoredPosition;
        _shopPanel.anchoredPosition = new Vector2(Screen.width, original.y);

        gameObject.SetActive(true);
        _shopPanel.DOAnchorPosX(original.x, .3f).SetEase(Ease.InQuint);
    }

    public void Hide()
    {
        var original = _shopPanel.anchoredPosition;

        _shopPanel.DOAnchorPosX(Screen.width, .3f).SetEase(Ease.InQuint).OnComplete(() =>
        {
            gameObject.SetActive(false);
            _shopPanel.anchoredPosition = original;
        });
    }
}
