using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static Constants;

public class MatchController : IDisposable
{
    PLAY_TYPE _matchPlayType;
    IBaseGameTypeController _gameTypeController; //Multi, AI, Replay

    //For MatchMaking
    int _matchTimeSecond = 3;
    bool _isMatched = false;
    bool _isMatchMaking = false;
    bool _isCancelMatch = false;

    //For Game
    MatchInfo _matchInfo;
    MATCH_STATE _matchState = MATCH_STATE.End;
    float _turnTime = 30f;
    
    private Cell currentCell;

    public Action<TurnData, CELL_TYPE> OnDrawCell;

    public void SetCurrentCell(Cell cell)
    {
        if(currentCell != null)
            OnDrawCell?.Invoke(new TurnData{ row = currentCell.row, col = currentCell.col }, CELL_TYPE.None);
        currentCell = cell;
        
        if (_matchState == MATCH_STATE.BlackTurn)
        {
            OnDrawCell?.Invoke(new TurnData{ row = currentCell.row, col = currentCell.col }, CELL_TYPE.PreviewBlack);
        }
        else if (_matchState == MATCH_STATE.WhiteTurn)
        {
            OnDrawCell?.Invoke(new TurnData{ row = currentCell.row, col = currentCell.col }, CELL_TYPE.PreviewWhite);
        }
    }
    
    
    public void Initailize(PLAY_TYPE matchPlayType){
        _gameTypeController?.Dispose();
        _gameTypeController = null;
        _matchState = MATCH_STATE.End;

        _matchPlayType = matchPlayType;
        _isMatched = false;
        _isCancelMatch = false;
        SetUIMode();
    }

    public void StartMatchMaking(){
        switch(_matchPlayType){
            case PLAY_TYPE.Multi:
                //Show MatchMaking Loading UI
                UIManager.Instance.GetUI<MatchMakingController>(UI_TYPE.MatchMaking);
                //Multiplay Initialize
                InitializeMultiController();
                //waiting thread
                FindMatching().Forget();
                break;
            case PLAY_TYPE.AI:
                //Show GameBoard UI
                UIManager.Instance.GetUI<GameBoardUIController>(UI_TYPE.Game).Show();
                //AI Initialize
                InitializeAIController();
                break;
            case PLAY_TYPE.Replay:
                //Show Replay UI
                UIManager.Instance.GetUI<ReplayGameBoardUIController>(UI_TYPE.Replay).Show();
                //Replay Initialize
                break;
        }
    }

    public void CloseMatchMaking()
    {
        _isMatchMaking = false;
        _isMatched = false;
        _isCancelMatch = true;
        Dispose();
    }

    void StartMatch()
    {
        _isMatchMaking = false;
        _isMatched = true;
        _isCancelMatch = false;
        _matchState = MATCH_STATE.BlackTurn;
        UIManager.Instance.GetUI<GameBoardUIController>(UI_TYPE.Game).Show();
    }

    void SetUIMode(){
        //TODO: Replay, Game
        if(_matchPlayType == PLAY_TYPE.Replay){
            
        }
        else{

        }
    }


    async UniTask FindMatching()
    {
        float time = 0;

        _isMatchMaking = true;
        while(_isMatchMaking){
            await UniTask.Delay(100);
            time += 0.1f;

            if(time >= _matchTimeSecond){
                _isMatchMaking = false;
            }
        }
        
        UIManager.Instance.GetUI<MatchMakingController>(UI_TYPE.MatchMaking).Hide();
        if(_isMatched || _isCancelMatch){
            Debug.Log("## Find or Cancel Matching");
            return;
        }
        Debug.Log("## Timeout Matching");
        
        Initailize(PLAY_TYPE.AI);
        StartMatchMaking();
    }

    void InitializeAIController(){


        AIController aiController = new AIController();
        aiController.Initailize();

        StartMatch();
        
        _gameTypeController = aiController;
    }

    void InitializeReplayController(int replayIndex){
        ReplayController replayController = new ReplayController();
        replayController.Initailize(replayIndex);
        _matchInfo = replayController.GetMatchInfo();

        _gameTypeController = replayController;
    }

    void InitializeMultiController(){
        _matchInfo = new MatchInfo(){
            turn = new List<TurnData>(),
        };

        MultiplayController multiplayController = new MultiplayController((state, data) =>
        {
            switch (state)
            {
                case MultiplayManagerState.CreateRoom:
                    _matchInfo.isBlack = true;
                    Debug.Log("## Create Room");
                    break;
                case MultiplayManagerState.JoinRoom:
                    _matchInfo.isBlack = false;
                    Debug.Log("## Join Room");
                    break;
                case MultiplayManagerState.StartGame:
                    try{
                        _isMatched = true;
                        if (data is UserInfo user)
                        {
                            UserInfo opponent = user;
                            _matchInfo.opponent = opponent;
                        }
                        else
                        {
                            Debug.LogError("데이터가 UserInfo 형식이 아닙니다.");
                        }
                        
                        Debug.Log("## Start Game: " + _matchInfo.opponent.nickname);
                    }
                    catch(Exception err)
                    {
                        Debug.Log("## Start Game Error: " + err.Message);
                    }

                    StartMatch();
                    Debug.Log("## Start Game");
                    break;
                case MultiplayManagerState.EndGame:
                    Dispose();
                    Debug.Log("## End Game");
                    break;
                case MultiplayManagerState.EndTurn:
                    try
                    {
                        if(data is TurnData turnData)
                        {
                            SetTurn(turnData.row, turnData.col);    
                        }
                        else
                        {
                            Debug.LogError("데이터가 TurnData 형식이 아닙니다.");
                        }
                        Debug.Log("## End Turn");
                    }
                    catch (Exception err)
                    {
                        Debug.Log("## End Turn Error: " + err.Message);
                    }
                    
                    break;
                case MultiplayManagerState.ReadyComplete:                    
                    break;
            }
        });
        multiplayController.Initailize();

        _gameTypeController = multiplayController;
    }


    public void SetTurn()
    {
        if (!currentCell.IsUnityNull())
        {
            SetTurn(currentCell.row, currentCell.col);
            currentCell = null;
        }
    }
    
    public void SetTurn(int row, int col){
        
        
        TurnData turnData = new TurnData(){
            row = row,
            col = col,
        };

        _matchInfo.turn.Add(turnData);

        if(_matchState == MATCH_STATE.BlackTurn){
            OnDrawCell?.Invoke(turnData, CELL_TYPE.Black);
            _matchState = MATCH_STATE.WhiteTurn;
        }
        else{
            OnDrawCell?.Invoke(turnData, CELL_TYPE.White);
            _matchState = MATCH_STATE.BlackTurn;
        }
    }

    public MATCH_STATE GetMatchState(){
        return _matchState;
    }
    
    public void Dispose()
    {
        _gameTypeController?.Dispose();
        _gameTypeController = null;
        _matchState = MATCH_STATE.End;
    }
}
