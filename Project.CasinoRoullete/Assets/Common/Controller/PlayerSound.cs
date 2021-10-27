using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViewModel;
using UniRx;
using System;

namespace Controllers
{
    public class PlayerSound : Singlenton<PlayerSound>
    {
        public GameSound gameSound;
        public AudioSource _audioSourceFx;
        public AudioSource _audioSourceMusic;
        
        public bool IsFxOn
        {
            get{ return gameSound.isFxOn.Value; }
            set{ gameSound.isFxOn.Value = value; }
        }
        public bool IsMusicOn
        {
            get{ return gameSound.isMusicOn.Value; }
            set{ gameSound.isMusicOn.Value = value; }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            
            gameSound.OnSound
                .Subscribe(OnSound)
                .AddTo(this);

            gameSound.OnMusic
                .Subscribe(OnMusic)
                .AddTo(this);
            
            gameSound.isFxOn
                .Subscribe(OnFxIs)
                .AddTo(this);
            
            gameSound.isMusicOn
                .Subscribe(OnMusicIs)
                .AddTo(this);

            OnGameOpened();
        }

        // Sound events
        private void OnMusicIs(bool value)
        {
            _audioSourceMusic.mute = !value;
        }
        private void OnFxIs(bool value)
        {
            _audioSourceFx.mute = !value;
        }

        private void OnMusic(int pos)
        {
            _audioSourceMusic.clip = gameSound.musicFx[pos];
            _audioSourceMusic.loop = true;
            _audioSourceMusic.Play();
        }
        private void OnSound(int pos)
        {
            _audioSourceFx.clip = gameSound.soundFx[pos];
            _audioSourceFx.loop = false;
            _audioSourceFx.Play();
        }

        private void OnGameOpened()
        {
            IsMusicOn = true;
            IsFxOn = true;

            gameSound.OnMusic.OnNext(0);
        }
    }
}
