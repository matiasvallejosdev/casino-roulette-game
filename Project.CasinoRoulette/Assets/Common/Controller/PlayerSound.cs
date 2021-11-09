using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViewModel;
using UniRx;
using System;
using System.Threading.Tasks;

namespace Controllers
{
    public class PlayerSound : Singlenton<PlayerSound>
    {
        public GameSound gameSound;
        public AudioSource _audioSourceFx;
        public AudioSource _audioSourceMusic;
        
        private bool _isPlayingFx = false;

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
            _audioSourceMusic.volume = gameSound.musicVolume;
        }
        private void OnFxIs(bool value)
        {
            _audioSourceFx.mute = !value;
            _audioSourceFx.volume = gameSound.fxVolume;
        }

        private void OnMusic(int pos)
        {
            _audioSourceMusic.clip = gameSound.musicFx[pos];
            _audioSourceMusic.loop = true;
            _audioSourceMusic.Play();
        }
        private async void OnSound(int pos)
        {
            if(_isPlayingFx)
                return;

            await PlaySound(pos);

            _isPlayingFx = false;
        }

        private async Task PlaySound(int pos)
        {
            _isPlayingFx = true;
            _audioSourceFx.clip = gameSound.soundFx[pos];
            _audioSourceFx.loop = false;
            _audioSourceFx.Play();

            await Task.Delay(TimeSpan.FromMilliseconds(200));
        }

        private void OnGameOpened()
        {
            gameSound.isMusicOn.Value = true;
            gameSound.isFxOn.Value = true;

            gameSound.OnMusic.OnNext(0);
        }
    }
}
