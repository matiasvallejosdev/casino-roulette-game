using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverButtonUI : MonoBehaviour
{
    public void recoverFichas()
    {
        RoundFichas.RestorePreviousRound(PaymentController.Instance._fichasPrevious.ToArray());
    }
}
