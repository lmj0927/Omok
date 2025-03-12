
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopProductCell: MonoBehaviour
{
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private GameObject advertisePanel;
    [SerializeField] private GameObject purchasePanel;
    
    public enum ProductType { None, Advertise, Purchase};
    
    private Button _productButton;
    private ProductType _productType;
    private int _reward;
    private int _price;

    private void Awake()
    {
        _reward = 0;
        _price = 0;
        
        _productButton = gameObject.GetComponent<Button>();
        _productButton.onClick.AddListener(OnClickProductButton);
    }

    private void OnClickProductButton()
    {
        //TODO: 코인 reward만큼 더하기
        Debug.Log($"{_price:N0}원 결제: {_reward:N0}개 보상");
    }

    public void SetProductType(ProductType productType)
    {
        _productType = productType;
        if (productType == ProductType.Advertise)
        {
            advertisePanel.SetActive(true);
            purchasePanel.SetActive(false);
        }
        else
        {
            advertisePanel.SetActive(false);
            purchasePanel.SetActive(true);
        }
    }
    
    public void SetReward(int reward)
    {
        _reward = reward;
        rewardText.text = $"코인 {reward:N0} 개";
    }
    
    public void SetPrice(int price)
    {
        _price = price;
        priceText.text = $"{price:N0} 원";
    }
}
