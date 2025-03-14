using System;
using System.Collections.Generic;
using UnityEngine;

public class ReplayController : IBaseGameTypeController
{
    private MatchInfo _matchInfo;
    private int cursor;

    public void Initialize(int replayIndex)
    {
        cursor = 0;
        _matchInfo = GameManager.Instance.playerDataController.matchInfos[replayIndex];
        var replayGameBoardUIController = UIManager.Instance.ShowUI<ReplayGameBoardUIController>(UI_TYPE.Replay);

        var blackPlayer = _matchInfo.isBlack ? GameManager.Instance.playerDataController.UserInfo : _matchInfo.opponent;
        var whitePlayer = _matchInfo.isBlack ? _matchInfo.opponent : GameManager.Instance.playerDataController.UserInfo;

        replayGameBoardUIController.Initialize(blackPlayer, whitePlayer);
    }

    public MatchInfo GetMatchInfo()
    {
        return _matchInfo;
    }

    public void Operate(OperateCommand command)
    {
        if (command.operateType == OperateType.Draw)
        {
            if(cursor >= _matchInfo.turn.Count)
            {
                //TODO: 더이상 수가 없다고 알림 Popup
                Debug.Log("마지막 수입니다.");
                return;
            }
            
            GameManager.Instance.matchController.OnDrawCell?.Invoke(
                _matchInfo.turn[cursor], 
                cursor % 2 == 0 ? Constants.CELL_TYPE.Black : Constants.CELL_TYPE.White);
        
            cursor++;
        }
        else if (command.operateType == OperateType.Remove)
        {
            if (--cursor < 0)
            {
                cursor = 0;
                //TODO: 더이상 뒤로 못간다고 알림 Popup                
                Debug.Log("더 이상 뒤로 갈 수 없습니다.");
                return;
            }
            
            GameManager.Instance.matchController.OnDrawCell?.Invoke(_matchInfo.turn[cursor], Constants.CELL_TYPE.None);
        }
    }

    public void Dispose()
    {
    }

}