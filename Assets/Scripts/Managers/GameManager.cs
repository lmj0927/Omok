using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class GameManager : Singleton<GameManager>
{
    public MatchController matchController;
    public PlayerDataController playerDataController;

    [Header("Camera")]
    [SerializeField] public CameraMover cameraMover;    
    [SerializeField] List<Vector3Int> cameraPreset = new List<Vector3Int>();
    int _currentCameraPresetIndex = 0;

    [Header("Main UI")]
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

    public void ChangeView()
    {
        if(cameraPreset.Count == 0)
        {
            return;
        }

        int nextIndex = _currentCameraPresetIndex + 1;
        if(nextIndex >= cameraPreset.Count)
        {
            nextIndex = 0;
        }
        cameraMover.SetCamera(new Vector3(cameraPreset[nextIndex].y, cameraPreset[nextIndex].z, 0), cameraPreset[nextIndex].x);
        _currentCameraPresetIndex = nextIndex;
    }


    public new void OnDestroy()
    {
        StoneSpawner?.StopSpawn();
        matchController?.Dispose();
        base.OnDestroy();
    }
}
