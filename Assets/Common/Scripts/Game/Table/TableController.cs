using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableController : Singlenton<TableController>
{
    public FxNewNumber fxNewNumber;
    public EventButtonRoullete.EventLongPress OnLongPressButton { get; private set; }

    // Table references
    [Header("Table")]
    public GameObject[] numeros;
    public GameObject[] docenas;
    public GameObject[] columnas;
    public GameObject[] pareimpar;
    public GameObject[] rojoenegro;
    public GameObject[] dieciochoavos;

    void Start()
    {
        OnLongPressButton.AddListener(HandlerOnLongPress);
    }
    
    public GameObject[] FindEffectNumbers(int num)
    {
        List<GameObject> auxiliar = new List<GameObject>();

        // Busca en los numeros

        foreach(GameObject n in numeros)
        {
            ButtonDisplay b = n.GetComponent<ButtonDisplay>();
            if(b.button.value[0] == num)
            {
                auxiliar.Add(n);
                b.FxFicha(true);
                break;
            }
        }

        if(num != 0)
        {
            // Busca la docena y columna
            foreach(GameObject n in docenas)
            {
                ButtonDisplay b = n.GetComponent<ButtonDisplay>();
                for (int index = 0; index < b.button.value.Length; index++)
                {
                    if (b.button.value[index] == num)
                    {
                        auxiliar.Add(n);
                        b.FxFicha(true);
                        break;
                    }
                }
            }

            foreach(GameObject n in columnas)
            {
                ButtonDisplay b = n.GetComponent<ButtonDisplay>();
                for (int index = 0; index < b.button.value.Length; index++)
                {
                    if (b.button.value[index] == num)
                    {
                        auxiliar.Add(n);
                        b.FxFicha(true);
                        break;
                    }
                }
            }
            // Busca par e impar
            foreach(GameObject n in pareimpar)
            {
                ButtonDisplay b = n.GetComponent<ButtonDisplay>();
                for (int index = 0; index < b.button.value.Length; index++)
                {
                    if (b.button.value[index] == num)
                    {
                        auxiliar.Add(n);
                        b.FxFicha(true);
                        break;
                    }
                }
            }
            // Busca rojo e negro
            foreach(GameObject n in rojoenegro)
            {
                ButtonDisplay b = n.GetComponent<ButtonDisplay>();
                for (int index = 0; index < b.button.value.Length; index++)
                {
                    if (b.button.value[index] == num)
                    {
                        auxiliar.Add(n);
                        b.FxFicha(true);
                        break;
                    }
                }
            }
            // Busca dieciochoavos
            foreach(GameObject n in dieciochoavos)
            {
                ButtonDisplay b = n.GetComponent<ButtonDisplay>();
                for (int index = 0; index < b.button.value.Length; index++)
                {
                    if (b.button.value[index] == num)
                    {
                        auxiliar.Add(n);
                        b.FxFicha(true);
                        break;
                    }
                }
            }
        } 

        // Busca el numero ganador  
        FindFichasWinner(num);
        return auxiliar.ToArray();
    }


    /// <summary>
    /// Find the number winner and if founded set the flicker effect.
    /// </summary>
    /// <param name="numberWinner"></param>
    private void FindFichasWinner(int numberWinner)
    {
        GameObject[] FichasInTable = GameObject.FindGameObjectsWithTag("Fichas");
        int[] buttonValues;

        foreach (var ficha in FichasInTable)
        {
            FichaDisplay fichaDisplay = ficha.GetComponent<FichaDisplay>();
            buttonValues = fichaDisplay.currentButton.button.value;
            bool isFounded = false;

            if (fichaDisplay.currentButton.button.isPleno)
            {
                // Code for fichas pleno
                foreach (var value in buttonValues)
                {
                    if (value == numberWinner)
                    {
                        //Debug.Log("Match! NUMBER WINNER");
                        ButtonDisplay sc_buttonWinner = fichaDisplay.currentButton;
                        StartCoroutine(fxNewNumber.TititlarNumberWinner(sc_buttonWinner));
                        //Send Message to the payment system winner
                        PaymentController.Instance._fichasWinnerPlenos.Add(ficha);
                        isFounded = true;
                        // Save Rounded
                        PaymentController.Instance._fichasPrevious.Add(ficha);
                    } 
                }
            } 
            else if(!fichaDisplay.currentButton.button.isPleno)
            {
                // Code for fichas in the middle
                foreach (var value in buttonValues)
                {
                    if (value == numberWinner)
                    {
                        // Debug.Log("Match! NUMBER WINNER");
                        ButtonDisplay sc_buttonWinner = fichaDisplay.currentButton;
                        StartCoroutine(fxNewNumber.TititlarNumberWinner(sc_buttonWinner));
                        // Send Message to the payment system
                        PaymentController.Instance._fichasWinnerMedios.Add(ficha);
                        isFounded = true;
                        // Save Rounded
                        PaymentController.Instance._fichasPrevious.Add(ficha);
                    }
                }
            }
            
            if(!isFounded)
            {
                //Send Message to the payment system losted
                PaymentController.Instance._fichasLosted.Add(ficha);
                // Save Rounded
                PaymentController.Instance._fichasPrevious.Add(ficha);
            }
        }
    }

/*
    private void findFichasInMesa() 
    {
        GameObject[] go_fichas = GameObject.FindGameObjectsWithTag("Fichas");
        foreach (var ficha in go_fichas) 
        {
            // Save Rounded
            PaymentController.Instance._fichasPrevious.Add(ficha);
        }
    }
*/  
    
    private void HandlerOnLongPress(bool isPress, ButtonDisplay btn) 
    {
        /*if (isPress) 
        {
            if (btn.isLongPressed) 
            {
                // If button is long pressed
                btn.FxButtonPressed(isPress);
                StartCoroutine(WaitToDesactive(2, btn, false));
            } 
            else 
            {
                // If button isn't long pressed
                btn.fx_higlights();
            }
        }
        if (!isPress) 
        {
            if (btn.isLongPressed) 
            {
                btn.FxButtonPressed(isPress);
            }
        }*/
    }
    IEnumerator WaitToDesactive(int seg, ButtonDisplay btn, bool isPress) 
    {
        yield return new WaitForSeconds(seg);
        btn.FxButtonPressed(isPress);
    }
}
