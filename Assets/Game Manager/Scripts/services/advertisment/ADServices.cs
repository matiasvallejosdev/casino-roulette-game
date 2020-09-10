using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Analytics;

namespace GameServices 
{
    public class ADServices : MonoBehaviour, AServices, IUnityAdsListener
    {
        //private static string appStoreID = "3676772";
        private static string playStoreID = "3676773";

        private static string interestitialAd = "video";
        private static string rewardedAd = "rewardedVideo";

        private bool isTestAd = true;

        private Dictionary<string, object> customEventParams = new Dictionary<string, object>();

        private void Start()
        {
            Advertisement.AddListener(this);
        }

        public void Initialize() 
        {
            #if UNITY_IOS || UNITY_ANDROID
            Initialize(playStoreID, isTestAd, false);
            //Debug.Log("Advertisment Service Manager booted up.");
            #else
            Debug.LogWarning("Current platform is not supported");
            #endif
        }
        public static void Initialize(string gameId, bool testMode, bool enablePerPlacementLoad) 
        {
            Advertisement.Initialize(gameId, testMode);
        }
        public void PlayInterstitialAd()
        {
            if (!Advertisement.IsReady(interestitialAd))
            {
                Debug.LogWarningFormat("Placement not ready to display");
                return;
            }
            else
            {
                Advertisement.Show(interestitialAd);
            }
        }
        public void PlayRewardedVideoAd() 
        {
            if (!Advertisement.IsReady(interestitialAd))
            {
                Debug.LogWarningFormat("Placement not ready to display");
                return;
            }
            else
            {
                Advertisement.Show(rewardedAd);
            }
        }

        public void OnUnityAdsReady(string placementId)
        {

        }

        public void OnUnityAdsDidError(string message)
        {
            //throw new System.NotImplementedException();
        }

        public void OnUnityAdsDidStart(string placementId)
        {
            //throw new System.NotImplementedException();
        }

        public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
        {
            switch (showResult) 
            {
                case ShowResult.Failed:
                    Debug.LogWarningFormat("Placement is Failed!");
                    break;
                case ShowResult.Skipped:
                    Debug.LogWarningFormat("Placement is Skipped!");
                    break;
                case ShowResult.Finished:
                    if(placementId == rewardedAd) 
                    {
                        ServiceManager.Instance.OnRewardVideoFinished.Invoke(true);
                    }
                    if(placementId == interestitialAd) 
                    {
                        Debug.Log("Finished interstitial");
                    }
                    break;
            }
        }
    }
}
