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
using System.Threading.Tasks;
using ViewModel;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        // What the roullete game is currently
        // What the preferences of the user and the pass rounds
        // Load and unload roulletes
        // Keep track of the game state
        // Generate other persitente systems.
        
        public static GameManager Instance; // A static reference to the GameManager instance
        public CharacterTable characterTable;

        private protected string URL_PATH;
        public GameObject[] SystemPrefabs;
        
        private protected List<GameObject> _instanceSystemPrefabs;
        private protected List<AsyncOperation> _loadOperations;
        private GameState _currentGameState = GameState.PREGAME;
        private string _currentLevelName = string.Empty;
    
        public String UrlDataPath
        {
            get{ return URL_PATH;}
        }

        void Awake()
        {
            if(Instance == null) // If there is no instance already
            {
                DontDestroyOnLoad(gameObject); // Keep the GameObject, this component is attached to, across different scenes
                Instance = this;
            } else if(Instance != this) // If there is already an instance and it's not `this` instance
            {
                Destroy(gameObject); // Destroy the GameObject, this component is attached to
            }
        }

        async void Start() 
        {
            // Persistance instance
            URL_PATH = Application.persistentDataPath + "/Saves/";

            // Start game persistence
            var tasks = new Task[2];

            await StartRouletteInstance();
            await StartRouletteGame();
        
        }

        public async Task StartRouletteInstance()
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

            await Task.Yield();
        }

        private async Task StartRouletteGame()
        {
            // Initialize save directory
            CheckDirectory();

            await CreateNewPlayer();

            // Initialize game
            StartRound();
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
        private async Task CreateNewPlayer() 
        {
            string playerPath = URL_PATH+"player";
            await Player.CreatePlayer(characterTable, "MatiV154", playerPath);
        }
        
        private void StartRound()
        {
           // Initialize round components
            ToggleGame();
        }

        // States controller
        private async void UpdateState(GameState state)
        {
            GameState previousGameState = _currentGameState;
            _currentGameState = state;

            switch (_currentGameState)
            {
                case GameState.PAUSED:
                    Time.timeScale = 0.0f;
                    break;
                case GameState.RUNNING:
                    await OnGameOpened();
                    characterTable.OnLoadGame
                        .OnNext(true);
                    Time.timeScale = 1.0f;
                    break;
                case GameState.REWARD:
                    OnGameClosed();
                    Time.timeScale = 1.0f;
                    break;
            }
            Debug.Log($"[GameManager] Current state game is now {_currentGameState.ToString()}");
        }

        // Player event
        public void TogglePauseGame()
        {
            UpdateState(GameState.PAUSED);
        }
        public void ToggleRewardSystem()
        {
            UpdateState(GameState.REWARD);
        }
        public void ToggleGame()
        {
            UpdateState(GameState.RUNNING);
        }

        // Unity event
        public void OnGameClosed() 
        {
            Debug.Log("Game have been closed! Files was saved!");  

            characterTable.OnSaveGame
                .OnNext(true);

            characterTable.OnResetGame
                .OnNext(true);

            characterTable.currentNumbers.Clear();
            characterTable.currentTableInGame.Clear();
        }
        public async Task OnGameOpened() 
        { 
            await Task.Delay(TimeSpan.FromSeconds(1));

            // Update round parameters
            characterTable.currentTableActive.Value = false; 
            characterTable.currentTableCount = 0;
            characterTable.currentTable.Clear();
            characterTable.currentNumbers.Clear();
            characterTable.currentTableInGame.Clear();
            characterTable.lastNumber = 0;
            characterTable.lastTable.Clear();
            
            await Task.Delay(TimeSpan.FromSeconds(2));

            characterTable.currentTableActive.Value = true; 
            characterTable.currentChipSelected = characterTable.chipData.Where(chip => chip.chipkey == KeyFicha.Chip10).First();

            await Task.Yield();
        }
        protected void OnApplicationPause()
        {
            if(Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
                OnGameClosed();
        }
        protected void OnApplicationQuit()
        {
            if(Application.platform == RuntimePlatform.WindowsEditor)
                OnGameClosed();
        }
        protected void OnDestroy() 
        {
            if(Instance == this)
            {
                Instance = null;
            }

            if(_instanceSystemPrefabs == null)
                return;
            
            for(int i = 0; i < _instanceSystemPrefabs.Count; i++)
            {
                Destroy(_instanceSystemPrefabs[i]);
            }
            _instanceSystemPrefabs.Clear();
        }

        // Loaders
        public void LoadScene(string levelName)
        {
            SceneManager.LoadScene(levelName);
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
