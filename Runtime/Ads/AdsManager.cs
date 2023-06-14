using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AC.Base;
using System;
#if ADMOB_AD
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
#endif
namespace AC.GameTool.Ads
{
    public class AdsManager : ServiceSingleton<AdsManager>
    {
        AdmobDataSetting _admobDataSetting;
#if ADMOB_AD
        //Banner
        private BannerView _bannerView;
        private InterstitialAd _interstitialAd;
        private RewardedAd _rewardAd;
        private RewardedInterstitialAd _rewardedInterstitialAd;
        private AppOpenAd _appOpenAd;
#endif
        private bool _isBannerLoaded, _isBannerLoading;
        //Inter
        
        int _faileRequestInterstitial;
        bool _isAvailableToShowInterstitial;
        //reward

        int _failedRequestReward;
        string _placementReward;
        Action _rewardAdSuccessed;
        Action<string> _rewardAdFailed;
        //Reward Inter
        
        string _placementRewardInter;
        Action _rewardAdSuccessedInter;
        Action<string> _rewardAdFailedInter;
        int _faileRequestRewardInterstitial;
        //Open App
        private DateTime loadOpenAppAdTime;
        
        private bool _isShowingAppOpenAd;

        // Start is called before the first frame update
        void Start()
        {
            _admobDataSetting = AdmobDataSetting.LoadInstance();
            if (_admobDataSetting == null)
                return;
#if ADMOB_AD
            MobileAds.SetiOSAppPauseOnBackground(true);
            List<string> testDevices = new List<string> { AdRequest.TestDeviceSimulator };
#if UNITY_ANDROID
            testDevices.AddRange(_admobDataSetting.AdmobData.Android.TestDevices);
#elif UNITY_IOS
            testDevices.AddRange(_admobDataSetting.AdmobData.IOS.TestDevices);
#endif

            RequestConfiguration.Builder builder = new RequestConfiguration.Builder();
            if (_admobDataSetting.AdmobData.IsTagForChild)
            {
                builder.SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.True);

            }
            builder.SetTestDeviceIds(testDevices);
            RequestConfiguration requestConfiguration = builder.build();
            MobileAds.SetRequestConfiguration(requestConfiguration);
            MobileAds.Initialize((initStatus) =>
            {
                Debug.Log("Init Status: " + initStatus.ToString());
                if(_admobDataSetting.AdmobData.AutoLoadBannerAdOnStartup)
                    LoaddingAdsBanner();
                RequestInterstitial();
                CreateAndLoadRewardedAd();
                RequestAndLoadRewardedInterstitialAd();
                LoadAppOpenAd();
                _isAvailableToShowInterstitial = false;
                KillAvailableToShowInterstitial_Timer();
                _availableToShowInterstitial_Timer = StartCoroutine(AvailableToShowInterstitial_Timer());
            });

            
            AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
#endif
            
        }

        private void OnDestroy()
        {
#if ADMOB_AD
            if (_bannerView != null)
                _bannerView.Destroy();
            if (_interstitialAd != null)
                _interstitialAd.Destroy();
            if (_rewardAd != null)
                _rewardAd.Destroy();
            if (_rewardedInterstitialAd != null)
                _rewardedInterstitialAd.Destroy();
            if (_appOpenAd != null)
                _appOpenAd.Destroy();
#endif
        }

        #region Banner Ads

        public void LoaddingAdsBanner()
        {
            if (!_isBannerLoaded && !_isBannerLoading)
            {
                RequestBanner();
            }
        }
        public void ShowAdsBanner(bool isShow)
        {
#if ADMOB_AD
            if (_bannerView != null && _isBannerLoaded)
            {
                if (isShow)
                {
                    _bannerView.Show();
                }
                else
                {
                    _bannerView.Hide();
                }
            }
#endif
        }

        void RequestBanner()
        {

#if UNITY_ANDROID
            string adUnitId =  _admobDataSetting.AdmobData.Android.BanerUnitID;
#elif UNITY_IOS
            string adUnitId = _admobDataSetting.AdmobData.IOS.BanerUnitID;;
#else
            string adUnitId = "unexpected_platform";
#endif
#if ADMOB_AD
            // Create a 320x50 banner at the top of the screen.
            _bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

            // Called when an ad request has successfully loaded.
            this._bannerView.OnAdLoaded += this.HandleOnBannerAdLoaded;
            // Called when an ad request failed to load.
            this._bannerView.OnAdFailedToLoad += this.HandleOnBannerAdFailedToLoad;
            // Called when an ad is clicked.
            this._bannerView.OnAdOpening += this.HandleOnBannerAdOpened;
            // Called when the user returned from the app after an ad click.
            this._bannerView.OnAdClosed += this.HandleOnBannerAdClosed;

            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();
            // Load the banner with the request.
            this._bannerView.LoadAd(request);
            _isBannerLoading = true;
#endif
        }
#if ADMOB_AD
        public void HandleOnBannerAdLoaded(object sender, EventArgs args)
        {
            _isBannerLoaded = true;
            _isBannerLoading = false;          
        }

        public void HandleOnBannerAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            _isBannerLoaded = false;
            _isBannerLoading = false;
        }

        public void HandleOnBannerAdOpened(object sender, EventArgs args)
        {
            
        }

        public void HandleOnBannerAdClosed(object sender, EventArgs args)
        {
            
        }
#endif
        #endregion

        #region Interstitial Ad

        public void ShowInterstitialAd()
        {
#if ADMOB_AD
            if (_isAvailableToShowInterstitial && _interstitialAd != null && _interstitialAd.IsLoaded())
            {
                _interstitialAd.Show();
                
            }
#endif
        }
        void KillAvailableToShowInterstitial_Timer()
        {
            if(_availableToShowInterstitial_Timer != null)
            {
                StopCoroutine(_availableToShowInterstitial_Timer);
                _availableToShowInterstitial_Timer = null;
            }
        }
        Coroutine _availableToShowInterstitial_Timer;
        IEnumerator AvailableToShowInterstitial_Timer()
        {
            yield return new WaitForSeconds(_admobDataSetting.AdmobData.TimeBetweenInterstitial);
            _isAvailableToShowInterstitial = true;
        }

        private void RequestInterstitial()
        {

#if UNITY_ANDROID
        string adUnitId = _admobDataSetting.AdmobData.Android.InterstitialUnitID;
#elif UNITY_IPHONE
        string adUnitId = _admobDataSetting.AdmobData.IOS.InterstitialUnitID;
#else
            string adUnitId = "unexpected_platform";
#endif
#if ADMOB_AD
            if (_interstitialAd != null)
            {
                _interstitialAd.Destroy();
            }
            // Initialize an InterstitialAd.
            _interstitialAd = new InterstitialAd(adUnitId);
            // Called when an ad request has successfully loaded.
            _interstitialAd.OnAdLoaded += HandleOnInterstitialAdLoaded;
            // Called when an ad request failed to load.
            _interstitialAd.OnAdFailedToLoad += HandleOnInterstitialAdFailedToLoad;
            // Called when an ad is shown.
            _interstitialAd.OnAdOpening += HandleOnInterstitialAdOpening;
            // Called when the ad is closed.
            _interstitialAd.OnAdClosed += HandleOnInterstitialAdClosed;
            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();
            // Load the interstitial with the request.
            _interstitialAd.LoadAd(request);
#endif
        }
#if ADMOB_AD
        public void HandleOnInterstitialAdLoaded(object sender, EventArgs args)
        {
            LogManager.Log("InterstitialAdLoaded");
            _faileRequestInterstitial = 0;
        }

        public void HandleOnInterstitialAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            LogManager.Log("InterstitialAdFailedToLoad");
            if (_faileRequestInterstitial < 20)
                _faileRequestInterstitial += 1;
            Invoke(nameof(RequestInterstitial), _faileRequestInterstitial * 5f);
        }

        public void HandleOnInterstitialAdOpening(object sender, EventArgs args)
        {
            LogManager.Log("InterstitialAdOpening");
        }

        public void HandleOnInterstitialAdClosed(object sender, EventArgs args)
        {
            RequestInterstitial();
            _isAvailableToShowInterstitial = false;
            KillAvailableToShowInterstitial_Timer();
            _availableToShowInterstitial_Timer = StartCoroutine(AvailableToShowInterstitial_Timer());
        }
#endif
        #endregion

        #region Reward Ads

        public void ShowAdsReward(string placement, Action successedCallback, Action<string> failedCallback)
        {
            _placementReward = placement;
            _rewardAdSuccessed = successedCallback;
            _rewardAdFailed = failedCallback;
#if ADMOB_AD
            if (_rewardAd != null)
            {
                _rewardAd.Show();
            }
            else
            {
                _rewardAdFailed?.Invoke("Reward Ads Null");
            }
#endif
        }

        void CreateAndLoadRewardedAd()
        {

#if UNITY_ANDROID
        string adUnitId = _admobDataSetting.AdmobData.Android.RewardUnitID;
#elif UNITY_IPHONE
            string adUnitId = _admobDataSetting.AdmobData.IOS.RewardUnitID;
#else
            string adUnitId = "unexpected_platform";
#endif
#if ADMOB_AD
            _rewardAd = new RewardedAd(adUnitId);

            // Called when an ad request has successfully loaded.
            _rewardAd.OnAdLoaded += HandleRewardedAdLoaded;
            // Called when an ad request failed to load.
            _rewardAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
            // Called when an ad is shown.
            _rewardAd.OnAdOpening += HandleRewardedAdOpening;
            // Called when an ad request failed to show.
            _rewardAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
            // Called when the user should be rewarded for interacting with the ad.
            _rewardAd.OnUserEarnedReward += HandleUserEarnedReward;
            // Called when the ad is closed.
            _rewardAd.OnAdClosed += HandleRewardedAdClosed;

            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();
            // Load the rewarded ad with the request.
            _rewardAd.LoadAd(request);
#endif
        }
#if ADMOB_AD
        public void HandleRewardedAdLoaded(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleRewardedAdLoaded event received");
            _failedRequestReward = 0;
        }

        public void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            MonoBehaviour.print(
                "HandleRewardedAdFailedToLoad event received with message: "
                                 + args.LoadAdError);
            if (_failedRequestReward < 20)
            {
                _failedRequestReward += 1;
            }
            Invoke(nameof(CreateAndLoadRewardedAd), _failedRequestReward * 4f);
        }

        public void HandleRewardedAdOpening(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleRewardedAdOpening event received");
        }

        public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
        {
            MonoBehaviour.print(
                "HandleRewardedAdFailedToShow event received with message: "
                                 + args.AdError.ToString());
            _rewardAdFailed?.Invoke(args.AdError.ToString());
        }

        public void HandleRewardedAdClosed(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleRewardedAdClosed event received");
            CreateAndLoadRewardedAd();
        }
        public void HandleUserEarnedReward(object sender, Reward args)
        {
            _rewardAdSuccessed?.Invoke();
        }
#endif
        #endregion

        #region Reward Interstitial Ad

        public void ShowRewardedInterstitialAd(string placement, Action successed, Action<string> failed)
        {
            _placementRewardInter = placement;
            _rewardAdSuccessedInter = successed;
            _rewardAdFailedInter = failed;
#if ADMOB_AD
            if (_rewardedInterstitialAd != null)
            {
                _rewardedInterstitialAd.Show(UserEarnedRewardCallback);
            }
            else
            {
                _rewardAdFailedInter?.Invoke("Reward Intertitial Null");
            }
#endif
        }
        void RequestAndLoadRewardedInterstitialAd()
        {
#if UNITY_ANDROID
        string adUnitId = _admobDataSetting.AdmobData.Android.RewardInterUnitID;
#elif UNITY_IPHONE
            string adUnitId = _admobDataSetting.AdmobData.IOS.RewardInterUnitID;
#else
            string adUnitId = "unexpected_platform";
#endif
#if ADMOB_AD
            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();
            // Load the rewarded ad with the request.
            RewardedInterstitialAd.LoadAd(adUnitId, request, AdLoadCallback);
#endif
        }
#if ADMOB_AD
        private void AdLoadCallback(RewardedInterstitialAd ad, AdFailedToLoadEventArgs error)
        {
            if (error == null)
            {
                _rewardedInterstitialAd = ad;
                _faileRequestRewardInterstitial = 0;
                _rewardedInterstitialAd.OnAdFailedToPresentFullScreenContent += HandleAdRewardInterFailedToPresent;
                _rewardedInterstitialAd.OnAdDidPresentFullScreenContent += HandleAdRewardInterDidPresent;
                _rewardedInterstitialAd.OnAdDidDismissFullScreenContent += HandleAdRewardInterDidDismiss;
                _rewardedInterstitialAd.OnPaidEvent += HandleRewardInterPaidEvent;
            }
            else
            {
                _rewardAdFailedInter?.Invoke("Failed to Load Reward Intertitial");
                if (_faileRequestRewardInterstitial < 20)
                    _faileRequestRewardInterstitial += 1;
                Invoke(nameof(RequestAndLoadRewardedInterstitialAd), _faileRequestRewardInterstitial * 5f);
            }
        }

        private void HandleAdRewardInterFailedToPresent(object sender, AdErrorEventArgs args)
        {
            LogManager.Log("Rewarded interstitial ad has failed to present.");
        }

        private void HandleAdRewardInterDidPresent(object sender, EventArgs args)
        {
            LogManager.Log("Rewarded interstitial ad has presented.");
        }

        private void HandleAdRewardInterDidDismiss(object sender, EventArgs args)
        {
            LogManager.Log("Rewarded interstitial ad has dismissed presentation.");
        }

        private void HandleRewardInterPaidEvent(object sender, AdValueEventArgs args)
        {
            LogManager.Log("Rewarded interstitial ad has received a paid event.");
        }

        private void UserEarnedRewardCallback(Reward reward)
        {
            // TODO: Reward the user.
            _rewardAdSuccessedInter?.Invoke();
            RequestAndLoadRewardedInterstitialAd();
        }
#endif
        #endregion

        #region Open App Ads


        public bool IsAppOpenAdAvailable
        {

            get
            {
#if ADMOB_AD
                return _appOpenAd != null && (System.DateTime.UtcNow - loadOpenAppAdTime).TotalHours < 4;
#else
    return false;
#endif
            }

        }

    public void LoadAppOpenAd()
        {

            if (IsAppOpenAdAvailable)
            {
                return;
            }
#if UNITY_ANDROID
        string adUnitId = _admobDataSetting.AdmobData.Android.OpenAppUnitID;
#elif UNITY_IOS
            string adUnitId = _admobDataSetting.AdmobData.IOS.OpenAppUnitID;;
#else
            string adUnitId = "unexpected_platform";
#endif
#if ADMOB_AD
            AdRequest request = new AdRequest.Builder().Build();
            AppOpenAd.LoadAd(adUnitId, ScreenOrientation.Portrait, request, ((appOpenAd, error) =>
            {
                if (error != null)
                {
                    Debug.LogFormat("Failed to load the ad. (reason: {0})", error.LoadAdError.GetMessage());
                    return;
                }

                _appOpenAd = appOpenAd;
                loadOpenAppAdTime = DateTime.UtcNow;
            }));
#endif
        }

        public void ShowAppOpenAdIfAvailable()
        {
            if (!IsAppOpenAdAvailable || _isShowingAppOpenAd)
            {
                return;
            }
#if ADMOB_AD
            _appOpenAd.OnAdDidDismissFullScreenContent += HandleAdDidDismissFullScreenContent;
            _appOpenAd.OnAdFailedToPresentFullScreenContent += HandleAdFailedToPresentFullScreenContent;
            _appOpenAd.OnAdDidPresentFullScreenContent += HandleAdDidPresentFullScreenContent;
            _appOpenAd.OnAdDidRecordImpression += HandleAdDidRecordImpression;
            _appOpenAd.OnPaidEvent += HandlePaidEvent;

            _appOpenAd.Show();
#endif
        }
#if ADMOB_AD
        private void HandleAdDidDismissFullScreenContent(object sender, EventArgs args)
        {
            Debug.Log("Closed app open ad");
            // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
            _appOpenAd = null;
            _isShowingAppOpenAd = false;
            LoadAppOpenAd();
        }

        private void HandleAdFailedToPresentFullScreenContent(object sender, AdErrorEventArgs args)
        {
            Debug.LogFormat("Failed to present the ad (reason: {0})", args.AdError.GetMessage());
            // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
            _appOpenAd = null;
            LoadAppOpenAd();
        }

        private void HandleAdDidPresentFullScreenContent(object sender, EventArgs args)
        {
            Debug.Log("Displayed app open ad");
            _isShowingAppOpenAd = true;
        }

        private void HandleAdDidRecordImpression(object sender, EventArgs args)
        {
            Debug.Log("Recorded ad impression");
        }

        private void HandlePaidEvent(object sender, AdValueEventArgs args)
        {
            Debug.LogFormat("Received paid event. (currency: {0}, value: {1}",
                    args.AdValue.CurrencyCode, args.AdValue.Value);
        }
        private void OnAppStateChanged(AppState state)
        {
            // Display the app open ad when the app is foregrounded.
            UnityEngine.Debug.Log("App State is " + state);
            if (state == AppState.Foreground)
            {
                ShowAppOpenAdIfAvailable();
            }
        }
#endif
        #endregion
    }

}
