using System;
using UnityEngine;

public class CoinController
{
    public int Coin { get; private set; }
    public event Action<int> OnCoinChanged;

    public CoinController()
    {
        Coin = PlayerPrefs.GetInt(Constants.OwnCoin, 0);
    }
    
    public void AddCoin(int amount)
    {
        Coin += amount;
        SaveCoin();
        OnCoinChanged?.Invoke(Coin);
    }

    public void ConsumeCoin(int amount)
    {
        Coin -= amount;
        SaveCoin();
        OnCoinChanged?.Invoke(Coin);
    }

    private void SaveCoin()
    {
        PlayerPrefs.SetInt(Constants.OwnCoin, Coin);
    }
}