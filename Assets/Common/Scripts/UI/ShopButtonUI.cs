using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopButtonUI : MonoBehaviour
{
    public bool isMenu;
    public void handleShopUI()
    {
        SoundContoller.Instance.fx_sound(4);
        game_manager.Instance.toggleShop();
        game_manager.Instance.setIsInMenu(isMenu);
        RoundController.Instance.activeButtons(false);
    }
}
