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
    public float TurnTime
    {
        get => _turnTime;
        set
        {
            //타이머 0 아래로 내려가지 않게.
            if (value < 0)_turnTime = 0;
            else _turnTime = value;
        }
    }

    private Cell currentCell;
    private bool isFirst = true;
    
    public Action<TurnData, CELL_TYPE> OnDrawCell;
    public Action<TurnData, MATCH_STATE> TurnEnded;
    public Action OnTurnEndUI;
    public Action<END_TYPE, Action> OnGameEndUI;
    public Action OnEndGridOmok;
    public Action OnSurrender;
    public List<Cell> FiveCells = new();
    public List<Transform> EndStones = new();

    public void SetCurrentCell(Cell cell)
    {
        if(!IsMyTurn() && (_matchPlayType == PLAY_TYPE.Multi || _matchPlayType == PLAY_TYPE.AI)) return;

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
                _ = UIManager.Instance.ShowUI<MatchMakingController>(UI_TYPE.MatchMaking);
                //Multiplay Initialize
                InitializeMultiController();
                //waiting thread
                FindMatching().Forget();
                break;
            case PLAY_TYPE.AI:
                //Show GameBoard UI
                //UIManager.Instance.GetUI<GameBoardUIController>(UI_TYPE.Game).Show();
                //AI Initialize
                InitializeAIController();
                break;
            case PLAY_TYPE.Replay:
                //Show Replay UI
                // UIManager.Instance.ShowUI<ReplayGameBoardUIController>(UI_TYPE.Replay);
                break;
        }
    }

    public void CloseMatchMaking()
    {
        if(_isMatched || _isCancelMatch) return;
        _isMatched = false;
        _isCancelMatch = true;
        _matchMakingCts?.Cancel();
        Dispose();
    }

    void StartMatch()
    {
        _isMatched = true;
        _isCancelMatch = false;
        _matchMakingCts?.Cancel();

        UIManager.Instance.HideUI<MatchMakingController>(UI_TYPE.MatchMaking);
        UIManager.Instance.HideUI<MainMenuController>(UI_TYPE.MainMenu);


        _matchState = MATCH_STATE.BlackTurn;
        var gameBoardUIController = UIManager.Instance.GetUI<GameBoardUIController>(UI_TYPE.Game);
        gameBoardUIController.Initialize();
        gameBoardUIController.Show();
        gameBoardUIController.StartMatch();
    }

    //StartMatch Delay
    async UniTask StartMatchDelayed(){
        _isMatched = true;
        _isCancelMatch = false;
        _matchState = MATCH_STATE.BlackTurn;
        _matchMakingCts?.Cancel();
        
        UIManager.Instance.HideUI<MatchMakingController>(UI_TYPE.MatchMaking);
        UIManager.Instance.HideUI<MainMenuController>(UI_TYPE.MainMenu);

        CursorManager.Instance.SetHandCursorTexture();
        
        var gameBoardUIController = UIManager.Instance.GetUI<GameBoardUIController>(UI_TYPE.Game);
        gameBoardUIController.Initialize();
        gameBoardUIController.Show().Forget();
        
        await UniTask.Delay(200);
        GameManager.Instance.cameraMover.SetCamera(new Vector3(cameraMoverMatchStartRotateX, cameraMoverMatchStartRotateY, 0), cameraMoverMatchStartDistance);
        await UniTask.Delay(500);
        
        
        
        gameBoardUIController.StartMatch();
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
            // easy
            aiId = "ai1";
            aiController.SetAILevel(aiId);
        }
        else if(tier >= 5 && tier <= 9)
        {
            // mid
            aiId = "ai2";
            aiController.SetAILevel(aiId);
        }
        else if(tier >= 1 && tier <= 4)
        {
            // hard
            aiId = "ai3";
            aiController.SetAILevel(aiId);
        }
        
        NetworkManage.Instance.LoadUserInfo(aiId, 
            (userInfo) =>
            {
                _matchInfo.opponent = userInfo;
                Debug.Log("AI 정보를 불러오는데 성공했습니다.");

                _ = StartMatchDelayed();
                
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

                _ = StartMatchDelayed();
            }
        );
    }

    public void InitializeReplayController(int replayIndex)
    {
        ReplayController replayController = new ReplayController();
        replayController.Initialize(replayIndex);
        _matchInfo = replayController.GetMatchInfo();

        _gameTypeController = replayController;
    }

    void InitializeMultiController()
    {
        _matchInfo = new MatchInfo(){
            turn = new List<TurnData>(),
        };

        MultiplayController multiplayController = new MultiplayController();
        multiplayController.InitailizeMultiplay((state, data) =>
        {
            switch (state)
            {
                case MultiplayManagerState.CreateRoom:
                    _matchInfo.isBlack = true;
                    Debug.Log("## Create Room");
                    break;
                case MultiplayManagerState.JoinRoom:
                    _matchInfo.isBlack = false;
                    isFirst = false;
                    _isMatched = true;
                    Debug.Log("## Join Room");
                    break;
                case MultiplayManagerState.StartGame:
                    try{
                        if (data is UserInfo user)
                        {
                            UserInfo opponent = user;
                            _matchInfo.opponent = opponent;
                        }
                        else
                        {
                            Debug.LogError("데이터가 UserInfo 형식이 아닙니다.");
                        }
                        _ = StartMatchDelayed();
                        Debug.Log("## Start Game: " + _matchInfo.opponent.userId);
                    }
                    catch(Exception err)
                    {
                        Debug.Log("## Start Game Error: " + err.Message);
                    }
                    break;
                case MultiplayManagerState.EndGame:
                    Time.timeScale = 1;
                    try{
                        if (data is bool isBlackWin)
                        {
                            EndMatch(isBlackWin ? END_TYPE.BlackWin : END_TYPE.WhiteWin, true);
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
                case MultiplayManagerState.DrawAnswer:
                    //신청한 쪽의 timescale
                    Time.timeScale = 1;
                    if (data is bool isAccept)
                    {
                        if(isAccept)
                        {
                            EndMatch(END_TYPE.Draw, false);
                        }
                        else
                        {
                            UIManager.Instance.GetUI<AlarmPanelController>(UI_TYPE.Alarm).Show("상대방이 무승부를 거절했습니다.", () => {});
                        }                        
                    }
                    else
                    {
                        Debug.LogError("데이터가 bool 형식이 아닙니다.");
                    }
                    break;                
                case MultiplayManagerState.Draw:
                    if (data is bool isQuestion)
                    {
                        if(isQuestion)
                        {
                            //받는 쪽의 timescale 조절
                            Time.timeScale = 0;
                            UIManager.Instance.GetUI<ConfirmPanelController>(UI_TYPE.Confirm).Show("상대방이 무승부를 요청했습니다. 수락하시겠습니까?", () =>
                            {
                                multiplayController.SendAnswerDraw(true);
                                EndMatch(END_TYPE.Draw, false);
                                Time.timeScale = 1;
                            }, () =>
                            {
                                multiplayController.SendAnswerDraw(false);
                                Time.timeScale = 1;
                            });

                        }
                        else
                        {
                            EndMatch(END_TYPE.Draw, false);
                        }
                    }
                    else
                    {
                        Debug.LogError("데이터가 bool 형식이 아닙니다.");
                    }

                    break;
            }
        });
        multiplayController.Initailize();

        _gameTypeController = multiplayController;
    }

    public void SetTurn()
    {
        if (isFirst)
        {
            if (currentCell.row != 7 || currentCell.col != 7)
            {
                UIManager.Instance.GetUI<AlarmPanelController>(UI_TYPE.Alarm).Show("첫 수는 가운데에 놓아야 합니다!", () => {});
                return;
            }
            
            isFirst = false;
        }
        
        if (!currentCell.IsUnityNull())
        {
            if (_gameTypeController is MultiplayController multiplayController)
            {
                multiplayController.SendEndTurn(currentCell.row, currentCell.col);
            }
            
            SetTurn(currentCell.row, currentCell.col);
            currentCell = null;
        }
    }
    
    public void SetTurn(int row, int col)
    {
        TurnData turnData = new TurnData(){
            row = row,
            col = col,
        };

        _matchInfo.turn.Add(turnData);

        TurnEnded?.Invoke(turnData, _matchState);

        if(_matchState == MATCH_STATE.BlackTurn){
            _matchState = MATCH_STATE.WhiteTurn;
        }
        else if(_matchState == MATCH_STATE.WhiteTurn){
            _matchState = MATCH_STATE.BlackTurn;
        }

        if (_matchState == MATCH_STATE.WhiteTurn && _gameTypeController is AIController aiController)
        {
            OperateCommand command = new OperateCommand();
            command.turnData = turnData;
            
            aiController.Operate(command);
        }
        OnTurnEndUI?.Invoke();
    }

    public void Operate(OperateCommand command)
    {
        _gameTypeController.Operate(command);
    }

    public MATCH_STATE GetMatchState(){
        return _matchState;
    }
    
    public MatchInfo GetCurrentMatchInfo()
    {
        return _matchInfo;
    }

    public PLAY_TYPE GetCurrentPlayType()
    {
        return _matchPlayType;
    }

    public bool IsMyTurn(){
        return _matchInfo.isBlack == (_matchState == MATCH_STATE.BlackTurn);
    }

    public bool IsClientBlack(){
        return _matchInfo.isBlack;
    }
    
    public void Dispose()
    {
        Time.timeScale = 1;
        Debug.Log("## Dispose MatchController");
        _gameTypeController?.Dispose();
        _gameTypeController = null;
        _matchState = MATCH_STATE.End;
        isFirst = true;
    }

    public void RequestDraw()
    {
        if(_gameTypeController is MultiplayController multiplayController)
        {
            multiplayController.SendRequestDraw(true);
        }
    }

    public void Surrender()
    {
        if(_gameTypeController is MultiplayController multiplayController)
        {
            multiplayController.EndGame(!IsClientBlack());
        }
        
        EndMatch(IsClientBlack() ? END_TYPE.WhiteWin : END_TYPE.BlackWin, true);
    }

    public void DrawMatch()
    {
        if(_gameTypeController is MultiplayController multiplayController)
        {
            multiplayController.SendRequestDraw(true);
        }

        EndMatch(END_TYPE.Draw, false);
    }

    public void EndMatch(END_TYPE endType, bool isSurrender)
    {
        if(_matchState == MATCH_STATE.End) return;
        _matchState = MATCH_STATE.End;
        if(isSurrender)
        {
            OnSurrender?.Invoke();
        }

        PlayerDataController myPlayerDataController = GameManager.Instance.playerDataController;
        PlayerDataController opponentPlayerDataController = new PlayerDataController(_matchInfo.opponent);

        _matchInfo.gameResult = endType;

        bool isClientWin = IsClientBlack() && endType == END_TYPE.BlackWin || !IsClientBlack() && endType == END_TYPE.WhiteWin;

        switch (endType)
        {
            case END_TYPE.Draw:
                OnGameEndUI?.Invoke(END_TYPE.Draw, () =>
                {
                    UIManager.Instance.GetUI<AlarmPanelController>(UI_TYPE.Alarm).Show("무승부입니다!" ,() =>{});
                    UIManager.Instance.HideUI<GameBoardUIController>(UI_TYPE.Game);
                    _ = UIManager.Instance.ShowUI<MainMenuController>(UI_TYPE.MainMenu);
                });

                myPlayerDataController.Draw();
                if(_matchPlayType == PLAY_TYPE.AI)
                {
                    opponentPlayerDataController.Draw();
                }
                break;
            case END_TYPE.BlackWin:
                OnGameEndUI?.Invoke(END_TYPE.BlackWin, () =>
                {
                    UIManager.Instance.GetUI<ResultPanelController>(UI_TYPE.MatchResult).Show(isClientWin, myPlayerDataController.UserInfo);
                    UIManager.Instance.HideUI<GameBoardUIController>(UI_TYPE.Game);
                    _ = UIManager.Instance.ShowUI<MainMenuController>(UI_TYPE.MainMenu);
                });
                break;
            case END_TYPE.WhiteWin:
                OnGameEndUI?.Invoke(END_TYPE.WhiteWin, () =>
                {
                    UIManager.Instance.GetUI<ResultPanelController>(UI_TYPE.MatchResult).Show(isClientWin, myPlayerDataController.UserInfo);
                    UIManager.Instance.HideUI<GameBoardUIController>(UI_TYPE.Game);
                    _ = UIManager.Instance.ShowUI<MainMenuController>(UI_TYPE.MainMenu);
                });
            
                if(_matchPlayType == PLAY_TYPE.AI)
                {
                    myPlayerDataController.Lose();
                    opponentPlayerDataController.Win();
                }
                break;
        }

        if(isClientWin)
        {
            myPlayerDataController.Win();
            opponentPlayerDataController.Lose();
        }

        
        FiveCells.Clear();
        EndStones.Clear();
        
        CursorManager.Instance.SetBrushCursorTexture();

        MatchInfoUtil.AddMatchInfo(_matchInfo);
        
        Dispose();
    }
}
