using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotationCell : MonoBehaviour, IReloadableCell<MatchInfo>
{
    [Header("UI Elements")]
    [SerializeField] Button _button;
    [SerializeField] Image turnImage;
    [SerializeField] TMP_Text opponentName;
    [SerializeField] TMP_Text gameResultText;
    
    [Header("Data")]
    [SerializeField] List<Sprite> turnImages = new List<Sprite>();

    private int matchIndex = -1;
    
    private void Start()
    {
        _button.onClick.AddListener(OnClickNotation);
    }

    private void OnClickNotation()
    {
        UIManager.Instance.HideUI<MainMenuController>(UI_TYPE.MainMenu);
        UIManager.Instance.HideUI<NotationUIController>(UI_TYPE.Notation);
        
        GameManager.Instance.StartGame(Constants.PLAY_TYPE.Replay);
        GameManager.Instance.matchController.InitializeReplayController(matchIndex);
    }

    public void SetData(MatchInfo data, int index)
    {
        matchIndex = index;
        SetTurn(data.isBlack);
        SetOpponentName(data.opponent.nickname);
        SetGameResult(data.isWin);
    }

    private void SetTurn(bool isBlack)
    {
        turnImage.sprite = isBlack ? turnImages[0] : turnImages[1];
    }

    private void SetOpponentName(string name)
    {
        opponentName.text = name;
    }
    
    private void SetGameResult(bool isWin)
    {
        gameResultText.text = isWin ? "승" : "패";
    }
}
