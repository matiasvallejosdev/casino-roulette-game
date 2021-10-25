using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using ViewModel;
using Controllers;

namespace Commands
{    
    public class UndoTurnCmd : ICommand
    {
        public void Execute()
        {
            Debug.Log("Undo chip table!");
            PlayerRound.Instance.DestroyLastChip();
        }
    }
}
