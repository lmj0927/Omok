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
    [SerializeField] TMP_Text turnCountText;
    
    [Header("Data")]
    [SerializeField] List<Sprite> turnImages = new List<Sprite>();

    private int matchIndex = -1;
    
    private void Start()
    {
        _button.onClick.AddListener(OnClickNotation);
    }

    private void OnClickNotation()
    {
        AudioManager.Instance.PlaySFX("buttonClick");
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
        SetGameResult(data.gameResult, data.isBlack);
        SetTurnCount(data.turn.Count);
    }

    private void SetTurn(bool isBlack)
    {
        turnImage.sprite = isBlack ? turnImages[0] : turnImages[1];
    }

    private void SetOpponentName(string name)
    {
        opponentName.text = name;
    }
    
    private void SetGameResult(Constants.END_TYPE gameResult, bool isBlack)
    {
        switch (gameResult)
        {
            case Constants.END_TYPE.BlackWin:
                gameResultText.text = isBlack ? "승" : "패";
                break;
            case Constants.END_TYPE.WhiteWin:
                gameResultText.text = isBlack ? "패" : "승";
                break;
            case Constants.END_TYPE.Draw:
                gameResultText.text = "무";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameResult), gameResult, null);
        }
    }

    private void SetTurnCount(int count)
    {
        turnCountText.text = $"{count.ToString()}수";
    }
}
