using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundController : Singlenton<RoundController>
{
    [Header("Round Controller")]
    [SerializeField] private int betActual = 0;
    [SerializeField] private int cashTotal = 120;

    public EventsRound.EventRoundState OnRoundChanged;
    public EventsRound.EventApuestaChanged OnApuestaChanged;

    [Header("Variables")]
    [SerializeField] private GameObject magnetDestroyer;
    [SerializeField] private manejador_fichas _scManejadorFichas;
    private GameObject[] _goButtons;


    private void Start()
    {
        _goButtons = GameObject.FindGameObjectsWithTag("Button");

        OnRoundChanged.Invoke(0, cashTotal);
        OnApuestaChanged.Invoke(0, 0, betActual);
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
        OnRoundChanged.Invoke(aux, cashTotal);
    }
    private void restarCashTotal(int cashLost)
    {
        int aux = cashTotal;
        cashTotal -= cashLost;
        if(cashTotal < 0)
        {
            cashTotal = 0;
        }
        OnRoundChanged.Invoke(aux, cashTotal);
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

    public void onRoundFinished(int newCash, int num)
    {
        // Sum Cash
        if (newCash >= 0)
        {
            sumarCashTotal(newCash);
            CanvasUI.Instance.turnWinOrLost("YOU WIN!", newCash.ToString(), true);
        } else if (newCash < 0)
        {
            restarCashTotal(newCash * -1);
            CanvasUI.Instance.turnWinOrLost("YOU LOST!", newCash.ToString(), false);
        }
        // Delete fichas
        PaymentController.Instance.deleteFichasInPayment();
        // Save Rounded
        PaymentController.Instance.saveRounded();
        // Apuesta to zero
        restarBet(betActual);
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
    public void recoverPreviousFicha()
    {
        GameObject[] fichasPrevious = PaymentController.Instance._fichasPrevious.ToArray();
        if (findIfIsPossibleDuplicate(fichasPrevious))
        {
            SoundContoller.Instance.fx_sound(4);
            foreach (var ficha in PaymentController.Instance._fichasPrevious)
            {
                fichas _scFicha = ficha.GetComponent<fichas>();
                if (verficatedValueOfFicha(_scFicha._valor))
                {
                    Debug.Log("Bet possible");
                    // Get Options
                    string clave = _scFicha.button.GetComponent<fx_button>().clave;
                    int[] valor = _scFicha.button.GetComponent<fx_button>().valor;
                    bool pleno = _scFicha.button.GetComponent<fx_button>().pleno;
                    Vector2 pivot = _scFicha.button.GetComponent<fx_button>().GetSpritePivot(_scFicha.button.GetComponent<fx_button>()._spriteRender.sprite);
                    bool _fichasTopBoolean = false;
                    if (_scFicha.button.GetComponent<fx_button>()._fichasOnTop != 0)
                    {
                        _fichasTopBoolean = true;
                    }
                    Vector2 offset = _scFicha.button.GetComponent<fx_button>().GetOffsetFicha();
                    // Ficha Nueva
                    _scManejadorFichas.previous_ficha(pivot, _fichasTopBoolean, offset, ficha, clave, valor, pleno, _scFicha.button);
                    // Sound Control
                    SoundContoller.Instance.fx_sound(1);
                }
                else
                {
                    Debug.Log("Bet is not possible because the value of ficha is very high");
                }
            }
        }
        else
        {
            SoundContoller.Instance.fx_sound(3);
        }  
    }

    private int getTotalPreviousRounded()
    {
        int t = 0;
        List<GameObject> aux = PaymentController.Instance._fichasPrevious;
        foreach (var ficha in aux)
        {
            t = t + ficha.GetComponent<fichas>()._valor;
        }
        return t;
    }

    public void deleteFichas()
    {
        resetButtons();

        int count = GameObject.FindGameObjectsWithTag("Fichas").Length;
        if(count > 0)
        {
            int aux = betActual;
            restarBet(betActual);
            sumarCashTotal(aux);

            magnetDestroyerSystem(3.5f);
        }
    }

    public void magnetDestroyerSystem(float seg)
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

    public void multiplierActualFichas()
    {
        GameObject[] fichasInGame = GameObject.FindGameObjectsWithTag("Fichas");

        if (findIfIsPossibleDuplicate(fichasInGame))
        {
            // Duplicate
            SoundContoller.Instance.fx_sound(4);

            Debug.Log("Duplicando fichas en mesa");
            foreach (var ficha in fichasInGame)
            {
                fichas _scFicha = ficha.GetComponent<fichas>();
                if (verficatedValueOfFicha(_scFicha._valor))
                {
                    Debug.Log("Bet possible");
                    // Get Options
                    string clave = _scFicha.button.GetComponent<fx_button>().clave;
                    int[] valor = _scFicha.button.GetComponent<fx_button>().valor;
                    bool pleno = _scFicha.button.GetComponent<fx_button>().pleno;
                    Vector2 pivot = _scFicha.button.GetComponent<fx_button>().GetSpritePivot(_scFicha.button.GetComponent<fx_button>()._spriteRender.sprite);
                    bool _fichasTopBoolean = false;
                    if (_scFicha.button.GetComponent<fx_button>()._fichasOnTop != 0)
                    {
                        _fichasTopBoolean = true;
                    }
                    Vector2 offset = _scFicha.button.GetComponent<fx_button>().GetOffsetFicha();
                    // Ficha Nueva
                    _scManejadorFichas.previous_ficha(pivot, _fichasTopBoolean, offset, ficha, clave, valor, pleno, _scFicha.button);
                    // Sound Control
                    SoundContoller.Instance.fx_sound(1);
                }
                else
                {
                    Debug.Log("Bet is not possible because the value of ficha is very high");
                }
            }
        }
        else
        {
            SoundContoller.Instance.fx_sound(3);
        }
    }

    private bool findIfIsPossibleDuplicate(GameObject[] fichasInGame)
    {
        bool isPossible = false;
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
}
