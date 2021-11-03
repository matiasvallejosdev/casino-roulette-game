using System.Collections;
using System.Collections.Generic;
using Commands;
using UnityEngine;
using ViewModel;
using UniRx;
using System;
using UnityEngine.UI;

namespace Components
{
    public class GameMusicImageDisplay : MonoBehaviour
    {
        public GameSound gameSound;
        public Image imageDisplay;
        public Sprite[] imageOnOff;
        
        void Start()
        {
            gameSound.isMusicOn
                .Subscribe(OnMusic)
                .AddTo(this);
        }

        private void OnMusic(bool isOn)
        {
            Sprite i = isOn == true ? imageOnOff[0] : imageOnOff[1];
            imageDisplay.sprite = i;
        }
    }
}
