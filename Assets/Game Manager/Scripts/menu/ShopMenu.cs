using GameServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour
{
    [Header("Button UI")]
    [SerializeField] private Button _exitBtn = null;
    [SerializeField] private Button _resumeBtn = null;
    
    [Header("Purchase UI")]
    [SerializeField] private Button _C1 = null;
    [SerializeField] private Button _C2 = null;
    [SerializeField] private Button _C3 = null;
    [SerializeField] private Button _C4 = null;
    [SerializeField] private Button _C5 = null;

    private void Start()
    {
        _resumeBtn.onClick.AddListener(HandleResumeClick);
        _exitBtn.onClick.AddListener(HandleExitClick);

        _C1.onClick.AddListener(HandleC1Click);
        _C2.onClick.AddListener(HandleC2Click);
        _C3.onClick.AddListener(HandleC3Click);
        _C4.onClick.AddListener(HandleC4Click);
        _C5.onClick.AddListener(HandleC5Click);
    }
    void HandleC1Click() 
    {
        IAPManager.Instance.BuyC1();
    }
    void HandleC2Click()
    {
        IAPManager.Instance.BuyC2();
    }
    void HandleC3Click()
    {
        IAPManager.Instance.BuyC3();
    }
    void HandleC4Click()
    {
        IAPManager.Instance.BuyC4();
    }
    void HandleC5Click()
    {
        IAPManager.Instance.BuyC5();
    }
    void HandleResumeClick()
    {
        game_manager.Instance.toggleShop();
        if(!game_manager.Instance.getIsInMenu()) 
        {
            RoundController.Instance.activeButtons(true);
        }
    }
    void HandleExitClick()
    {
        game_manager.Instance.exitGame();
    }
}
