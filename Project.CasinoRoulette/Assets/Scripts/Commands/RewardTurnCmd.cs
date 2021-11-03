using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using ViewModel;
using Controllers;
using Infrastructure;
using System;

namespace Commands
{
    public class RewardTurnCmd : ICommand
    {
        public void Execute()
        {
            Debug.Log($"Opening game reward to get more money!");
        }
    }
}
