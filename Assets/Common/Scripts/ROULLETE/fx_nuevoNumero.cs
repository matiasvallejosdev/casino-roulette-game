using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fx_nuevoNumero : MonoBehaviour
{
    private static GameObject[] numeros = new GameObject[37];
    private static GameObject[] docenas = new GameObject[4];
    private static GameObject[] columnas = new GameObject[4];
    private static GameObject[] pareimpar = new GameObject[2];
    private static GameObject[] rojoenegro = new GameObject[2];
    private static GameObject[] dieciochoavos = new GameObject[2];
    private static GameObject[] auxiliar = new GameObject[37];

    private backNumber_controller numeroAnterior;

    // Start is called before the first frame update
    void Start()
    {      
        numeroAnterior = GameObject.Find("BackNumberHUD").GetComponent<backNumber_controller>();
        auxiliar = new GameObject[37];

        for(int i = 0; i < 37; i++)
        {
            numeros[i] = GameObject.Find("n_" + i);
        }

        for(int i = 0; i < 4; i++)
        {
            docenas[i] = GameObject.Find("d_" + i);
        }

        for(int i = 0; i < 4; i++)
        {
            columnas[i] = GameObject.Find("c_" + i);
        }

        pareimpar[0] = GameObject.Find("cl_par");
        pareimpar[1] = GameObject.Find("cl_impar");

        rojoenegro[0] = GameObject.Find("cl_red");
        rojoenegro[1] = GameObject.Find("cl_black");

        dieciochoavos[0] = GameObject.Find("cl_1a18");
        dieciochoavos[1] = GameObject.Find("cl_19a36");
    }
    /// <summary>
    /// Find the effect of the new number and set the button white.
    /// </summary>
    /// <param name="num"></param>
    public void effectNewNumber(int num)
    {
        number(num);
        // Set the new number in HUD
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

        // Busca el numero ganador  
        findNumber_winner(num);

        // Espera unos segundos y apaga sprites
        StartCoroutine(wait_seconds(16));
    }
    
    /// <summary>
    /// Wait and set off the sprite
    /// </summary>
    /// <param name="seg"></param>
    /// <returns></returns>
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
        RoundController.Instance.MagnetDestroyerFichas(3.5f);
    }
    /// <summary>
    /// Find the number winner and if founded set the flicker effect.
    /// </summary>
    /// <param name="num"></param>
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
    /// <summary>
    /// Find the all fichas in mesa and save them.
    /// </summary>
    private void findFichasInMesa() 
    {
        GameObject[] go_fichas = GameObject.FindGameObjectsWithTag("Fichas");
        foreach (var ficha in go_fichas) 
        {
            // Save Rounded
            PaymentController.Instance._fichasPrevious.Add(ficha);
        }
    }
    /// <summary>
    /// Set the button flicker
    /// </summary>
    /// <param name="sc"></param>
    /// <returns></returns>
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
