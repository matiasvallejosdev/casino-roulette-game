using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

namespace GameServices 
{
    public class AnalyticsServices : MonoBehaviour, AServices
    {
        public void Initialize() 
        {
            AnalyticsEvent.GameStart();
            //Debug.Log("Analytic Service Manager booted up.");
        }
    }
}
