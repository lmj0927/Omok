using System;
using UnityEngine;

public class PlayerDataController
{
    public Action<int> OnChangedProfile;
    public UserInfo UserInfo { get; private set; }

    public PlayerDataController(UserInfo userInfo)
    {
        UserInfo = userInfo;
    }

    public void SetProfile(int index)
    {
        //TODO: 프로필 변경 요청
        OnChangedProfile?.Invoke(index);
    }

    public void Win()
    {
        var info = UserInfo;
        info.winCount++;
        info.score++;
        UserInfo = info;

        Save();
    }

    public void Lose()
    {
        var info = UserInfo;
        info.loseCount++;
        info.score--;
        UserInfo = info;

        Save();
    }

    public void SetNickname(string nickname)
    {
        var info = UserInfo;
        info.nickname = nickname;
        UserInfo = info;
    }

    public void Save()
    {
        NetworkManage.Instance.SaveUserInfo(this.UserInfo,
            () =>{
                Debug.Log("Save Success");
            },
            () => {
                Debug.Log("Save Fail");
            });
    }
}
