public class Constants
{
    public const string ServerURL = "https://omok-server.onrender.com";
    public const string GameServerURL = "wss://omok-server.onrender.com";
    // public const string ServerURL = "http://localhost:3000";
    // public const string GameServerURL = "ws://localhost:3000";
    

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