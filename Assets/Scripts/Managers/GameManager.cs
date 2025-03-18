using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class GameManager : Singleton<GameManager>
{
    public MatchController matchController;
    public PlayerDataController playerDataController;
    [SerializeField] public CameraMover cameraMover;
    [SerializeField] public StoneSpawner StoneSpawner;
    
    public Action mainUIUpdate;
    
    public CoinController coinController;
    
    private void Start()
    {
        coinController = new CoinController();
    }

    public void SetUserInfo(UserInfo userInfo)
    {
        playerDataController = new PlayerDataController(userInfo);
    }

    public UserInfo GetUserInfo()
    {
        return playerDataController.UserInfo;
    }

    public void StartGame(PLAY_TYPE playType)
    {
        StoneSpawner.StopSpawn();

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
        if(matchController != null)
        {
            //본인이 도망갔을 경우 처리 방향이지만, 처리 불가
            // if(matchController.GetMatchState() != MATCH_STATE.End)
            // {
            //     matchController.Surrender();
            // }
            matchController.Dispose();
        }
        base.OnDestroy();
    }
}
