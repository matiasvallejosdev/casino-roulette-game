using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Components;

namespace ViewModel
{
    public abstract class AudioEvent : ScriptableObject
    {
        public abstract void Play(AudioSource audio);
    }
}
