using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ReplayController : IBaseGameTypeController
{
    private MatchInfo _matchInfo;
    private int cursor;
    private bool isOperating;

    public void Initialize(int replayIndex)
    {
        cursor = 0;
        isOperating = false;
        
        _matchInfo = MatchInfoUtil.LoadMatchInfoList()[replayIndex];
        var replayGameBoardUIController = UIManager.Instance.ShowUI<ReplayGameBoardUIController>(UI_TYPE.Replay);

        var blackPlayer = _matchInfo.isBlack ? GameManager.Instance.playerDataController.UserInfo : _matchInfo.opponent;
        var whitePlayer = _matchInfo.isBlack ? _matchInfo.opponent : GameManager.Instance.playerDataController.UserInfo;

        replayGameBoardUIController.Initialize(blackPlayer, whitePlayer);
        GameManager.Instance.cameraMover.SetCamera(new Vector3(65, 0, 0), 8);
    }

    public MatchInfo GetMatchInfo()
    {
        return _matchInfo;
    }

    public async void Operate(OperateCommand command)
    {
        if(isOperating) return;
        isOperating = true;
        
        if (command.operateType == OperateType.Draw)
        {
            if(cursor >= _matchInfo.turn.Count)
            {
                //TODO: 더이상 수가 없다고 알림 Popup
                Debug.Log("마지막 수입니다.");
                isOperating = false;
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
                isOperating = false;
                return;
            }
            
            GameManager.Instance.matchController.OnDrawCell?.Invoke(_matchInfo.turn[cursor], Constants.CELL_TYPE.None);
        }
        else if (command.operateType == OperateType.DrawAll)
        {
            while (cursor < _matchInfo.turn.Count)
            {
                GameManager.Instance.matchController.OnDrawCell?.Invoke(
                    _matchInfo.turn[cursor], 
                    cursor % 2 == 0 ? Constants.CELL_TYPE.Black : Constants.CELL_TYPE.White);
                
                cursor++;

                await UniTask.Delay(Constants.AutoPlaceSpeed);
            }
        }
        else if (command.operateType == OperateType.RemoveAll)
        {
            while (--cursor >= 0)
            {
                GameManager.Instance.matchController.OnDrawCell?.Invoke(_matchInfo.turn[cursor], Constants.CELL_TYPE.None);
                await UniTask.Delay(Constants.AutoPlaceSpeed);
            }
            
            cursor = 0;
        }

        isOperating = false;
    }

    public void Dispose()
    {
    }

}