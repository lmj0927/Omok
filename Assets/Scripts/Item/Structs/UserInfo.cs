// [System.Serializable]
// public class UserInfo{   
//     public string userId { get; set; }
//     public string nickname { get; set; }
//     public int tier { get; set; }
//     public int score { get; set; }
//     public int profileIndex { get; set; }
//     public int winCount { get; set; }
//     public int loseCount { get; set; }
// }

[System.Serializable]
public struct UserInfo{   
    public string userId { get=>UserId; set=>UserId=value; }
    public string nickname { get=>Nickname; set=>Nickname = value; }
    public int tier { get=>Tier; set=>Tier=value; }
    public int score { get=>Score; set=>Score=value; }
    public int profileIndex { get=>ProfileIndex; set=>ProfileIndex=value; }
    public int winCount { get=>WinCount; set=>WinCount=value; }
    public int loseCount { get=>LoseCount; set=>LoseCount=value; }

    public string UserId;
    public string Nickname;
    public int Tier;
    public int Score;
    public int ProfileIndex;
    public int WinCount;
    public int LoseCount;
}