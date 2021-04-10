using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace GameServices 
{
    public class ServiceManager : Singlenton<ServiceManager>
    {
        private List<AServices> _servicesManager = new List<AServices>();
        public static ADServices Ads { get; private set; }
        public static AnalyticsServices Analytics { get; private set; }
        public static IAPManager Purchase { get; private set; }

        public EventAds.EventRewardVideo OnRewardVideoFinished;
        public EventAds.EventRewardShop OnRewardShopFinished;

        private void Start()
        {
            InitializeServices();
            DontDestroyOnLoad(gameObject);
        }

        private void InitializeServices()
        {
            Ads = GetComponent<ADServices>();
            Analytics = GetComponent<AnalyticsServices>();
            Purchase = GetComponent<IAPManager>();

            _servicesManager.Add(Analytics);
            _servicesManager.Add(Ads);
            _servicesManager.Add(Purchase);

            foreach (AServices service in _servicesManager)
            {
                service.Initialize();
            }
        }
    }
}
