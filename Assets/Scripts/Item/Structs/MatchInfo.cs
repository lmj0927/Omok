using System.Collections.Generic;

[System.Serializable]
public struct MatchInfo{
    public int index;
    public List<TurnData> turn;
    public UserInfo opponent;
    public bool isWin;
    public bool isBlack;
}