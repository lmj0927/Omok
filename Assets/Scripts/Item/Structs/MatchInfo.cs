using System;
using System.Collections.Generic;
using Unity.VisualScripting;

[System.Serializable]
public struct MatchInfo{
    // public int index;
    public List<TurnData> turn;
    public UserInfo opponent;
    public Constants.END_TYPE gameResult;
    public bool isBlack;
}