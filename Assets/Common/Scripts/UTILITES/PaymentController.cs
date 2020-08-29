using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaymentController : Singlenton<PaymentController>
{
    // Payment Controller System
    // He actived when the roullete is finished and finded the number winner
    // Calculate the bet with the number winner and the equation of payment

    public List<GameObject> _fichasWinnerPlenos = new List<GameObject>();
    public List<GameObject> _fichasWinnerMedios = new List<GameObject>();
    public List<GameObject> _fichasLosted = new List<GameObject>();
    public List<GameObject> _fichasPrevious = new List<GameObject>();

    public int totalPaymentSystem;
    public int totalFichasWinner;

    /// <summary>
    /// Execute the payment system.
    /// </summary>
    /// <param name="num"></param>
    public void roundFinished(int num)
    {
        // Calculate payment
        paymentSystem();
        // Finished the rounded and display the new values
        RoundController.Instance.onRoundFinished(totalPaymentSystem, num, _fichasPrevious.ToArray());
    }
    /// <summary>
    /// Calculate all the payments winner and losted.
    /// </summary>
    public void paymentSystem()
    {
        totalPaymentSystem = 0;
        totalFichasWinner = 0;

        int totalPaymentWinner = calculateTotalPaymentWinner();
        //Debug.Log("El total ganado es: " + totalPaymentWinner);

        int totalPaymentLosted = calculateTotalPaymentLosted();
       //Debug.Log("El total perdido es: " + totalPaymentLosted);

        int totalFichasWin = calculateTotalFichasWin();
        //Debug.Log("Fichas devueltas: " + totalFichasWinner);

        totalPaymentSystem = totalPaymentWinner - (totalPaymentLosted * - 1);
        totalPaymentSystem = totalPaymentSystem + totalFichasWin;

        //Debug.Log("La casa paga: " + totalPaymentSystem);
    }
    
    // Payment in plenos, in medios and the equation.
    private int paymentPlenos()
    {
        int total = 0;
        Debug.Log("Calculando el pago de las fichas plenas ganadoras");
        foreach(var element in _fichasWinnerPlenos)
        {
            // Four cases
            // Numero (1a36) 
            fichas scFichas = element.GetComponent<fichas>();
            string key = scFichas.getKeyOfDictionary();
            switch (key)
            {
                case "Numero":
                    int sum = equationPaymentNumeros(scFichas.countTheNumberOfTheDictionary(key), scFichas._valor);
                    total = total + sum;
                    //Debug.Log("Pagando a el numero " + scFichas.gameObject.name);
                    break;
                case "Docena":
                    int sum1 = equationPaymentNumeros(scFichas.countTheNumberOfTheDictionary(key), scFichas._valor);
                    total = total + sum1;
                    //Debug.Log("Pagando a la docena " + scFichas.gameObject.name);
                    break;
                case "Columna":
                    int sum2 = equationPaymentNumeros(scFichas.countTheNumberOfTheDictionary(key), scFichas._valor);
                    total = total + sum2;
                    //Debug.Log("Pagando a la columna " + scFichas.gameObject.name);
                    break;
                case "Otros":
                    int sum3 = equationPaymentNumeros(scFichas.countTheNumberOfTheDictionary(key), scFichas._valor);
                    total = total + sum3;
                    //Debug.Log("Pagando a otros " + scFichas.gameObject.name);
                    break;
                default:
                    int sum4 = 0;
                    total = total + sum4;
                    //Debug.Log("Tag empty");
                    break;
            }
            //Otros(1a18,19a36)(Odd,Even)(Black,Red)
        }
        return total;
    }
    private int paymentMedios()
    {
        int total = 0;
        foreach (var element in _fichasWinnerMedios)
        {
            Debug.Log("Calculando el pago de las fichas medias ganadoras");
            fichas scFichas = element.GetComponent<fichas>();
            string key = scFichas.getKeyOfDictionary();
            switch (key)
            {
                case "Medio":
                    int sum = equationPaymentNumeros(scFichas.countTheNumberOfTheDictionary(key), scFichas._valor);
                    total = total + sum;
                    //Debug.Log("Pagando a medios " + scFichas.gameObject.name);
                    break;
            }
        }
        return total;
    }
    private int equationPaymentNumeros(int cantidadNumerosAbarcados, int valueFicha)
    {
        int payment = 0;

        // Calculate the multiply depending the numbers abarcated of the  ficha
        int n = cantidadNumerosAbarcados;
        int multiply = (36 - n) / n;
        // Payment
        payment = valueFicha * multiply;

        return payment;
    }
    
    // Calculate and return the all values of payment.
    private int calculateTotalPaymentWinner()
    {
        int totalPayment = 0;

        int totalPlenos = paymentPlenos();
        int totalMedios = paymentMedios();

        totalPayment = totalPlenos + totalMedios;

        return totalPayment;
    }
    private int calculateTotalPaymentLosted()
    {
        int totalLost = 0;
        foreach(var element in _fichasLosted)
        {
            fichas scFichas = element.GetComponent<fichas>();
            int value = scFichas._valor;
            totalLost = totalLost - value;
        }
        return totalLost;
    }
    private int calculateTotalFichasWin()
    {
        int total = 0;
        foreach(var ficha in _fichasWinnerPlenos)
        {
            fichas scFichas = ficha.GetComponent<fichas>();
            int value = scFichas._valor;
            total = total + value;
        }
        foreach(var ficha in _fichasWinnerMedios)
        {
            fichas scFichas = ficha.GetComponent<fichas>();
            int value = scFichas._valor;
            total = total + value;
        }

        totalFichasWinner = total;
        return total;
    }
    
    /// <summary>
    /// Delete the assiganed fichas in payment.
    /// </summary>
    public void deleteFichasInPayment()
    {
        PaymentController.Instance._fichasWinnerPlenos.Clear();
        PaymentController.Instance._fichasWinnerMedios.Clear();
        PaymentController.Instance._fichasLosted.Clear();
    }

    /// <summary>
    /// Save all the fichas in the payment.
    /// </summary>
    public void saveRounded()
    {
        List<GameObject> listAux = new List<GameObject>();

        GameObject plenoPrevious = GameObject.Find("PlenosPrevious");
        GameObject medioPrevious = GameObject.Find("MediosPrevious");

        foreach (var ficha in _fichasPrevious)
        {
            GameObject go = Instantiate(ficha, ficha.transform.position, ficha.transform.rotation);
            go.name = "Previous_" + ficha.name;
            if (ficha.GetComponent<fichas>()._pleno)
            {
                go.transform.SetParent(plenoPrevious.gameObject.transform);
            }
            else
            {
                go.transform.SetParent(medioPrevious.gameObject.transform);
            }
            listAux.Add(go);
            go.SetActive(false);
        }

        _fichasPrevious.Clear();
        _fichasPrevious = listAux;
    }
}
