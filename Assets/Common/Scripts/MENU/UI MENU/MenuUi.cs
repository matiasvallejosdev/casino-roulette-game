using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUi : Singlenton<MenuUi>
{
    [SerializeField] private WinUI _winOrLostSc = null;
    [SerializeField] private Text _cash = null;
    [SerializeField] private MoneySystemController _handlerMoney = null;
    private void Start() 
    {
        setMoneyUi();
    }
    public void setMoneyUi() 
    {
        _handlerMoney.LoadRound();
        _handlerMoney.LoadPlayerCash();
        _cash.text = _handlerMoney._cashBack.ToString();
    }
    public void rewardVideoUiFinished(string win, string number, bool isWin)
    {
        _winOrLostSc.winOrLost(win, number, isWin);
    }
}
