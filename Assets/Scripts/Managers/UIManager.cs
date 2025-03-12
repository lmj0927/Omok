using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum UI_TYPE
{
    Confirm,
    Setting,
    Shop,
    Leaderboard,
    MatchMaking,
    MatchResult,
    Replay,
    SignIn,
    SignUp,
    MainMenu,
    Game,
}


public class UIManager : Singleton<UIManager>
{
    public SerializedDictionary<UI_TYPE, GameObject> _uiPrefabs = new SerializedDictionary<UI_TYPE, GameObject>();
    private Dictionary<UI_TYPE, Component> _activeUIs = new();

    private Canvas _mainCanvas;
    private Camera _mainCamera;

    private const string CanvasTag = "MainCanvas";
    
    public bool IsInitialized { get; private set; }

    private void Start()
    {

        if (GameObject.FindWithTag(CanvasTag) != null)
        {
            _mainCanvas = GameObject.FindWithTag(CanvasTag).GetComponent<Canvas>();
        }
        
        _mainCamera = Camera.main;
        SceneManager.sceneLoaded += OnSceneLoaded;
        

        //초기화
        GetUI<SigninPanelController>(UI_TYPE.SignIn); 
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (GameObject.FindWithTag(CanvasTag) != null)
        {
            _mainCanvas = GameObject.FindWithTag(CanvasTag).GetComponent<Canvas>();
        }

        IsInitialized = true;

        
    }

    public T ShowUI<T>(UI_TYPE type) where T : Component, IGameUI
    {
        var ui = GetUI<T>(type);
        ui.Show();

        return ui;
    }
    
    public void HideUI<T>(UI_TYPE type) where T : Component, IGameUI
    {
        var ui = GetUI<T>(type);
        ui.Show();
    }
    
    public T GetUI<T>(UI_TYPE type) where T : Component
    {
        if (_activeUIs.TryGetValue(type, out var ui))
        {
            if(ui.IsUnityNull()) return CreateUI<T>(type);

            return ui as T;
        }

        return CreateUI<T>(type);
    }

    private T CreateUI<T>(UI_TYPE type) where T : Component
    {
        var ui = _uiPrefabs[type];


        if (ui == null) return null;
        
        var instance = Instantiate(ui, _mainCanvas.transform).GetComponent<T>();
        return (_activeUIs[type] = instance) as T;
    }
}
