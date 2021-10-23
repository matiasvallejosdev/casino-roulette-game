using System.Collections;
using System.Collections.Generic;
using Components;
using UnityEngine;
using ViewModel;

namespace Components
{
    public class UndoButtonUI : MonoBehaviour
    {
        public void OnClick() 
        {
            PlayerRound.Instance.DestroyLastChip();
        }
    }
}
