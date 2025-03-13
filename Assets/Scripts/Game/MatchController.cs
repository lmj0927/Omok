using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
    CancellationTokenSource _matchMakingCts;
    bool _isCancelMatch = false;

    //For Game
    MatchInfo _matchInfo;
    MATCH_STATE _matchState = MATCH_STATE.End;
    float _turnTime = 30f;
    
    private Cell currentCell;

    public Action<TurnData, CELL_TYPE> OnDrawCell;
    public Action<TurnData, MATCH_STATE> TurnEnded;
    
    public void SetCurrentCell(Cell cell)
    {
        if(!IsMyTurn() && _matchPlayType == PLAY_TYPE.Multi) return;

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
    }

    public void StartMatchMaking(){
        switch(_matchPlayType){
            case PLAY_TYPE.Multi:
                //Show MatchMaking Loading UI
                UIManager.Instance.GetUI<MatchMakingController>(UI_TYPE.MatchMaking).Show();
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
        _matchMakingCts?.Cancel();
        _isMatched = false;
        _isCancelMatch = true;
        Dispose();
    }

    void StartMatch()
    {
        _matchMakingCts?.Cancel();
        _isMatched = true;
        _isCancelMatch = false;
        _matchState = MATCH_STATE.BlackTurn;

        UIManager.Instance.GetUI<MatchMakingController>(UI_TYPE.MatchMaking).Hide();
        UIManager.Instance.GetUI<GameBoardUIController>(UI_TYPE.Game).Show();
    }
    
    async UniTask FindMatching()
    {
        if(_matchMakingCts != null){
            _matchMakingCts.Cancel();
        }
        _matchMakingCts = new CancellationTokenSource();
        await UniTask.Delay(_matchTimeSecond * 1000, cancellationToken: _matchMakingCts.Token);
        
        if(_isMatched || _isCancelMatch){
            Debug.Log("## Find or Cancel Matching");
            return;
        }
        Debug.Log("## Timeout Matching");
        
        Initailize(PLAY_TYPE.AI);
        StartMatchMaking();
    }

    void InitializeAIController()
    {
        AIController aiController = new AIController();
        aiController.Initailize();

        _gameTypeController = aiController;

        //AI정보 1,2,3
        int tier = GameManager.Instance.playerDataController.UserInfo.tier;
        //Mathf

        //10~18 = 1
        //5 ~ 9 = 2
        //1 ~ 4 = 3
        string aiId = "ai1";
        if(tier >= 10 && tier <= 18)
        {
            //aiController.SetAILevel(1);
            aiId = "ai1";
            
        }
        else if(tier >= 5 && tier <= 9)
        {
            //aiController.SetAILevel(2);
            aiId = "ai2";
        }
        else if(tier >= 1 && tier <= 4)
        {
            //aiController.SetAILevel(3);
            aiId = "ai3";
        }
        
        NetworkManage.Instance.LoadUserInfo(aiId, 
            (userInfo) =>
            {
                _matchInfo.opponent = userInfo;
                Debug.Log("AI 정보를 불러오는데 성공했습니다.");
                
                StartMatch();
                
            },
            () =>
            {
                Debug.Log("AI 정보를 불러오는데 실패했습니다.");
                //가짜 정보
                _matchInfo.opponent = new UserInfo()
                {
                    userId = "FakeAI2025",
                    nickname = "AI",
                    tier = 10,
                    winCount = 0,
                    loseCount = 0,
                    score = 0,
                    profileIndex = 0,
                };

                StartMatch();
            }
        );
    }

    void InitializeReplayController(int replayIndex)
    {
        ReplayController replayController = new ReplayController();
        replayController.Initailize(replayIndex);
        _matchInfo = replayController.GetMatchInfo();

        _gameTypeController = replayController;
    }

    void InitializeMultiController()
    {
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
                        StartMatch();
                        Debug.Log("## Start Game: " + _matchInfo.opponent.userId);
                    }
                    catch(Exception err)
                    {
                        Debug.Log("## Start Game Error: " + err.Message);
                    }
                    break;
                case MultiplayManagerState.EndGame:
                    try{
                        if (data is bool isBlackWin)
                        {
                            EndMatch(isBlackWin, true);
                        }
                        else
                        {
                            Debug.LogError("데이터가 bool 형식이 아닙니다.");
                        }
                        Debug.Log("## End Game");
                    }
                    catch(Exception err)
                    {
                        Debug.Log("## End Game Error: " + err.Message);
                    }
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
            if(_gameTypeController is MultiplayController multiplayController)
            {
                multiplayController.SendEndTurn(currentCell.row, currentCell.col);
            }

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

        TurnEnded?.Invoke(turnData, _matchState);

        if(_matchState == MATCH_STATE.BlackTurn){
            _matchState = MATCH_STATE.WhiteTurn;
        }
        else{
            _matchState = MATCH_STATE.BlackTurn;
        }
    }

    public MATCH_STATE GetMatchState(){
        return _matchState;
    }

    public bool IsMyTurn(){
        return _matchInfo.isBlack == (_matchState == MATCH_STATE.BlackTurn);
    }

    public bool IsClientBlack(){
        return _matchInfo.isBlack;
    }
    
    public void Dispose()
    {
        _gameTypeController?.Dispose();
        _gameTypeController = null;
        _matchState = MATCH_STATE.End;
    }

    public void Surrender()
    {
        if(_gameTypeController is MultiplayController multiplayController)
        {
            multiplayController.EndGame(!IsClientBlack());
        }
        
        EndMatch(!IsClientBlack(), true);
    }

    public void EndMatch(bool isBlackWin, bool isSurrender)
    {
        _matchState = MATCH_STATE.End;

        PlayerDataController myPlayerDataController = GameManager.Instance.playerDataController;
        PlayerDataController opponentPlayerDataController = new PlayerDataController(_matchInfo.opponent);
        
        if(isBlackWin == IsClientBlack())
        {
            myPlayerDataController.Win();
            opponentPlayerDataController.Lose();
        }
        else
        {
            if(_matchPlayType == PLAY_TYPE.AI)
            {
                myPlayerDataController.Lose();
                opponentPlayerDataController.Win();
            }
        }

        UIManager.Instance.GetUI<GameBoardUIController>(UI_TYPE.Game).Hide();
        Dispose();
    }
}
