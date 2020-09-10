using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeButtonUI : MonoBehaviour
{
    public void onClick() 
    {
        GameManager.Instance.unloadLevel("2_Game_Roullete");
        GameManager.Instance.loadLevel("1_Game_Menu");
    }
}
