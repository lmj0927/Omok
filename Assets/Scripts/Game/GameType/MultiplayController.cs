using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using SocketIOClient;
using UnityEngine;

public class MultiplayController : IBaseGameTypeController
{
    private SocketIOUnity _socket;
    private event Action<Constants.MultiplayManagerState, dynamic> _onMultiplayStateChanged;

    public void Initailize(){
        var sid = PlayerPrefs.GetString(Constants.SID);
        var uri = new Uri(Constants.GameServerURL);
        _socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket,
            Query = new Dictionary<string, string>
            {
                {"Cookie", sid}
            },
            
        });
        
        _socket.OnUnityThread("createRoomCli", CreateRoom);
        _socket.OnUnityThread("joinRoomCli", JoinRoom);
        _socket.OnUnityThread("startGameCli", StartGame);
        _socket.OnUnityThread("endGameCli", EndGame);
        _socket.OnUnityThread("endTurnCli", EndTurn);
        _socket.OnUnityThread("readyCompleteCli", ReadyComplete);
        _socket.OnUnityThread("drawGameCli", DrawGame);
        _socket.OnUnityThread("drawAnswerCli", DrawAnswer);
        
        _socket.Connect();
    }
    public MultiplayController()
    {

    }

    public MultiplayController(Action<Constants.MultiplayManagerState, dynamic> onMultiplayStateChanged)
    {
        _onMultiplayStateChanged = onMultiplayStateChanged;
    }

    public void InitailizeMultiplay(Action<Constants.MultiplayManagerState, dynamic> onMultiplayStateChanged)
    {
        _onMultiplayStateChanged = onMultiplayStateChanged;
    }

    private void CreateRoom(SocketIOResponse response)
    {
        _onMultiplayStateChanged?.Invoke(Constants.MultiplayManagerState.CreateRoom, null);
    }

    private void JoinRoom(SocketIOResponse response)
    {
        _onMultiplayStateChanged?.Invoke(Constants.MultiplayManagerState.JoinRoom, null);
    }

    private void LeaveRoom(SocketIOResponse response)
    {
        _onMultiplayStateChanged?.Invoke(Constants.MultiplayManagerState.EndMatch, null);
    }

    private void StartGame(SocketIOResponse response)
    {
        var data = response.GetValue<UserInfo>();
        _onMultiplayStateChanged?.Invoke(Constants.MultiplayManagerState.StartGame, data);
    }

    private void EndGame(SocketIOResponse response)
    {   
        var data = response.GetValue<bool>();
        _onMultiplayStateChanged?.Invoke(Constants.MultiplayManagerState.EndGame, data);
    }

    private void ReadyComplete(SocketIOResponse response)
    {
        SendReadyComplete();
        _onMultiplayStateChanged?.Invoke(Constants.MultiplayManagerState.ReadyComplete, null);
    }
  
    private void EndTurn(SocketIOResponse response)
    {
        var data = response.GetValue<TurnData>();   

        _onMultiplayStateChanged?.Invoke(Constants.MultiplayManagerState.EndTurn, data);
    }
    
    private void DrawGame(SocketIOResponse response)
    {
        var data = response.GetValue<bool>();
        _onMultiplayStateChanged?.Invoke(Constants.MultiplayManagerState.Draw, data);
    }

    private void DrawAnswer(SocketIOResponse response)
    {
        var data = response.GetValue<bool>();
        _onMultiplayStateChanged?.Invoke(Constants.MultiplayManagerState.DrawAnswer, data);
    }


    public void SendEndTurn(int row, int col){
        _socket.Emit("endTurn", new { row, col });
    }

    public void SendReadyComplete(){
        _socket.Emit("readyComplete");
    }

    public void LeaveRoom(){
        _socket.Emit("leaveRoom");
    }

    public void EndGame(bool isBlackWin){
        _socket.Emit("endGame", isBlackWin);
    }
    
    public void SendRequestDraw(bool isQuestion){
        _socket.Emit("drawGame", isQuestion);
    }

    public void SendAnswerDraw(bool isAccept){
        _socket.Emit("drawAnswer", isAccept);
    }

    public void Operate(OperateCommand command)
    {
        
    }

    public void Dispose()
    {
        Debug.Log("## MultiplayController Dispose");

        if (_socket != null)
        {
            LeaveRoom();
            _socket.Disconnect();
            _socket.Dispose();
            _socket = null;
        }
    }
}
