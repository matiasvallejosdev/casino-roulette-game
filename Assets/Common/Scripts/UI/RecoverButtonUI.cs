using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverButtonUI : MonoBehaviour
{
    public void recoverFichas()
    {
        RoundController.Instance.RestorePreviousFichas(PaymentController.Instance._fichasPrevious.ToArray());
    }
}
