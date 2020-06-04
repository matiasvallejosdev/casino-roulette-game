using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class round_manager : Singlenton<round_manager>
{
    [SerializeField] private int betActual = 0;
    [SerializeField] private int cashTotal = 120;

    public EventsRound.EventRoundState OnRoundChange;
    public EventsRound.EventApuestaChanged OnApuestaChanged;

    private void Start()
    {
        OnRoundChange.Invoke(0, cashTotal);
        OnApuestaChanged.Invoke(0, 0, betActual);
    }

    public bool verficatedValueOfFicha(int valueFicha)
    {
        bool aux =  true;
        if (valueFicha <= cashTotal)
        {
            //Debug.Log("Apuesta (TRUE!): " + valueFicha + " .Dinero total" + cashTotal);
            aux = true;
            restarCashTotal(valueFicha);
            sumarBet(valueFicha);
        }
        else
        {
            //Debug.Log("Apuesta (FALSE!): " + valueFicha + " .Dinero total" + cashTotal);
            aux = false;
        }
        return aux;
    }

    private void sumarCashTotal(int cashWinner)
    {
        int aux = cashTotal;
        cashTotal += cashWinner;
        OnRoundChange.Invoke(aux, cashTotal);
    }
    private void restarCashTotal(int cashLost)
    {
        int aux = cashTotal;
        cashTotal -= cashLost;
        OnRoundChange.Invoke(aux, cashTotal);
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

}
