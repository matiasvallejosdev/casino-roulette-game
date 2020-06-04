using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButtonUI : MonoBehaviour
{
    // Press play and start the roullete

    [SerializeField] private manejador_ruleta _wheelRoulleteSc;

    public void startRoullete()
    {
        _wheelRoulleteSc.start_giro(generateRandomNumber());
    }

    private int generateRandomNumber()
    {
        int aux = 31;
        return aux;
    }
}
