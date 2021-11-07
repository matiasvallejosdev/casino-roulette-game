using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using Components;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Controllers;
using System.Linq;

namespace Managers
{
    public class GameManager : Singlenton<GameManager>
    {
        // What the roullete game is currently
        // What the preferences of the user and the pass rounds
        // Load and unload roulletes
        // Keep track of the game state
        // Generate other persitente systems.

        private protected string URL_PATH;
        public GameObject[] SystemPrefabs;
        
        private List<GameObject> _instanceSystemPrefabs;
        private List<AsyncOperation> _loadOperations;
        private GameState _currentGameState = GameState.PREGAME;
        private string _currentLevelName = string.Empty;
    
        public String UrlDataPath
        {
            get{ return URL_PATH;}
        }

        void Start() 
        {
            // Persistance instance
            URL_PATH = Application.persistentDataPath + "/Saves/";
            DontDestroyOnLoad(gameObject);

            // Start game manager
            StartRouletteInstance();
            StartRouletteGame();
        }

        private void StartRouletteInstance()
        {   
            // Create game instance undestroyable.
            _instanceSystemPrefabs = new List<GameObject>();
            _loadOperations = new List<AsyncOperation>();

            GameObject prefabsInstance;

            for(int i = 0; i < SystemPrefabs.Length;i++)
            {
                prefabsInstance = Instantiate(SystemPrefabs[i]);
                _instanceSystemPrefabs.Add(prefabsInstance);
            }
        }
        private void StartRouletteGame()
        {
            // Initialize game components
            CheckDirectory();
            CreateNewPlayer();
        }

        void CheckDirectory()
        {
            Debug.Log($"Directory: {URL_PATH}");

            // Check if the save directory exists
            if(!Directory.Exists(URL_PATH))
            {
                Directory.CreateDirectory(URL_PATH);
            }
        }
        private void CreateNewPlayer() 
        {
            PlayerPrefs.SetString("LastRewardOpen", DateTime.Now.Ticks.ToString());
            PlayerPrefs.SetFloat("SecondsToWaitReward", 120);
        }

        // States controller
        private void UpdateState(GameState state)
        {
            GameState previousGameState = _currentGameState;
            _currentGameState = state;

            switch (_currentGameState)
            {
                case GameState.PREGAME:
                    Time.timeScale = 1.0f;
                    break;
                case GameState.RUNNING:
                    Time.timeScale = 1.0f;
                    break;
                case GameState.PAUSED:
                    Time.timeScale = 0.0f;
                    break;
                case GameState.REWARD:
                    Time.timeScale = 1.0f;
                    break;
            }
        }

        // Player event
        public void TogglePauseGame()
        {
            UpdateState(_currentGameState == GameState.RUNNING ? GameState.PAUSED : GameState.RUNNING);
        }
        public void ToggleRewardSystem()
        {
            UpdateState(_currentGameState == GameState.RUNNING ? GameState.REWARD : GameState.RUNNING);
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
            AsyncOperation ao = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
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
        RUNNING,
        PAUSED,
        REWARD
    }
}
