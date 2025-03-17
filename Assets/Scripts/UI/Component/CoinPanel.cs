using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class CoinPanel : MonoBehaviour
{
    [SerializeField] TMP_Text coinText;

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
        coinText.text = coin.ToString();
    }
}
