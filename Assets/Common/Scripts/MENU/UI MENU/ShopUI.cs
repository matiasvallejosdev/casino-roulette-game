using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public bool isMenu;
    public void onClick() 
    {
        game_manager.Instance.toggleShop();
        game_manager.Instance.setIsInMenu(isMenu);
    }
}
