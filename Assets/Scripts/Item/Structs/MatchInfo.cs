using System.Collections.Generic;
using Unity.VisualScripting;

[System.Serializable]
public struct MatchInfo{
    // public int index;
    public List<TurnData> turn;
    public UserInfo opponent;
    public bool isWin;
    public bool isBlack;
}