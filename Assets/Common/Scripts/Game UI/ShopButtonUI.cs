using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopButtonUI : MonoBehaviour
{
    public bool isMenu;
    public void handleShopUI()
    {
        SoundContoller.Instance.PlayFxSound(3);
        GameManager.Instance.toggleShop();
        GameManager.Instance.setIsInMenu(isMenu);
        RoundController.Instance.ActivateButtons(false);
    }
}
