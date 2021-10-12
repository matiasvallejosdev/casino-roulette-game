using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public bool isMenu;
    public void onClick() 
    {
        SoundContoller.Instance.PlayFxSound(3);
        //GameManager.Instance.toggleShop();
        //GameManager.Instance.setIsInMenu(isMenu);
    }
}
