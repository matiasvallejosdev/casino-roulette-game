using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{    
    public class EarnMoneyDisplay : MonoBehaviour
    {
        public float to = 0;
        public float speed = 0;

        private void OnEnable()
        {
            LeanTween.moveLocalY(gameObject, to, speed).setLoopPingPong();
        }
    }
}
