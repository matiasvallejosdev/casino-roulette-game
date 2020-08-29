using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button _exitBtn = null;
    [SerializeField] private Button _restartBtn = null;
    [SerializeField] private Button _resumeBtn = null;

    public Events.EventRestartGame OnRestartGame;

    private void Start()
    {
        _resumeBtn.onClick.AddListener(HandleResumeClick);
        _restartBtn.onClick.AddListener(HandleRestartClick);
        _exitBtn.onClick.AddListener(HandleExitClick);
    }

    void HandleResumeClick()
    {
        game_manager.Instance.togglePause();
    }
    void HandleRestartClick()
    {
        OnRestartGame.Invoke(true);
        game_manager.Instance.restartGame();
    }
    void HandleExitClick()
    {
        game_manager.Instance.exitGame();
    }
}
