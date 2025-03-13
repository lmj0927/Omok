using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinPanel : MonoBehaviour
{
    [SerializeField] TMP_Text coinText;

    public void SetCoinText(string text)
    {
        coinText.text = text;
    }
}
