using Newtonsoft.Json;

[System.Serializable]
public class TurnData{
    [JsonProperty("row")]
    public int row;
    [JsonProperty("col")]
    public int col;
}