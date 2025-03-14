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

    #endregion

    #region Game
    public const int AutoPlaceSpeed = 200;

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