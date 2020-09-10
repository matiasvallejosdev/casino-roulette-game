using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Button _exitBtn = null;
    [SerializeField] private UnityEngine.UI.Button _restartBtn = null;
    [SerializeField] private UnityEngine.UI.Button _resumeBtn = null;

    public Events.EventRestartGame OnRestartGame;

    private void Start()
    {
        _resumeBtn.onClick.AddListener(HandleResumeClick);
        _restartBtn.onClick.AddListener(HandleRestartClick);
        _exitBtn.onClick.AddListener(HandleExitClick);
    }

    void HandleResumeClick()
    {
        GameManager.Instance.togglePause();
    }
    void HandleRestartClick()
    {
        OnRestartGame.Invoke(true);
        GameManager.Instance.restartGame();
    }
    void HandleExitClick()
    {
        GameManager.Instance.exitGame();
    }
}
