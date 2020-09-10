using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUi : Singlenton<MenuUi>
{
    [SerializeField] private WinUI _winOrLostSc = null;
    [SerializeField] private Text _cash = null;
    [SerializeField] private MoneySystemController _handlerMoney = null;

    public GameObject[] panels;
    public UnityEngine.UI.Button btnVolver;
    private GameObject currentPanel;
    private void Start() 
    {
        SetMoneyUi();
        currentPanel = panels[0];
        btnVolver.onClick.AddListener(HandlerReturnClick);
    }
    public void HandlerReturnClick() 
    {
        SoundContoller.Instance.PlayFxSound(3);

        if(currentPanel != panels[0]) 
        {
            currentPanel.SetActive(false);
            currentPanel = panels[0];
            currentPanel.SetActive(true);
        } 
        else 
        {
            btnVolver.gameObject.SetActive(false);
            OpenPanel(4);
        }
    }
    public void OpenPanel(int pos) 
    {
        currentPanel.SetActive(false);
        
        currentPanel = panels[pos];
        currentPanel.SetActive(true);
    }
    public void SetMoneyUi() 
    {
        _handlerMoney.LoadRound();
        _handlerMoney.LoadPlayerCash();
        _cash.text = _handlerMoney._cashBack.ToString();
    }
    /// <summary>
    /// Set UI when reward finished.
    /// </summary>
    /// <param name="win"></param>
    /// <param name="number"></param>
    /// <param name="isWin"></param>
    public void OnRewardFinishedUI(string win, string number, bool isWin)
    {
        _winOrLostSc.winOrLost(win, number, isWin);
    }
}
