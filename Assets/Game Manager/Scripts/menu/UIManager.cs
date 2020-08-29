using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singlenton<UIManager>
{
    [SerializeField] private MainMenu _mainMenu = null;
    [SerializeField] private PauseMenu _pauseMenu = null;
    [SerializeField] private ShopMenu _shopMenu = null;

    [SerializeField] private Camera _dummyCamera = null;
    private bool isBootOn = true;

    public Events.EventRestartGame OnGameRestart;
    public Events.EventFadeComplete OnMainMenuFadeComplete;

    private void Start()
    {
        game_manager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);

        _mainMenu.OnMainMenuFadeComplete.AddListener(HandleMainMenuFadeComplete);

        _pauseMenu.OnRestartGame.AddListener(HandleRestartGame);
    }

    void HandleMainMenuFadeComplete(bool fadeOut)
    {
        OnMainMenuFadeComplete.Invoke(fadeOut);
    }

    void HandleRestartGame(bool restart)
    {
        OnGameRestart.Invoke(restart);
    }

    void HandleGameStateChanged(game_manager.GameState curentState, game_manager.GameState previous)
    {
        _mainMenu.gameObject.SetActive(curentState == game_manager.GameState.PREGAME);
        _pauseMenu.gameObject.SetActive(curentState == game_manager.GameState.PAUSED);
        _shopMenu.gameObject.SetActive(curentState == game_manager.GameState.SHOP);       
    }

    private void Update()
    {
        if(game_manager.Instance.CurrentGameState != game_manager.GameState.PREGAME)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isBootOn )
        {
            game_manager.Instance.startGame();
        }
        if(Input.touchCount > 0) 
        {
            game_manager.Instance.startGame();
            Touch touch = Input.GetTouch(0);
        }
    }

    public void SetDummyCameraActive(bool active)
    {
        isBootOn = active;
        _dummyCamera.gameObject.SetActive(active);
    }
}
