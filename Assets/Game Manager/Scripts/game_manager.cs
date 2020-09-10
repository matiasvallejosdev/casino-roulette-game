using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class game_manager : Singlenton<game_manager>
{
    // What the roullete game is currently in
    // What the preferences of the user and the pass rounds
    // Load and unload roulletes
    // Keep track of the game state
    // Generate other persitente systems. Example preferences users and store

    // Pregame, Runing Roullete, Paused
    public enum GameState
    {
        PREGAME,
        MENU,
        RUNING,
        PAUSED,
        SHOP
    }

    public GameObject[] SystemPrefabs;
    public Events.EventGameState OnGameStateChanged;
    public Events.EventRestartGame OnRestartGame;
    public EventsRound.EventRoundState OnGameRoundChange;

    List<GameObject> _instanceSystemPrefabs;
    List<AsyncOperation> _loadOperations;
    GameState _currentGameState = GameState.PREGAME;

    private string _currentLevelName = string.Empty;

    private bool isShopInMenu;
  
    public GameState CurrentGameState
    {
        get { return _currentGameState; }
        private set { _currentGameState = value; }
    }

    private void Start() 
    {
        initializeGame();

        DontDestroyOnLoad(gameObject);

        _instanceSystemPrefabs = new List<GameObject>();
        _loadOperations = new List<AsyncOperation>();

        InstantiateSystemPrefabs();

        UIManager.Instance.OnGameRestart.AddListener(HandleRestart);

        UIManager.Instance.OnMainMenuFadeComplete.AddListener(HandleMainMenuFadeComplete);
    }

    private void initializeGame()
    {
        isShopInMenu = true;
        string path = Application.persistentDataPath + "/roullete.data";
        Debug.Log(path);

        if (!File.Exists(path)) 
        {
            createNewPlayer();
        } else 
        {
            Debug.Log("El archivo ya existe!");
        }
    }

    private void createNewPlayer() 
    {
        int[] a = { 0, 0 };
        FichasSave[] r = { };
        
        SaveSystem.SavePlayer(a, r, true);

        PlayerPrefs.SetFloat("SecondsToWaitReward", 80);
        PlayerPrefs.SetFloat("SecondsToWaitRewardVideo", 120);
        Debug.Log("Generando archivos para iniciar por primera vez el juego.");
    }

    private void Update()
    {
        if (_currentGameState == GameState.PREGAME)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            togglePause();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            toggleShop();
        }
    }

    void HandleMainMenuFadeComplete(bool fadeOut)
    {
        if (!fadeOut)
        {
            unloadLevel(_currentLevelName);
        }
    }

    void HandleRestart(bool restart)
    {
        restart_game restarting = GameObject.Find("RestartGame").GetComponent<restart_game>();
        restarting.restartGame();
    }

    void InstantiateSystemPrefabs()
    {
        GameObject prefabsInstance;
        for(int i = 0; i < SystemPrefabs.Length;i++)
        {
            prefabsInstance = Instantiate(SystemPrefabs[i]);
            _instanceSystemPrefabs.Add(prefabsInstance);
        }
    }  

    protected override void OnDestroy() 
    {
        base.OnDestroy();
        for(int i = 0; i < _instanceSystemPrefabs.Count; i++)
        {
            Destroy(_instanceSystemPrefabs[i]);
        }
        _instanceSystemPrefabs.Clear();
    }

    private void UpdateState(GameState state)
    {
        GameState previousGameState = _currentGameState;
        _currentGameState = state;

        switch (_currentGameState)
        {
            case GameState.PREGAME:
                Time.timeScale = 1.0f;
                break;
            case GameState.MENU:
                Time.timeScale = 1.0f;
                break;
            case GameState.RUNING:
                Time.timeScale = 1.0f;
                break;
            case GameState.PAUSED:
                Time.timeScale = 0.0f;
                break;
            case GameState.SHOP:
                Time.timeScale = 1.0f;
                break;
            default:
    
                break;
        }
        OnGameStateChanged.Invoke(_currentGameState, previousGameState);
        // transitions between scenes
    }

    public void startGame()
    {
        loadLevel("0_Game_Reward");
    }
    
    public void togglePause()
    {
        // condition ? true : false
        UpdateState(_currentGameState == GameState.RUNING ? GameState.PAUSED : GameState.RUNING);
    }

    public void toggleShop()
    {
        UpdateState(_currentGameState == GameState.RUNING ? GameState.SHOP : GameState.RUNING);
    }

    public void restartGame()
    {
        OnRestartGame.Invoke(true);
        Debug.Log("Restarting game");
    }

    public void exitGame()
    {
        // Implement features for quitting and save de game
        Debug.Log("Quiting game");
        Application.Quit();
    }

    #region LoadScene
    // Functions
    public string getCurrentLevel() 
    {
        return _currentLevelName;
    }
    // Levels
    public void loadLevel(string levelName)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        if (ao == null)
        {
            Debug.Log("[GameManager] Unable to load level" + levelName);
            return;
        }
        ao.completed += OnLoadOperationComplete;
        _loadOperations.Add(ao);
        _currentLevelName = levelName;
    }
    public void unloadLevel(string levelName)
    {
        AsyncOperation ao = SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(levelName));
        if (ao == null)
        {
            Debug.Log("[GameManager] Unable to unload level" + levelName);
            return;
        }
        if(levelName == "2_Game_Roullete") 
        {
            RoundController.Instance.OnGameClosed();
        }
        ao.completed += OnUnloadOperationComplete;
    }
    // Operations before the load or unload
    void OnLoadOperationComplete(AsyncOperation ao)
    {
        if (_loadOperations.Contains(ao))
        {
            _loadOperations.Remove(ao);

            if (_loadOperations.Count == 0)
            {
                UpdateState(GameState.RUNING);
                Scene sc = SceneManager.GetSceneByName(_currentLevelName);
                SceneManager.SetActiveScene(sc);
            }
        }
        Debug.Log("Load complete");
    }
    void OnUnloadOperationComplete(AsyncOperation ao)
    {
        Debug.Log("Unload complete");
    }
    #endregion
    public bool getIsInMenu() 
    {
        Debug.Log("get in menu is: " + isShopInMenu);
        return isShopInMenu;
    }
    public void setIsInMenu(bool set) 
    {
        isShopInMenu = set;
    }
}
