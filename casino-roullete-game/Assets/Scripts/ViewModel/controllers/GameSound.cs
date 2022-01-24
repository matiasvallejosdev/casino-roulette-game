using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Components;

namespace ViewModel
{
    [CreateAssetMenu(fileName = "New Game Sound", menuName = "Scriptable/Game Sound")]
    public class GameSound : ScriptableObject
    {
        public BoolReactiveProperty isFxOn;
        public BoolReactiveProperty isMusicOn;

        public AudioClip[] soundFx;
        public AudioClip[] musicFx;

        [Range(0,1)] public float fxVolume;
        [Range(0,1)] public float musicVolume;
        
        public ISubject<int> OnSound = new Subject<int>();
        public ISubject<int> OnMusic = new Subject<int>();
    }
}
