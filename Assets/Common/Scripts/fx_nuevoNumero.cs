using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fx_nuevoNumero : MonoBehaviour
{
    [Header("References of buttons")]
    public GameObject[] numeros;
    public GameObject[] docenas;
    public GameObject[] columnas;
    public GameObject[] pareimpar;
    public GameObject[] rojoenegro;
    public GameObject[] dieciochoavos;
    public GameObject[] auxiliar;
    private backNumber_controller numeroAnterior; 
    // Start is called before the first frame update
    void Start()
    {      
        numeroAnterior = GameObject.Find("BackNumberHUD").GetComponent<backNumber_controller>();
        auxiliar = new GameObject[7];

        numeros = new GameObject[37];
        for(int i = 0; i < numeros.Length; i++)
        {
            numeros[i] = GameObject.Find("n_" + i);
        }

        docenas = new GameObject[4];
        for(int i = 0; i < docenas.Length; i++)
        {
            docenas[i] = GameObject.Find("d_" + i);
        }

        columnas = new GameObject[4];
        for(int i = 0; i < columnas.Length; i++)
        {
            columnas[i] = GameObject.Find("c_" + i);
        }
    }

    public void effectNewNumber(int num)
    {
        number(num);
        numeroAnterior.nuevoNumeroHUD(num);
    }
    private void number(int num)
    {
        int d = 0;
        int i = 0;
        bool encontrado = false;
        // Busca el numero
        while(i < numeros.Length && encontrado != true)
        {
            if(numeros[i].gameObject.name == "n_" + num)
            {
                auxiliar[d] = numeros[i];
                encontrado = true;
                fx_button sc_aux = numeros[i].gameObject.GetComponent<fx_button>();
                sc_aux.fx_fichasOn(true);
                d++;
                break;
            }
            i++;
        }
        // Busca las: Docenas, Columnas, Otros
        if(num != 0)
        {
            // Busca la docena
            encontrado = false;
            i = 0;
            while (i < docenas.Length && encontrado != true)
            {
                GameObject auxDocena = docenas[i];
                fx_button sc_aux = auxDocena.GetComponent<fx_button>();
                for (int n = 0; n < sc_aux.valor.Length; n++)
                {
                    if (sc_aux.valor[n] == num)
                    {
                        auxiliar[d] = auxDocena;
                        sc_aux.fx_fichasOn(true);
                        encontrado = true;
                        d++;
                        break;
                    }
                }
                i++;
            }
            // Busca la columna
            encontrado = false;
            i = 0;
            while (i < columnas.Length && encontrado != true)
            {
                GameObject auxColumna = columnas[i];
                fx_button sc_aux = auxColumna.GetComponent<fx_button>();
                for (int n = 0; n < sc_aux.valor.Length; n++)
                {
                    if (sc_aux.valor[n] == num)
                    {
                        auxiliar[d] = auxColumna;
                        sc_aux.fx_fichasOn(true);
                        encontrado = true;
                        d++;
                        break;
                    }
                }
                i++;
            }
            // Busca par e impar
            encontrado = false;
            i = 0;
            while (i < pareimpar.Length && encontrado != true)
            {
                GameObject auxParEimpar = pareimpar[i];
                fx_button sc_aux = auxParEimpar.GetComponent<fx_button>();
                for (int n = 0; n < sc_aux.valor.Length; n++)
                {
                    if (sc_aux.valor[n] == num)
                    {
                        auxiliar[d] = auxParEimpar;
                        sc_aux.fx_fichasOn(true);
                        encontrado = true;
                        d++;
                        break;
                    }
                }
                i++;
            }
            // Busca rojo e negro
            encontrado = false;
            i = 0;
            while (i < rojoenegro.Length && encontrado != true)
            {
                GameObject auxRojoEnegro = rojoenegro[i];
                fx_button sc_aux = auxRojoEnegro.GetComponent<fx_button>();
                for (int n = 0; n < sc_aux.valor.Length; n++)
                {
                    if (sc_aux.valor[n] == num)
                    {
                        auxiliar[d] = auxRojoEnegro;
                        sc_aux.fx_fichasOn(true);
                        encontrado = true;
                        d++;
                        break;
                    }
                }
                i++;
            }
            // Busca dieciochoavos
            encontrado = false;
            i = 0;
            while (i < dieciochoavos.Length && encontrado != true)
            {
                GameObject auxDieciochoavos = dieciochoavos[i];
                fx_button sc_aux = auxDieciochoavos.GetComponent<fx_button>();
                for (int n = 0; n < sc_aux.valor.Length; n++)
                {
                    if (sc_aux.valor[n] == num)
                    {
                        auxiliar[d] = auxDieciochoavos;
                        sc_aux.fx_fichasOn(true);
                        encontrado = true;
                        d++;
                        break;
                    }
                }
                i++;
            }
        }   

        // Busca el numero ganador si lo encuentra 
        findNumber_winner(num);

        // Espera unos segundos y apaga sprites
        StartCoroutine(wait_seconds(16));
    }
    IEnumerator wait_seconds(int seg)
    {
        yield return new WaitForSeconds(seg);
        for(int e = 0; e < auxiliar.Length; e++)
        {
            // Apaga los Sprites
            if(auxiliar[e] != null)
            {
                fx_button sc_aux = auxiliar[e].GetComponent<fx_button>();
                sc_aux.fx_fichasOn(false);
            }
        }
        // Elimina las fichas en mesa
        RoundController.Instance.magnetDestroyerSystem(3.5f);
    }

    private void findNumber_winner(int num)
    {
        GameObject[] go_fichas = GameObject.FindGameObjectsWithTag("Fichas");
        int[] aux;
        foreach (var ficha in go_fichas)
        {
            fichas sc_fichas = ficha.GetComponent<fichas>();
            if (sc_fichas._pleno)
            {
                // Code for fichas pleno
                aux = sc_fichas.getValueOfDictionary(sc_fichas.getKeyOfDictionary());
                bool isFounded = false;
                foreach (var value in aux)
                {
                    if (value == num)
                    {
                        //Debug.Log("Match! NUMBER WINNER");
                        GameObject go_buttonWinner = GameObject.Find("n_" + num);
                        fx_button sc_buttonWinner = go_buttonWinner.GetComponent<fx_button>();
                        StartCoroutine(titilar(sc_buttonWinner));
                        //Send Message to the payment system winner
                        PaymentController.Instance._fichasWinnerPlenos.Add(ficha);
                        isFounded = true;
                        // Save Rounded
                        PaymentController.Instance._fichasPrevious.Add(ficha);
                    }
                }
                if(!isFounded)
                {
                    //Send Message to the payment system losted
                    PaymentController.Instance._fichasLosted.Add(ficha);
                    // Save Rounded
                    PaymentController.Instance._fichasPrevious.Add(ficha);
                }
            } else if(!sc_fichas._pleno)
            {
                // Code for fichas in the middle
                aux = sc_fichas.getValueOfDictionary(sc_fichas.getKeyOfDictionary());
                bool isFounded = false;
                foreach (var value in aux)
                {
                    if (value == num)
                    {
                        // Debug.Log("Match! NUMBER WINNER");
                        GameObject go_buttonWinner = GameObject.Find("n_" + num);
                        fx_button sc_buttonWinner = go_buttonWinner.GetComponent<fx_button>();
                        StartCoroutine(titilar(sc_buttonWinner));
                        // Send Message to the payment system
                        PaymentController.Instance._fichasWinnerMedios.Add(ficha);
                        isFounded = true;
                        // Save Rounded
                        PaymentController.Instance._fichasPrevious.Add(ficha);
                    }
                }
                if (!isFounded)
                {
                    //Send Message to the payment system losted
                    PaymentController.Instance._fichasLosted.Add(ficha);
                    // Save Rounded
                    PaymentController.Instance._fichasPrevious.Add(ficha);
                }
            }
        }
    }
    IEnumerator titilar(fx_button sc)
    {
        sc.fx_numberWinner(true);
        yield return new WaitForSeconds(0.5f);
        sc.fx_numberWinner(false);
        yield return new WaitForSeconds(0.5f);
        sc.fx_numberWinner(true);
        yield return new WaitForSeconds(0.5f);
        sc.fx_numberWinner(false);
        yield return new WaitForSeconds(0.5f);
        sc.fx_numberWinner(true);
        yield return new WaitForSeconds(0.5f);
        sc.fx_numberWinner(false);
        yield return new WaitForSeconds(0.5f);
        sc.fx_numberWinner(true);
        yield return new WaitForSeconds(0.5f);
        sc.fx_numberWinner(false);
        yield return new WaitForSeconds(0.5f);
        sc.fx_numberWinner(true);
        yield return new WaitForSeconds(0.5f);
        sc.fx_numberWinner(false);
        yield return new WaitForSeconds(0.5f);
        sc.fx_numberWinner(true);
        yield return new WaitForSeconds(0.5f);
        sc.fx_numberWinner(false);
        yield return new WaitForSeconds(0.5f);
        sc.fx_numberWinner(true);
        yield return new WaitForSeconds(0.5f);
        sc.fx_numberWinner(false);
        yield return new WaitForSeconds(0.5f);
        sc.fx_numberWinner(true);
        yield return new WaitForSeconds(0.5f);
        sc.fx_numberWinner(false);
        yield return new WaitForSeconds(0.5f);
        sc.fx_numberWinner(false);
        yield return new WaitForSeconds(0.5f);
        sc.fx_numberWinner(true);
        yield return new WaitForSeconds(0.5f);
        sc.fx_numberWinner(false);
        yield return new WaitForSeconds(0.5f);
        sc.fx_numberWinner(true);
        yield return new WaitForSeconds(0.5f);
        sc.fx_numberWinner(false);
        yield return new WaitForSeconds(0.5f);
          sc.fx_numberWinner(false);
        yield return new WaitForSeconds(0.5f);
        sc.fx_numberWinner(true);
        yield return new WaitForSeconds(0.5f);
        sc.fx_numberWinner(false);
        yield return new WaitForSeconds(0.5f);
        sc.fx_numberWinner(true);
        yield return new WaitForSeconds(0.5f);
        sc.fx_numberWinner(false);
        yield return new WaitForSeconds(0.5f);
        sc.fx_numberWinner(true);
        yield return new WaitForSeconds(0.5f);
        sc.fx_numberWinner(false);
        yield return new WaitForSeconds(0.5f);
        sc.fx_numberWinner(true);
        yield return new WaitForSeconds(0.5f);
        sc.fx_numberWinner(false);
        yield return new WaitForSeconds(0.5f);
    }
    
}
