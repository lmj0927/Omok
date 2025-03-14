using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class GameManager : Singleton<GameManager>
{
    public MatchController matchController;
    public PlayerDataController playerDataController;

    protected override void Awake()
    {
        base.Awake();
        var tempData = new UserInfo();
        tempData.nickname = "test";
        tempData.score = 2;
        tempData.tier = 10;
        tempData.userId = "test@test.com";
        tempData.loseCount = 5;
        tempData.winCount = 3;
        tempData.profileIndex = 2;
        
        SetUserInfo(tempData);
    }

    public void SetUserInfo(UserInfo userInfo)
    {
        playerDataController = new PlayerDataController(userInfo);
    }

    public UserInfo GetUserInfo()
    {
        return playerDataController.UserInfo;
    }

    public void StartGame(PLAY_TYPE playType){
        if(matchController == null){
            matchController = new MatchController();
        }
        matchController.Initailize(playType);

        matchController.StartMatchMaking();
    }

    public void GiveUpGame()
    {   
        matchController.Surrender();
    }

    public new void OnDestroy()
    {
        if(matchController.GetMatchState() != MATCH_STATE.End)
        {
            matchController.Surrender();
        }
        matchController.Dispose();
        base.OnDestroy();
    }
}
