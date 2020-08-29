using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeButtonUI : MonoBehaviour
{
    public void onClick() 
    {
        game_manager.Instance.unloadLevel("2_Game_Roullete");
        game_manager.Instance.loadLevel("1_Game_Menu");
    }
}
