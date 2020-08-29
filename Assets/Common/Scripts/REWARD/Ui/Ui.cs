using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ui : Singlenton<Ui>
{
    public WinUI _winOrLostSc;

    public void turnWinOrLost(string win, string number, bool isWin, int payment)
    {
        _winOrLostSc.winOrLost(win, number, isWin);
        StartCoroutine(Wait());
    }
    IEnumerator Wait()
    {
        yield return new WaitForSecondsRealtime(6);
        if (game_manager.Instance.getIsInMenu())
        {
            closeScene("1_Game_Menu");
        }
        else
        {
            closeScene("2_Game_Roullete");
        }
    }
    public void closeScene(string sceneToOpen)
    {
        game_manager.Instance.unloadLevel("0_Game_Reward");
        game_manager.Instance.loadLevel(sceneToOpen);
    }
}
