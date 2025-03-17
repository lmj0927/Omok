using System;
using UnityEngine;

public class CoinController
{
    public int Coin { get; private set; } = PlayerPrefs.GetInt(Constants.OwnCoin);
    public event Action OnCoinChanged;

    public void AddCoin(int amount)
    {
        Coin += amount;
        SaveCoin();
        OnCoinChanged?.Invoke();
    }

    public void ConsumeCoin(int amount)
    {
        Coin -= amount;
        SaveCoin();
        OnCoinChanged?.Invoke();
    }

    private void SaveCoin()
    {
        PlayerPrefs.SetInt(Constants.OwnCoin, Coin);
    }
}