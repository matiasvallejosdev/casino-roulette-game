using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    public class PlayerSound : Singlenton<PlayerSound>
    {
        [Header("Effect Sounds")]
        public AudioClip[] fx;
        public AudioClip[] music;

        private AudioSource _fx;
        private AudioSource _music;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            _music = this.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<AudioSource>();
            _fx = this.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<AudioSource>();
        }

        public bool GetMusicStatus()
        {
            return _music.mute;
        }
        public bool GetSoundStatus() 
        {
            return _fx.mute;
        }

        public void SetMusicMute(bool status) 
        {
            _music.mute = status;
        }
        public void SetSoundMute(bool status) 
        {
            _fx.mute = status;    
        }

        public void PlayFxSound(int pos)
        {
            _fx.clip = fx[pos];
            _fx.loop = false;
            _fx.Play();
        }

        public void PlayMusic(int pos) 
        {
            _music.clip = music[pos];
            _music.loop = true;
            _music.Play();
        }
    }
}
