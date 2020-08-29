using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CanvasUI : Singlenton<CanvasUI>
{
    [SerializeField] Image shadowRoullete = null;
    [SerializeField] WinOrLostUI _winOrLostSc = null;

    private void Start()
    {
        game_manager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
    }

    void HandleGameStateChanged(game_manager.GameState curentState, game_manager.GameState previous)
    {
        shadowRoullete.gameObject.SetActive(curentState == game_manager.GameState.PAUSED || curentState == game_manager.GameState.SHOP);
    }

    public void turnOffUI(bool active)
    {
        this.gameObject.SetActive(active);
    }

    public void turnWinOrLost(string win, string number, bool isWin, int payment)
    {
        _winOrLostSc.winOrLost(win, number, isWin);
    }
    
}
