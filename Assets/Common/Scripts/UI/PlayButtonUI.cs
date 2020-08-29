using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButtonUI : MonoBehaviour
{
    // Press play and start the roullete

    public void onClick()
    {
        // StartRoullete
        manejador_ruleta.Instance.start_giro(generateRandomNumber());
    }

    private int generateRandomNumber()
    {
        int aux = 1;
        return aux;
    }
}
