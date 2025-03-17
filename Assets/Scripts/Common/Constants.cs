using System.IO;
using UnityEngine;

public class Constants
{   
    
    #region Network

#if false 
    public const string ServerURL = "http://localhost:3000";
    public const string GameServerURL = "ws://localhost:3000";
#else
    public const string ServerURL = "https://omok-server.onrender.com";
    public const string GameServerURL = "wss://omok-server.onrender.com";
#endif
    public const string SID = "sid";
    #endregion
    
    #region PlayerPrefKey
    public const string BGMVolume = "BGMVolume";
    public const string SFXVolume = "SFXVolume";
    public const string BGMMute = "BGMMute";
    public const string SFXMute = "SFXMute";
    public const string MATCH_INFO_KEY = "MatchInfoList";
    public const string OwnCoin  = "OwnCoin";

    #endregion

    #region Game
    public const int AutoPlaceSpeed = 200;
    public const int CostPerGame = 100;

    #endregion

    #region Setting
    public static readonly string MatchInfoFilePath = Path.Combine(Application.persistentDataPath, "matchData.json");
    #endregion
    
    public enum MultiplayManagerState
    {
        CreateRoom,
        JoinRoom,
        StartGame,
        EndGame,
        EndTurn,
        EndMatch,
        ReadyComplete
    }

    public enum PLAY_TYPE{
        Multi,
        AI,
        Replay,
    }

    public enum MATCH_STATE{
        BlackTurn,
        WhiteTurn,
        End,
    }

    public enum CELL_TYPE{
        None,
        Warning,
        Black,
        White,
        PreviewBlack,
        PreviewWhite,
    }
}