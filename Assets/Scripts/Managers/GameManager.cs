using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class GameManager : Singleton<GameManager>
{
    public MatchController matchController;


    public void StartGame(PLAY_TYPE playType){
        if(matchController == null){
            matchController = new MatchController();    
        }
        matchController.Initailize(playType);

        matchController.StartMatchMaking();
    }

    public void EndGame(){

    }

}
