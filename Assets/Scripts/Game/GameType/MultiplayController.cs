using System;
using System.Collections;
using System.Collections.Generic;
using SocketIOClient;
using UnityEngine;

public class MultiplayController : IBaseGameTypeController
{
    private SocketIOUnity _socket;
    private event Action<Constants.MultiplayManagerState, dynamic> _onMultiplayStateChanged;

    public void Initailize(){
        //sid
        var sid = PlayerPrefs.GetString("sid");
        var uri = new Uri(Constants.GameServerURL);
        _socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket,
            Query = new Dictionary<string, string>
            {
                {"Cookie", sid}
            }
        });
        
        _socket.OnUnityThread("createRoom", CreateRoom);
        _socket.OnUnityThread("joinRoom", JoinRoom);
        _socket.OnUnityThread("startGame", StartGame);
        _socket.OnUnityThread("endGame", EndGame);
        _socket.OnUnityThread("endTurn", EndTurn);
        
        _socket.Connect();
    }
    
    public MultiplayController(Action<Constants.MultiplayManagerState, dynamic> onMultiplayStateChanged)
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

    private void StartGame(SocketIOResponse response)
    {
        var data = response.GetValue<UserInfo>();   

        _onMultiplayStateChanged?.Invoke(Constants.MultiplayManagerState.StartGame, data);
    }

    private void EndGame(SocketIOResponse response)
    {
        _onMultiplayStateChanged?.Invoke(Constants.MultiplayManagerState.EndGame, null);
    }
  
    private void EndTurn(SocketIOResponse response)
    {
        var data = response.GetValue<TurnData>();   

        _onMultiplayStateChanged?.Invoke(Constants.MultiplayManagerState.EndTurn, data);
    }

    public void SendTurnChange(int row, int col){
        _socket.Emit("turnChange", new { row, col });
    }

    public void Operate(){
        
    }

    public void Dispose()
    {
        if (_socket != null)
        {
            _socket.Disconnect();
            _socket.Dispose();
            _socket = null;
        }
    }
}
