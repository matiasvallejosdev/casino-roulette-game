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

    public GameObject[] _actualRound;
    public FichasSave[] _lastRound { get; private set; }

    private int[] player = new int[1];
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void savePlayerCash() 
    {
        _cashBack = 0;
        loadPlayerCash();

        if(_cashNew > 0) 
        {
            Debug.Log(_cashNew + _cashBack);
            player[0] = _cashNew + _cashBack;
        } 
        else if(_cashNew < 0) 
        {
            //Debug.Log("CN: " + _cashNew);
            //Debug.Log("CB: " + _cashBack);
            _cashNew *= -1;
            player[0] = _cashBack - _cashNew;
        }
        FichasSave[] r = SavePlayer.GetFichas();
        SaveSystem.SavePlayer(player, r, false);
        Debug.Log("Guardando " + player[0]);
    }
    public void savePlayerRound()
    {
        _cashBack = 0;
        loadPlayerCash();

        if (_cashNew > 0)
        {
            //Debug.Log(_cashNew + _cashBack);
            player[0] = _cashNew + _cashBack;
        }
        else if (_cashNew < 0)
        {
            //Debug.Log("CN: " + _cashNew);
            //Debug.Log("CB: " + _cashBack);
            _cashNew *= -1;
            player[0] = _cashBack - _cashNew;
        }

        FichasSave[] r = SavePlayer.GetFichas();
        SaveSystem.SavePlayer(player, r, true);
        Debug.Log("Guardando " + player[0]);
    }
    public void loadPlayerCash() 
    {
        PlayerData player = SaveSystem.LoadPlayer();

        _cashBack = player.cash;
        Debug.Log("Asignando Moneda: " + _cashBack.ToString());   
    }
    public void loadPlayerRound() 
    {
        PlayerData player = SaveSystem.LoadPlayer();

        _lastRound = player.round.fichas;
        Debug.Log("Asignando Ronda Pasada: " + _lastRound.Length.ToString());
    }
}
