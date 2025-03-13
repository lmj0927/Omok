using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class GameManager : Singleton<GameManager>
{
    public MatchController matchController;
    public PlayerDataController playerDataController;

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

    public void EndGame(){
        matchController.EndMatch();
    }

    public void GiveUpGame(){
        playerDataController.SetLooseCount(playerDataController.UserInfo.loseCount + 1);
        SaveUserInfo(() =>{}, () =>{});
        
        matchController.EndMatch();
    }


    public void SaveUserInfo(Action success, Action fail){
        StartCoroutine(NetworkManage.Instance.SetUserInfo(playerDataController.UserInfo, success, fail));
    }
}
