using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class restart_game : MonoBehaviour
{
    // Delete bet
    // Delete fichas
    // Delete previous numbers
    [SerializeField] private GameObject _fichasEnMesa = null;

    public void restartGame()
    {
        PlayerPayment.Instance.deleteFichasInPayment();
        deleteFichasInMesa();
    }

    private void deleteFichasInMesa()
    {
        int n = 0;
        while(n < 2)
        {
            GameObject aux = _fichasEnMesa.transform.GetChild(n).gameObject;
            if(aux.transform.childCount != 0)
            {
                for (int i = 0; i < aux.transform.childCount; i++)
                {
                    Destroy(aux.transform.GetChild(i).gameObject);
                }
            }
            n++;
        }
    }
}
