using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CanvasHUD : MonoBehaviour
{

    public Text _txtSaldo;
    public Text _txtApuestas;

    public FichaDisplay fichaDisplay;

    public GameObject _backNumberHud;
    public GameObject _saldosHud;
    public GameObject _fichasHud;

    // Start is called before the first frame update
    void Start()
    {
        RoundController.Instance.OnRoundChanged.AddListener(HandleRoundStateChanged);
        RoundController.Instance.OnApuestaChanged.AddListener(HandleApuestaStateChanged);
    }

    public void OnWheelRotate(bool isRotate)
    {
        if(isRotate)
        {
            _fichasHud.SetActive(true);
            _backNumberHud.SetActive(true);
            _saldosHud.SetActive(true);
        } else 
        {
            _fichasHud.SetActive(false);
            _backNumberHud.SetActive(false);
            _saldosHud.SetActive(false);
        }
    } 
    void HandleRoundStateChanged(int saldoAnterior, int saldoNuevo)
    {
        _txtSaldo.text = saldoNuevo.ToString();
        //fichaDisplay = saldoNuevo;
    }
    void HandleApuestaStateChanged(int apuestaAnterior, int apuestaNueva, int apuestaTotal)
    {
        _txtApuestas.text = apuestaTotal.ToString();
    }
}
