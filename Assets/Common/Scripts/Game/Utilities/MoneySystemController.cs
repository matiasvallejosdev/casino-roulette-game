using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class MoneySystemController : Singlenton<MoneySystemController>
{
    public int _cashNew = 0;
    public int _cashBack = 0;

    private PlayerData myPlayer;

    public FichasSave[] _actualRound;
    public FichasSave[] _lastRound;

    private int[] player = new int[1];
    
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void SavePlayerCash() 
    {
        // Get the previous cash
        LoadRound();
        LoadPlayerCash();
        // Verify the new cash and sum or rest depend the operator
        if(_cashNew > 0) 
        {
            Debug.Log(_cashNew + _cashBack);
            player[0] = _cashNew + _cashBack;
        } 
        else if(_cashNew < 0) 
        {
            _cashNew *= -1;
            player[0] = _cashBack - _cashNew;
        }
        // Save only the cash and set the round save in false
        SaveSystem.SavePlayer(player, null, false);
        Debug.Log("Guardando CASH del player: " + player[0]);
    }
    public void SavePlayerRound()
    {
        // Get the previous rounded and cash
        LoadRound();
        LoadPlayerCash();
        // Set the values
        player[0] = _cashBack;
        FichasSave[] r = _actualRound;
        // Save cash and round
        SaveSystem.SavePlayer(player, r, true);
    }
    public void InitializeGameRound() 
    {
        LoadRound();
        LoadPlayerCash();
        LoadPlayerRound();
    }
    public void LoadRound()
    {
        myPlayer = SaveSystem.LoadPlayer();
    }
    public void LoadPlayerCash() 
    {
        _cashBack = myPlayer.cash;
        player[0] = myPlayer.cash;
        //Debug.Log("Loading Cash for player: " + _cashBack.ToString());   
    }
    public void LoadPlayerRound() 
    {
        if(myPlayer.fichas != null) 
        {
            _lastRound = myPlayer.fichas;
            //Debug.Log("Loading Round for player: " + myPlayer.fichas.Length.ToString());
        }
    }
}
