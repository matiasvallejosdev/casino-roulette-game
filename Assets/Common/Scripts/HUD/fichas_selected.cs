using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fichas_selected : MonoBehaviour
{
    private manejador_fichas _scManejadorFichas;
    // Start is called before the first frame update
    void Start()
    {
        _scManejadorFichas = GameObject.Find("Fichas_Container").GetComponent<manejador_fichas>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name != "MagnetFichas" && other.gameObject.GetComponent<FichaHUD>())
        {
            SoundContoller.Instance.fx_sound(7);
            string fichaNum;
            int aux = 0;
            if(other.gameObject.name != "all" && Convert.ToInt32(other.gameObject.name) >= 0 && Convert.ToInt32(other.gameObject.name) <= 12) 
            {
                _scManejadorFichas.valueOfFichaAll = RoundController.Instance.GetCashTotal();
                fichaNum = (String)other.gameObject.name;
                aux = Convert.ToInt32(fichaNum);
            }
            _scManejadorFichas.num_ficha(aux);
        }
    }
}
