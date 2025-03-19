using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public static class MatchInfoUtil
{
    // MatchInfo 리스트 저장
    public static void SaveMatchInfoToFile(List<MatchInfo> matchInfoList)
    {
        var jsonData = JsonUtility.ToJson(new MatchInfoWrapper { matches = matchInfoList });
        File.WriteAllText(Constants.MatchInfoFilePath, jsonData);
    }

    // MatchInfo 리스트 불러오기
    public static List<MatchInfo> LoadMatchInfoList()
    {
        var path = Path.Combine(Application.persistentDataPath, "matchData.json");
        if(!File.Exists(path))
        {
            File.Create(path).Close();
            return new List<MatchInfo>();
        }
        
        var jsonData = File.ReadAllText(path);
        return JsonUtility.FromJson<MatchInfoWrapper>(jsonData)?.matches ?? new List<MatchInfo>();
    }

    public static void AddMatchInfo(MatchInfo matchInfo)
    {
        List<MatchInfo> matchList = LoadMatchInfoList();
        matchList.Insert(0, matchInfo);
        SaveMatchInfoToFile(matchList);
    }

    public static void RemoveMatchInfo(int index)
    {
        List<MatchInfo> matchList = LoadMatchInfoList();
        matchList.RemoveAt(index);
        SaveMatchInfoToFile(matchList);
    }

    public static void ClearAllMatchInfo()
    {
        PlayerPrefs.DeleteKey(Constants.MATCH_INFO_KEY);
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