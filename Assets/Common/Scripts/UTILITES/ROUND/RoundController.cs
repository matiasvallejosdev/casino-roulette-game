using System.Collections;
using UnityEngine;

public class RoundController : Singlenton<RoundController>
{
    [Header("Round Controller")]
    [SerializeField] private int betActual = 0;
    [SerializeField] private int cashTotal { get; set; }
    [SerializeField] private FichasSave[] oldRound { get; set; }

    public EventsRound.EventRoundState OnRoundChanged;
    public EventsRound.EventApuestaChanged OnApuestaChanged;

    [Header("Variables")]
    public GameObject magnetDestroyer;
    public manejador_fichas _scManejadorFichas = null;
    private GameObject[] _goButtons;

    private void Start()
    {
        _goButtons = GameObject.FindGameObjectsWithTag("Button");

        OnRoundChanged.Invoke(cashTotal, cashTotal);
        OnApuestaChanged.Invoke(0, 0, betActual);
    }
    public int GetCashTotal() 
    {
        int c = cashTotal;
        return c;
    }

    // Operator in round values
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
    public bool CheckBetValue(int valueFicha)
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

    // Operators running the game
    public void OnRoundIntialize()
    {
        //activeButtons(false);
        PaymentController.Instance._fichasPrevious.Clear();
        ResetButtons();
    }
    public void OnRoundFinished(int newCash, GameObject[] round)
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
        saveCash(newCash);
        // Carga el archivo
        loadRound();
    }
    public void OnGameClosed() 
    {
        Debug.Log("Closed the game. With cash: " + cashTotal);
        // Guarda el archivo
        saveRound(RoundFichas.ReturnFichasToSave(GameObject.FindGameObjectsWithTag("Fichas")));
    }
    public void OnGameOpened() 
    {
        loadRound();

        if (oldRound.Length > 0) 
        {
            // Charge the old round
            RoundFichas.RestorePlayerRound(oldRound);
        }
    }
    public void OnRewardFinished(int newCash)
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
    
    // Undo the fichas in table one at a time
    public void UndoTable(GameObject ficha) 
    {
        GameObject btn = ficha.GetComponent<fichas>().button;
        if (ficha.GetComponent<fichas>()._valor > 0)
        {
            int aux = ficha.GetComponent<fichas>()._valor;
            restarBet(aux);
            sumarCashTotal(aux);
            Destroy(ficha);
        }
    }
    // Reset table and components
    public void ResetTable()
    {
        ResetButtons();
        int count = GameObject.FindGameObjectsWithTag("Fichas").Length;
        if(count > 0)
        {
            int aux = betActual;
            restarBet(betActual);
            sumarCashTotal(aux);
            MagnetDestroyerFichas(3.5f);
        }
    }   
    public void MagnetDestroyerFichas(float seg)
    {
        StartCoroutine(magnet(seg));
    }
    IEnumerator magnet(float seg)
    {
        ActivateButtons(false);
        magnetDestroyer.SetActive(true);
        yield return new WaitForSeconds(seg);
        magnetDestroyer.SetActive(false);
        ActivateButtons(true);
    }
    private void ResetButtons()
    {
        GameObject[] _goButtons = GameObject.FindGameObjectsWithTag("Button");
        foreach (var btn in _goButtons)
        {
            fx_button sc = btn.GetComponent<fx_button>();
            sc.resetOnTop();
        }
    }
    public void ActivateButtons(bool isOn)
    {
        foreach (var btn in _goButtons)
        {
            btn.SetActive(isOn);
        }
    }

    // Player save controller system
    private void saveCash(int cash) 
    {
        MoneySystemController.Instance._cashNew = cash;
        MoneySystemController.Instance.SavePlayerCash();
    }
    private void saveRound(FichasSave[] round) 
    {
        MoneySystemController.Instance._actualRound = round;
        MoneySystemController.Instance.SavePlayerRound();
    }
    private void loadRound()
    {
        MoneySystemController.Instance.InitializeGameRound();

        cashTotal = MoneySystemController.Instance._cashBack;
        oldRound = MoneySystemController.Instance._lastRound;

        OnRoundChanged.Invoke(cashTotal, cashTotal);
    }
}
