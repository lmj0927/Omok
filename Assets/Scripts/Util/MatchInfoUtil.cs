using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class MatchInfoUtil
{
    const string MATCH_INFO_KEY = "MatchInfoList";

    // MatchInfo 리스트 저장
    public static void SaveMatchInfoList(List<MatchInfo> matchInfoList)
    {
        string jsonData = JsonUtility.ToJson(new MatchInfoWrapper { matches = matchInfoList });
        PlayerPrefs.SetString(MATCH_INFO_KEY, jsonData);
        PlayerPrefs.Save();
    }

    // MatchInfo 리스트 불러오기
    public static List<MatchInfo> LoadMatchInfoList()
    {
        if (!PlayerPrefs.HasKey(MATCH_INFO_KEY))
            return new List<MatchInfo>();

        string jsonData = PlayerPrefs.GetString(MATCH_INFO_KEY);
        return JsonUtility.FromJson<MatchInfoWrapper>(jsonData)?.matches ?? new List<MatchInfo>();
    }

    public static void AddMatchInfo(MatchInfo matchInfo)
    {
        List<MatchInfo> matchList = LoadMatchInfoList();
        matchList.Add(matchInfo);
        SaveMatchInfoList(matchList);
    }

    public static void RemoveMatchInfo(int index)
    {
        List<MatchInfo> matchList = LoadMatchInfoList();
        matchList = matchList.Where(m => m.index != index).ToList();
        SaveMatchInfoList(matchList);
    }

    public static void ClearAllMatchInfo()
    {
        PlayerPrefs.DeleteKey(MATCH_INFO_KEY);
        PlayerPrefs.Save();
    }

    public static int GetMatchInfoCount()
    {
        return LoadMatchInfoList().Count;
    }

    [System.Serializable]
    class MatchInfoWrapper
    {
        public List<MatchInfo> matches;
    }
}