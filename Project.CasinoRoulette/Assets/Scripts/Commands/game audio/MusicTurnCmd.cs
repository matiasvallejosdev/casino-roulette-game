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
        private float value;

        public MusicTurnCmd(GameSound gameSound, bool isOn, float value)
        {
            this.gameSound = gameSound;
            this.isOn = isOn;
            this.value = value;
        }

        public void Execute()
        {
            if(value > 0)
                gameSound.musicVolume = value;

            gameSound.isMusicOn.Value = isOn;
        }
    }
}
