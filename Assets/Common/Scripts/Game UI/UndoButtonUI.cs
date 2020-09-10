using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoButtonUI : MonoBehaviour
{
    public void UndoClick() 
    {
        RoundFichas.UndoFichasInTable();
    }
}
