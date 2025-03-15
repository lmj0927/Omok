using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataController
{
    //TODO: matchInfos 설정
    public List<MatchInfo> matchInfos = new List<MatchInfo>();
    public Action<int> OnChangedProfile;
    public UserInfo UserInfo { get; private set; }

    public PlayerDataController(UserInfo userInfo)
    {
        UserInfo = userInfo;
    }

    public void SetProfile(int index)
    {
        var info = UserInfo;
        info.profileIndex = index;
        UserInfo = info;

        Save();

        OnChangedProfile?.Invoke(index);
    }

    public void Win()
    {
        NetworkManage.Instance.SendWinnerWrapper(UserInfo.userId,
            (userInfo) =>{
                UserInfo = userInfo;
                Debug.Log("SendWinner Success " + userInfo.nickname + " tier" + userInfo.tier + " score" + userInfo.score);
            },
            () => {
                Debug.Log("SendWinner Fail");
            });
    }

    public void Lose()
    {
        NetworkManage.Instance.SendLoserWrapper(UserInfo.userId,
            (userInfo) =>{
                UserInfo = userInfo;
                Debug.Log("SendLoser Success " + userInfo.nickname + " tier" + userInfo.tier + " score" + userInfo.score);
            },
            () => {
                Debug.Log("SendLoser Fail");
            });
    }

    public void SetNickname(string nickname)
    {
        var info = UserInfo;
        info.nickname = nickname;
        UserInfo = info;

        Save();
    }

    public void Save()
    {
        NetworkManage.Instance.SaveUserInfoWrapper(this.UserInfo,
            () =>{
                Debug.Log("Save Success");
            },
            () => {
                Debug.Log("Save Fail");
            });
    }
}
