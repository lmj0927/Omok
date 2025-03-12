using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanelController: MonoBehaviour,IGameUI
{
    [Header("UI Elements")]
    [SerializeField] private RectTransform shopPanel;
    [SerializeField] private RectTransform content;
    [SerializeField] private GameObject itemCellPrefab;
    [SerializeField] private Button backButton;
    
    [Header("Data")]
    [SerializeField] private List<ProductData> products;

    private void Awake()
    {
        shopPanel = gameObject.GetComponent<RectTransform>();
        
        //Product 설정
        foreach (var product in products)
        {
            var productCell = Instantiate(itemCellPrefab, content); 
            if(productCell.TryGetComponent<ShopProductCell>(out var cell))
            {
                cell.SetProductType(product.productType);
                cell.SetReward(product.reward);
                cell.SetPrice(product.price);
            }
        }
        
        backButton.onClick.AddListener(Hide);
        
        //TODO: Coin 설정
    }

    public void Show()
    {
        var original = shopPanel.anchoredPosition;
        shopPanel.anchoredPosition = new Vector2(Screen.width, original.y);

        gameObject.SetActive(true);
        shopPanel.DOAnchorPosX(original.x, .3f).SetEase(Ease.InQuint);
    }

    public void Hide()
    {
        var original = shopPanel.anchoredPosition;

        shopPanel.DOAnchorPosX(Screen.width, .3f).SetEase(Ease.InQuint).OnComplete(() =>
        {
            gameObject.SetActive(false);
            shopPanel.anchoredPosition = original;
        });
    }
}
