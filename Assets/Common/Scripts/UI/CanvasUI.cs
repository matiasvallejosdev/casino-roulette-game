using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CanvasUI : Singlenton<CanvasUI>
{
    [SerializeField] Image shadowRoullete;
    [SerializeField] WinOrLostUI _winOrLostSc;

    private void Start()
    {
        game_manager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
    }

    void HandleGameStateChanged(game_manager.GameState curentState, game_manager.GameState previous)
    {
        RoundController.Instance.activeButtons(curentState == game_manager.GameState.PAUSED || curentState == game_manager.GameState.SHOP);
        shadowRoullete.gameObject.SetActive(curentState == game_manager.GameState.PAUSED || curentState == game_manager.GameState.SHOP);
    }

    public void turnOffUI(bool active)
    {
        this.gameObject.SetActive(active);
    }

    public void turnWinOrLost(string win, string number, bool isWin)
    {
        _winOrLostSc.winOrLost(win, number, isWin);
    }
    
}
