using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ShopPanelController: PanelController
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
        backButton.onClick.AddListener(() =>
        {
            Hide();
        });
        
        //TODO: Coin 설정
    }
}
