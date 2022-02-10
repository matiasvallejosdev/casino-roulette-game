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
    public class MusicTurnCmd : ICommand
    {
        private GameSound gameSound;
        private bool isOn;

        public MusicTurnCmd(GameSound gameSound, bool isOn)
        {
            this.isOn = isOn;
        }

        public void Execute()
        {
            gameSound.isMusicOn.Value = isOn;
        }
    }
}
