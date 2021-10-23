using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using Components;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class GameManager : Singlenton<GameManager>
{
    // What the roullete game is currently in
    // What the preferences of the user and the pass rounds
    // Load and unload roulletes
    // Keep track of the game state
    // Generate other persitente systems. Example preferences users and store

    private protected string URL_PATH;
    public GameObject[] SystemPrefabs;

    //public Events.EventGameState OnGameStateChanged;
    //public Events.EventRestartGame OnRestartGame;
    //public EventsRound.EventRoundState OnGameRoundChange;
    
    private List<GameObject> _instanceSystemPrefabs;
    private List<AsyncOperation> _loadOperations;
    private GameState _currentGameState = GameState.PREGAME;
    private string _currentLevelName = string.Empty;
  
    public String CurrentLevelName
    {
        get { return _currentLevelName; }
        private set { _currentLevelName = value; }
    }
    public GameState CurrentGameState
    {
        get { return _currentGameState; }
        private set { _currentGameState = value; }
    }
    public String UrlDataPath
    {
        get{ return URL_PATH;}
    }

    private void Start() 
    {
        // Persistance instance
        URL_PATH = Application.persistentDataPath + "/Saves/";
        DontDestroyOnLoad(gameObject);

        // Start game manager
        StartInstances();
        StartRoulleteGame();
    }

    private void StartInstances()
    {
        _instanceSystemPrefabs = new List<GameObject>();
        _loadOperations = new List<AsyncOperation>();

        GameObject prefabsInstance;

        for(int i = 0; i < SystemPrefabs.Length;i++)
        {
            prefabsInstance = Instantiate(SystemPrefabs[i]);
            _instanceSystemPrefabs.Add(prefabsInstance);
        }
    }

    private void StartRoulleteGame()
    {
        CheckDirectory();
        CreateNewPlayer();
    }

    void CheckDirectory()
    {
        if(!Directory.Exists(URL_PATH))
        {
            Directory.CreateDirectory(URL_PATH);
        }
    }

    private void CreateNewPlayer() 
    {
        PlayerSystem.Instance.characterTable.OnSaveGame
            .OnNext(true);

        PlayerPrefs.SetString("LastRewardOpen", DateTime.Now.Ticks.ToString());
        PlayerPrefs.SetFloat("SecondsToWaitReward", 120);
    }

    // State controller
    private void UpdateState(GameState state)
    {
        GameState previousGameState = _currentGameState;
        _currentGameState = state;

        switch (_currentGameState)
        {
            case GameState.PREGAME:
                Time.timeScale = 1.0f;
                break;
            case GameState.RUNING:
                Time.timeScale = 1.0f;
                break;
            case GameState.PAUSED:
                Time.timeScale = 0.0f;
                break;
            case GameState.REWARD:
                Time.timeScale = 1.0f;
                break;
            default:
    
                break;
        }
        //OnGameStateChanged.Invoke(_currentGameState, previousGameState);
        // transitions between scenes
    }

    // Player event
    public void TogglePauseGame()
    {
        // condition ? true : false
        UpdateState(_currentGameState == GameState.RUNING ? GameState.PAUSED : GameState.RUNING);
    }
    public void ToggleRestartGame()
    {
        //OnRestartGame.Invoke(true);
        //restart_game restarting = GameObject.Find("RestartGame").GetComponent<restart_game>();
        //restarting.restartGame();
        Debug.Log("Restarting game");
    }
    public void ToggleExitGame()
    {
        // Implement features for quitting and save de game
        Debug.Log("Quiting game");
        Application.Quit();
    }

    // Unity event
    protected void OnApplicationQuit()
    {
        PlayerRound.Instance.OnGameClosed();
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

    // Loaders
    public void LoadLevel(string levelName)
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
    public void UnloadLevel(string levelName)
    {
        AsyncOperation ao = SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(levelName));
        if (ao == null)
        {
            Debug.Log("[GameManager] Unable to unload level" + levelName);
            return;
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
}

// Pregame, Runing, Paused, Reward
public enum GameState
{
    PREGAME,
    RUNING,
    PAUSED,
    REWARD
}
