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
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);

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

    void HandleGameStateChanged(GameManager.GameState curentState, GameManager.GameState previous)
    {
        _mainMenu.gameObject.SetActive(curentState == GameManager.GameState.PREGAME);
        _pauseMenu.gameObject.SetActive(curentState == GameManager.GameState.PAUSED);
        _shopMenu.gameObject.SetActive(curentState == GameManager.GameState.SHOP);       
    }

    private void Update()
    {
        if(GameManager.Instance.CurrentGameState != GameManager.GameState.PREGAME)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isBootOn )
        {
            GameManager.Instance.startGame();
        }
        if(Input.touchCount > 0) 
        {
            GameManager.Instance.startGame();
            Touch touch = Input.GetTouch(0);
        }
    }

    public void SetDummyCameraActive(bool active)
    {
        isBootOn = active;
        _dummyCamera.gameObject.SetActive(active);
    }
}
