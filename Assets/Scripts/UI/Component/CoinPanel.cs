using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CoinPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private float animationDuration = 0.5f;
    private int _currentCoin;
    
    private async void Start()
    {
        await UniTask.WaitUntil(() => GameManager.Instance.coinController != null);
        GameManager.Instance.coinController.OnCoinChanged += SetCoinText;
    }

    private void OnEnable()
    {
        SetCoinText(GameManager.Instance.coinController.Coin);
    }

    private void SetCoinText(int coin)
    {
        DOTween.To(() => _currentCoin, x => {
                _currentCoin = x;
                coinText.text = x.ToString();
            }, coin, animationDuration)
            .SetEase(Ease.OutQuad).OnComplete(() =>
            {
                _currentCoin = coin;
            });
    }
}
