using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Components;

namespace ViewModel
{
    [CreateAssetMenu(fileName = "New Reward Fortune", menuName = "Scriptable/Reward Fortune")]
    public class RewardFortune : ScriptableObject
    {
        [Header("Fortune configuration")]
        public float totalAngle;
        public int sectionCount;
        public int[] payment;

        public bool isPlay;
        public bool hasInitialize;

        public ISubject<bool> OnFortune = new Subject<bool>();
    }
}
