using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Components;

namespace ViewModel
{
    [CreateAssetMenu(fileName = "New Game Roullete", menuName = "Scriptable/Game Roullete")]
    public class GameRoullete : ScriptableObject
    {
        [Header("Wheel configuration")]
        public GameObject sphere;
        [Range(0,1000)] public float defaultSpeed = 0;
        public float currentSpeed;
        
        public ISubject<int> OnNumber = new Subject<int>();
        public ISubject<bool> OnRotate = new Subject<bool>();
    }
}
