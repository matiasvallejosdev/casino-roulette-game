using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fichas_selected : MonoBehaviour
{
    private HandlerFichas scriptHandlerFicha;
    // Start is called before the first frame update
    void Start()
    {
        scriptHandlerFicha = GameObject.Find("Fichas_Container").GetComponent<HandlerFichas>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name != "MagnetFichas" && other.gameObject.GetComponent<FichaHUD>())
        {
            SoundContoller.Instance.PlayFxSound(5);
            string fichaNum;
            int aux = 0;
            if(other.gameObject.name != "all" && Convert.ToInt32(other.gameObject.name) >= 0 && Convert.ToInt32(other.gameObject.name) <= 12) 
            {
                scriptHandlerFicha.FichaAll = RoundController.Instance.GetCashTotal();
                fichaNum = (String)other.gameObject.name;
                aux = Convert.ToInt32(fichaNum);
            }
            scriptHandlerFicha.ChangeSelectedIndexFichas(aux);
        }
    }
}
