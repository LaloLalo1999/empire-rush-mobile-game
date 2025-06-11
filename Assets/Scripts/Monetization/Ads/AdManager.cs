using System;
using System.Collections;
using UnityEngine;

namespace EmpireRush.Monetization
{
    public class AdManager : MonoBehaviour
    {
        [Header("Ad Settings")]
        [SerializeField] private bool enableAds = true;
        [SerializeField] private bool enableInterstitials = true;
        [SerializeField] private bool enableRewarded = true;
        [SerializeField] private bool enableBanners = false;
        [SerializeField] private bool debugMode = false;
        
        [Header("Ad Frequency")]
        [SerializeField] private float interstitialCooldown = 120f; // 2 minutes
        [SerializeField] private int minSessionsBeforeInterstitial = 3;
        
        private bool isInitialized = false;
        private bool isRewardedAdReady = false;
        private bool isInterstitialAdReady = false;
        private DateTime lastInterstitialTime;
        private int sessionCount = 0;
        
        // Ad callbacks
        private Action<bool> currentRewardedCallback;
        private string currentRewardedContext;
        
        public static event Action OnAdsInitialized;
        public static event Action<string> OnAdsInitializationFailed;
        public static event Action<string, bool> OnRewardedAdCompleted;
        public static event Action OnInterstitialAdShown;
        
        private void Start()
        {
            InitializeAds();
        }
        
        private void InitializeAds()
        {
            if (!enableAds)
            {
                if (debugMode)
                    Debug.Log("Ads are disabled");
                return;
            }
            
            sessionCount = PlayerPrefs.GetInt("session_count", 0);
            
            // TODO: Initialize Unity Ads
            // Advertisement.Initialize("your_game_id", testMode: debugMode, this);
            
            // For now, simulate successful initialization
            StartCoroutine(SimulateInitialization());
        }
        
        private IEnumerator SimulateInitialization()
        {
            yield return new WaitForSeconds(1f);
            
            isInitialized = true;
            isRewardedAdReady = true;
            isInterstitialAdReady = true;
            
            OnAdsInitialized?.Invoke();
            
            if (debugMode)
                Debug.Log("Ad system initialized (simulated)");
        }
        
        #region Rewarded Ads
        
        public bool IsRewardedAdReady()
        {
            if (!enableAds || !enableRewarded || !isInitialized)
                return false;
            
            // TODO: Check Unity Ads
            // return Advertisement.IsReady("Rewarded_Video");
            
            return isRewardedAdReady;
        }
        
        public void ShowRewardedAd(string context, Action<bool> callback)
        {
            if (!IsRewardedAdReady())
            {
                callback?.Invoke(false);
                return;
            }
            
            currentRewardedCallback = callback;
            currentRewardedContext = context;
            
            // TODO: Show Unity Ads
            // Advertisement.Show("Rewarded_Video", this);
            
            // Simulate ad showing
            StartCoroutine(SimulateRewardedAd());
        }
        
        private IEnumerator SimulateRewardedAd()
        {
            if (debugMode)
                Debug.Log($"Showing rewarded ad for context: {currentRewardedContext}");
            
            // Simulate ad duration
            yield return new WaitForSeconds(2f);
            
            // Simulate 95% completion rate
            bool completed = UnityEngine.Random.Range(0f, 1f) > 0.05f;
            
            OnRewardedAdCompleted?.Invoke(currentRewardedContext, completed);
            currentRewardedCallback?.Invoke(completed);
            
            // Set cooldown before next ad is ready
            isRewardedAdReady = false;
            yield return new WaitForSeconds(30f); // 30 second cooldown
            isRewardedAdReady = true;
            
            if (debugMode)
                Debug.Log($"Rewarded ad completed: {completed}");
        }
        
        #endregion
        
        #region Interstitial Ads
        
        public bool CanShowInterstitialAd()
        {
            if (!enableAds || !enableInterstitials || !isInitialized)
                return false;
            
            // Check session count
            if (sessionCount < minSessionsBeforeInterstitial)
                return false;
            
            // Check cooldown
            TimeSpan timeSinceLastInterstitial = DateTime.Now - lastInterstitialTime;
            if (timeSinceLastInterstitial.TotalSeconds < interstitialCooldown)
                return false;
            
            // TODO: Check Unity Ads
            // return Advertisement.IsReady("Interstitial");
            
            return isInterstitialAdReady;
        }
        
        public void ShowInterstitialAd(string placement = "default")
        {
            if (!CanShowInterstitialAd())
                return;
            
            // TODO: Show Unity Ads
            // Advertisement.Show("Interstitial", this);
            
            // Simulate interstitial ad
            StartCoroutine(SimulateInterstitialAd(placement));
        }
        
        private IEnumerator SimulateInterstitialAd(string placement)
        {
            if (debugMode)
                Debug.Log($"Showing interstitial ad for placement: {placement}");
            
            yield return new WaitForSeconds(1f);
            
            lastInterstitialTime = DateTime.Now;
            OnInterstitialAdShown?.Invoke();
            
            // Set cooldown before next ad is ready
            isInterstitialAdReady = false;
            yield return new WaitForSeconds(60f); // 1 minute cooldown
            isInterstitialAdReady = true;
        }
        
        #endregion
        
        #region Banner Ads
        
        public void ShowBannerAd()
        {
            if (!enableAds || !enableBanners || !isInitialized)
                return;
            
            // TODO: Show Unity Ads banner
            // Advertisement.Banner.Show("Banner");
            
            if (debugMode)
                Debug.Log("Banner ad shown (simulated)");
        }
        
        public void HideBannerAd()
        {
            if (!enableAds || !enableBanners)
                return;
            
            // TODO: Hide Unity Ads banner
            // Advertisement.Banner.Hide();
            
            if (debugMode)
                Debug.Log("Banner ad hidden (simulated)");
        }
        
        #endregion
        
        #region Session Management
        
        public void OnGameSessionStart()
        {
            sessionCount++;
            PlayerPrefs.SetInt("session_count", sessionCount);
            PlayerPrefs.Save();
            
            if (debugMode)
                Debug.Log($"Session started. Count: {sessionCount}");
        }
        
        public void OnGameSessionEnd()
        {
            // Opportunity to show interstitial ad
            if (CanShowInterstitialAd() && UnityEngine.Random.Range(0f, 1f) < 0.3f) // 30% chance
            {
                ShowInterstitialAd("session_end");
            }
        }
        
        #endregion
        
        #region Ad Revenue Optimization
        
        public void SetUserConsentForPersonalizedAds(bool hasConsent)
        {
            // TODO: Set GDPR/CCPA consent for Unity Ads
            // MetaData gdprMetaData = new MetaData("gdpr");
            // gdprMetaData.Set("consent", hasConsent);
            // Advertisement.SetMetaData(gdprMetaData);
            
            PlayerPrefs.SetInt("personalized_ads_consent", hasConsent ? 1 : 0);
            PlayerPrefs.Save();
            
            if (debugMode)
                Debug.Log($"Personalized ads consent: {hasConsent}");
        }
        
        public bool HasUserConsentForPersonalizedAds()
        {
            return PlayerPrefs.GetInt("personalized_ads_consent", 0) == 1;
        }
        
        #endregion
        
        #region Unity Ads Interface Implementation (TODO)
        
        // public void OnUnityAdsReady(string placementId)
        // {
        //     if (debugMode)
        //         Debug.Log($"Unity Ads ready: {placementId}");
        // }
        // 
        // public void OnUnityAdsDidError(string message)
        // {
        //     OnAdsInitializationFailed?.Invoke(message);
        //     
        //     if (debugMode)
        //         Debug.LogError($"Unity Ads error: {message}");
        // }
        // 
        // public void OnUnityAdsDidStart(string placementId)
        // {
        //     if (debugMode)
        //         Debug.Log($"Unity Ads started: {placementId}");
        // }
        // 
        // public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
        // {
        //     if (placementId == "Rewarded_Video")
        //     {
        //         bool completed = showResult == ShowResult.Finished;
        //         OnRewardedAdCompleted?.Invoke(currentRewardedContext, completed);
        //         currentRewardedCallback?.Invoke(completed);
        //     }
        //     else if (placementId == "Interstitial")
        //     {
        //         lastInterstitialTime = DateTime.Now;
        //         OnInterstitialAdShown?.Invoke();
        //     }
        //     
        //     if (debugMode)
        //         Debug.Log($"Unity Ads finished: {placementId} - {showResult}");
        // }
        
        #endregion
        
        #region Analytics Integration
        
        public void TrackAdImpression(string adType, string placement, float revenue = 0f)
        {
            Analytics.AnalyticsManager.Instance?.TrackCustomEvent("ad_impression", new System.Collections.Generic.Dictionary<string, object>
            {
                { "ad_type", adType },
                { "placement", placement },
                { "revenue", revenue }
            });
        }
        
        #endregion
    }
}