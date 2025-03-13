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

    public void SetLooseCount(int count)
    {
        var info = UserInfo;
        info.loseCount = count;
        UserInfo = info;
    }
}
