using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundController : Singlenton<RoundController>
{
    [Header("Round Controller")]
    [SerializeField] private int betActual = 0;
    [SerializeField] private int cashTotal { get; set; }
    [SerializeField] private GameObject[] oldRound { get; set; }

    public EventsRound.EventRoundState OnRoundChanged;
    public EventsRound.EventApuestaChanged OnApuestaChanged;

    [Header("Variables")]
    public GameObject magnetDestroyer;
    public manejador_fichas _scManejadorFichas = null;
    private GameObject[] _goButtons;

    private void Start()
    {
        _goButtons = GameObject.FindGameObjectsWithTag("Button");

        onGameOpened();
        loadRound();

        OnRoundChanged.Invoke(cashTotal, cashTotal);
        OnApuestaChanged.Invoke(0, 0, betActual);
    }
    public int GetCashTotal() 
    {
        int c = 0;
        c = cashTotal;
        return c;
    }
    public bool verficatedValueOfFicha(int valueFicha)
    {
        bool aux = true;
        if (valueFicha <= cashTotal)
        {
            aux = true;
            restarCashTotal(valueFicha);
            sumarBet(valueFicha);
        }
        else
        {
            aux = false;
        }
        return aux;
    }

    private void sumarCashTotal(int cashWinner)
    {
        
        int aux = cashTotal;
        cashTotal += cashWinner;
        OnRoundChanged.Invoke(cashTotal, cashTotal);
    }

    private void restarCashTotal(int cashLost)
    {
        if(cashLost < 0) 
        {
            cashLost = cashLost * -1;
        }
        cashTotal -= cashLost;

        if (cashTotal < 0)
        {
            cashTotal = 0;
        }

        OnRoundChanged.Invoke(cashTotal, cashTotal);
    }

    private void sumarBet(int betSum)
    {
        int aux = betActual - betSum;
        betActual += betSum;
        OnApuestaChanged.Invoke(aux, betSum, betActual);
    }

    private void restarBet(int betRest)
    {
        int aux = betActual;
        betActual -= betRest;
        OnApuestaChanged.Invoke(aux, betRest, betActual);
    }

    public void onRoundIntialize()
    {
        //activeButtons(false);
        PaymentController.Instance._fichasPrevious.Clear();
        resetButtons();
    }

    public void onRoundFinished(int newCash, int num, GameObject[] round)
    {
        // Sum Cash
        if (newCash > 0)
        {
            sumarCashTotal(newCash);
            CanvasUI.Instance.turnWinOrLost("YOU WIN!", newCash.ToString(), true, 1);
        } else if (newCash < 0)
        {
            restarCashTotal(newCash * -1);
            CanvasUI.Instance.turnWinOrLost("YOU LOST!", newCash.ToString(), false, 1);
        }
        // Delete fichas
        PaymentController.Instance.deleteFichasInPayment();
        // Save Rounded
        PaymentController.Instance.saveRounded();
        // Apuesta to zero
        restarBet(betActual);
        // Guarda el archivo
        saveRound(newCash, round);
        // Carga el archivo
        loadRound();
    }

    public void onGameClosed() 
    {
        // Save Rounded
        PaymentController.Instance.saveRounded();
        // Guarda el archivo
        saveRound(cashTotal, PaymentController.Instance._fichasPrevious.ToArray());
    }
    public void onGameOpened() 
    {
        loadRound();
        if (oldRound.Length > 0) 
        {
            // Charge the old round
            RestorePreviousFichas(oldRound);
        }
    }
    public void onRewardFinished(int newCash)
    {
        if (newCash > 0) 
        {
            sumarCashTotal(newCash);
            // Guarda el archivo
            saveCash(newCash);
            // Carga el archivo
            loadRound();
        }
    }

    private void resetButtons()
    {
        GameObject[] _goButtons = GameObject.FindGameObjectsWithTag("Button");
        foreach(var btn in _goButtons)
        {
            fx_button sc = btn.GetComponent<fx_button>();
            sc.resetOnTop();
        }
    }
    public void activeButtons(bool isOn)
    {
        foreach (var btn in _goButtons)
        {
            btn.SetActive(isOn);
        }
    }

    /// <summary>
    /// Multiplier the fichas in the table when is possible.
    /// </summary>
    public void MultiplierFichasInTable()
    {
        GameObject[] fichasInGame = GameObject.FindGameObjectsWithTag("Fichas");
        if (FindIsPossibleToDuplicate(fichasInGame))
        {
            SoundContoller.Instance.fx_sound(4);
            foreach (var ficha in fichasInGame)
            {
                fichas _scFicha = ficha.GetComponent<fichas>();
                RecoverFicha(_scFicha);
            }
        }
        else
        {
            SoundContoller.Instance.fx_sound(3);
        }
    }

    /// <summary>
    /// Restore group of fichas in the game.
    /// </summary>
    /// <param name="fichas"></param>
    public void RestorePreviousFichas(GameObject[] fichas)
    {
        GameObject[] fichasPrevious = fichas;
        if (FindIsPossibleToDuplicate(fichasPrevious))
        {
            foreach (var ficha in PaymentController.Instance._fichasPrevious)
            {
                fichas _scFicha = ficha.GetComponent<fichas>();
                RecoverFicha(_scFicha);
            }
        } 
    }
    
    /// <summary>
    /// Restore specifc ficha in the game.
    /// </summary>
    /// <param name="fichaScript"></param>
    private void RecoverFicha(fichas fichaScript) 
    {
        if (verficatedValueOfFicha(fichaScript._valor))
        {
            Debug.Log("Bet possible");
            // Get Parameter
            #region Parameters
            string clave = fichaScript.button.GetComponent<fx_button>().clave;
            int[] valor = fichaScript.button.GetComponent<fx_button>().valor;
            bool pleno = fichaScript.button.GetComponent<fx_button>().pleno;
            Vector2 pivot = fichaScript.button.GetComponent<fx_button>().GetSpritePivot(fichaScript.button.GetComponent<fx_button>()._spriteRender.sprite);
            int fichaIndex = fichaScript.GetIndex();
            bool _fichasTopBoolean = false;
            if (fichaScript.button.GetComponent<fx_button>()._fichasOnTop != 0)
            {
                _fichasTopBoolean = true;
            }
            Vector2 offset = fichaScript.button.GetComponent<fx_button>().GetOffsetFicha();
            #endregion
            // Ficha Nueva
            _scManejadorFichas.RecoverFichas(pivot, _fichasTopBoolean, offset, clave, valor, pleno, fichaScript.button, fichaIndex);
            // Sound Control
            SoundContoller.Instance.fx_sound(1);
        }
        else
        {
            Debug.Log("Bet is not possible because the value of ficha is very high");
        }
    }

    public void DeleteFichasInTable()
    {
        resetButtons();
        int count = GameObject.FindGameObjectsWithTag("Fichas").Length;
        if(count > 0)
        {
            int aux = betActual;
            restarBet(betActual);
            sumarCashTotal(aux);
            MagnetDestroyerFichas(3.5f);
        }
    }
    
    // Destroyed all fichas in Table.
    public void MagnetDestroyerFichas(float seg)
    {
        StartCoroutine(magnet(seg));
    }
    IEnumerator magnet(float seg)
    {
        activeButtons(false);
        magnetDestroyer.SetActive(true);
        yield return new WaitForSeconds(seg);
        magnetDestroyer.SetActive(false);
        activeButtons(true);
    }

    // Search if is possible i will duplicate the new fichas
    private bool FindIsPossibleToDuplicate(GameObject[] fichasInGame)
    {
        bool isPossible;
        int aux = 0;
        foreach(var ficha in fichasInGame)
        {
            fichas _scFicha = ficha.GetComponent<fichas>();
            aux = aux + _scFicha._valor;
        }
        if(aux <= cashTotal)
        {
            isPossible = true;
        }
        else
        {
            isPossible = false;
        }
        return isPossible;
    }

    #region Save Round System
    private void saveCash(int cash) 
    {
        MoneySystemController.Instance._cashNew = cash;
        MoneySystemController.Instance.savePlayerCash();
    }
    private void saveRound(int cash, GameObject[] round) 
    {
        MoneySystemController.Instance._cashNew = cash;
        MoneySystemController.Instance._actualRound = round;

        MoneySystemController.Instance.savePlayerCash();
        MoneySystemController.Instance.savePlayerRound();
    }
    private void loadRound()
    {
        MoneySystemController.Instance.loadPlayerCash();
        MoneySystemController.Instance.loadPlayerRound();

        cashTotal = MoneySystemController.Instance._cashBack;
        oldRound = MoneySystemController.Instance._lastRound;

        OnRoundChanged.Invoke(cashTotal, cashTotal);
    }
    #endregion
}
