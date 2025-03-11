
using Newtonsoft.Json;


[System.Serializable]
public class UserInfo{
    [JsonProperty("userId")]
    public string userId;
    [JsonProperty("nickname")]
    public string nickname;
    [JsonProperty("tier")]
    public int tier;
    [JsonProperty("score")]
    public int score;
    [JsonProperty("profileIndex")]
    public int profileIndex;
    [JsonProperty("winCount")]
    public int winCount; 
    [JsonProperty("loseCount")]
    public int loseCount;
}