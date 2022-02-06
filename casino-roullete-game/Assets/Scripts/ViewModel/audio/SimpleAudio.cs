using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Components;
using System.Threading.Tasks;
using System;

namespace ViewModel
{
    [CreateAssetMenu(fileName = "NewSimpleAudio", menuName = "Scriptable/Simple Audio")]
    public class SimpleAudio : AudioEvent
    {
        public AudioClip clip;
        [Range(0,1)] public float volume;
        [Range(0,1)] public float delay;

        public async override void Play(AudioSource audio)
        {
            audio.volume = volume;
            audio.clip = clip;
            audio.Play();
            await Task.Delay(TimeSpan.FromMilliseconds(delay));
        }
    }
}
