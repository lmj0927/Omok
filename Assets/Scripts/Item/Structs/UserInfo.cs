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
    public string userId { get; set; }
    public string nickname { get; set; }
    public int tier { get; set; }
    public int score { get; set; }
    public int profileIndex { get; set; }
    public int winCount { get; set; }
    public int loseCount { get; set; }
}