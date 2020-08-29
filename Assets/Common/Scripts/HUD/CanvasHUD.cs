using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CanvasHUD : MonoBehaviour
{

    public Text _txtSaldo;
    public Text _txtApuestas;

    public fichas _all;

    // Start is called before the first frame update
    void Start()
    {
        RoundController.Instance.OnRoundChanged.AddListener(HandleRoundStateChanged);
        RoundController.Instance.OnApuestaChanged.AddListener(HandleApuestaStateChanged);
    }

    void HandleRoundStateChanged(int saldoAnterior, int saldoNuevo)
    {
        _txtSaldo.text = saldoNuevo.ToString();
        _all._valor = saldoNuevo;
    }
    void HandleApuestaStateChanged(int apuestaAnterior, int apuestaNueva, int apuestaTotal)
    {
        _txtApuestas.text = apuestaTotal.ToString();
    }
}
