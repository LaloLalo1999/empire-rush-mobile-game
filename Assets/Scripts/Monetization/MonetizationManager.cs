using System;
using UnityEngine;
using EmpireRush.Core;
using EmpireRush.Economy;
using EmpireRush.Analytics;

namespace EmpireRush.Monetization
{
    public class MonetizationManager : MonoBehaviour
    {
        public static MonetizationManager Instance { get; private set; }
        
        [Header("Monetization Settings")]
        [SerializeField] private bool enableIAP = true;
        [SerializeField] private bool enableAds = true;
        [SerializeField] private bool debugMode = false;
        
        [Header("Ad Settings")]
        [SerializeField] private float adCooldownSeconds = 30f;
        [SerializeField] private int energyPerRewardedAd = 5;
        [SerializeField] private float incomeMultiplierDuration = 1800f; // 30 minutes
        [SerializeField] private float incomeMultiplierValue = 2f;
        
        // IAP System
        public IAPManager IAPManager { get; private set; }
        
        // Ad System
        public AdManager AdManager { get; private set; }
        
        // State tracking
        private DateTime lastRewardedAdTime;
        private DateTime incomeMultiplierEndTime;
        private bool hasIncomeMultiplier = false;
        
        // Events
        public static event Action<string> OnPurchaseCompleted;
        public static event Action<string> OnPurchaseFailed;
        public static event Action<string> OnAdWatched;
        public static event Action<string> OnAdFailed;
        public static event Action<bool> OnIncomeMultiplierChanged;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeMonetization();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeMonetization()
        {
            // Initialize IAP Manager
            if (enableIAP)
            {
                GameObject iapManagerObj = new GameObject("IAPManager");
                iapManagerObj.transform.SetParent(transform);
                IAPManager = iapManagerObj.AddComponent<IAPManager>();
            }
            
            // Initialize Ad Manager
            if (enableAds)
            {
                GameObject adManagerObj = new GameObject("AdManager");
                adManagerObj.transform.SetParent(transform);
                AdManager = adManagerObj.AddComponent<AdManager>();
            }
            
            if (debugMode)
                Debug.Log("MonetizationManager initialized");
        }
        
        private void Update()
        {
            UpdateIncomeMultiplier();
        }
        
        #region Income Multiplier Management
        
        private void UpdateIncomeMultiplier()
        {
            if (hasIncomeMultiplier && DateTime.Now >= incomeMultiplierEndTime)
            {
                hasIncomeMultiplier = false;
                OnIncomeMultiplierChanged?.Invoke(false);
                
                if (debugMode)
                    Debug.Log("Income multiplier expired");
            }
        }
        
        public void ActivateIncomeMultiplier(float durationSeconds)
        {
            hasIncomeMultiplier = true;
            incomeMultiplierEndTime = DateTime.Now.AddSeconds(durationSeconds);
            OnIncomeMultiplierChanged?.Invoke(true);
            
            AnalyticsManager.Instance?.TrackCustomEvent("income_multiplier_activated", new System.Collections.Generic.Dictionary<string, object>
            {
                { "duration", durationSeconds }
            });
            
            if (debugMode)
                Debug.Log($"Income multiplier activated for {durationSeconds} seconds");
        }
        
        public float GetIncomeMultiplier()
        {
            return hasIncomeMultiplier ? incomeMultiplierValue : 1f;
        }
        
        public float GetIncomeMultiplierTimeRemaining()
        {
            if (!hasIncomeMultiplier) return 0f;
            return (float)(incomeMultiplierEndTime - DateTime.Now).TotalSeconds;
        }
        
        #endregion
        
        #region Ad Integration
        
        public bool CanShowRewardedAd()
        {
            if (!enableAds || AdManager == null) return false;
            
            TimeSpan timeSinceLastAd = DateTime.Now - lastRewardedAdTime;
            return timeSinceLastAd.TotalSeconds >= adCooldownSeconds && AdManager.IsRewardedAdReady();
        }
        
        public void ShowRewardedAdForEnergy()
        {
            if (!CanShowRewardedAd()) return;
            
            AnalyticsManager.Instance?.TrackAdOffered("rewarded", "energy_depleted", true);
            AdManager?.ShowRewardedAd("energy_reward", OnEnergyAdCompleted);
        }
        
        public void ShowRewardedAdForIncomeBoost()
        {
            if (!CanShowRewardedAd()) return;
            
            AnalyticsManager.Instance?.TrackAdOffered("rewarded", "income_boost", true);
            AdManager?.ShowRewardedAd("income_boost", OnIncomeBoostAdCompleted);
        }
        
        public void ShowRewardedAdForOfflineBonus()
        {
            if (!CanShowRewardedAd()) return;
            
            AnalyticsManager.Instance?.TrackAdOffered("rewarded", "offline_bonus", true);
            AdManager?.ShowRewardedAd("offline_bonus", OnOfflineBonusAdCompleted);
        }
        
        private void OnEnergyAdCompleted(bool success)
        {
            if (success)
            {
                EconomyManager.Instance?.AddEnergy(energyPerRewardedAd);
                lastRewardedAdTime = DateTime.Now;
                
                AnalyticsManager.Instance?.TrackAdWatched("rewarded", "energy_depleted", true);
                OnAdWatched?.Invoke("energy_reward");
                
                if (debugMode)
                    Debug.Log($"Rewarded ad completed: +{energyPerRewardedAd} energy");
            }
            else
            {
                AnalyticsManager.Instance?.TrackAdWatched("rewarded", "energy_depleted", false);
                OnAdFailed?.Invoke("energy_reward");
                
                if (debugMode)
                    Debug.Log("Rewarded ad failed for energy");
            }
        }
        
        private void OnIncomeBoostAdCompleted(bool success)
        {
            if (success)
            {
                ActivateIncomeMultiplier(incomeMultiplierDuration);
                lastRewardedAdTime = DateTime.Now;
                
                AnalyticsManager.Instance?.TrackAdWatched("rewarded", "income_boost", true);
                OnAdWatched?.Invoke("income_boost");
                
                if (debugMode)
                    Debug.Log("Rewarded ad completed: Income multiplier activated");
            }
            else
            {
                AnalyticsManager.Instance?.TrackAdWatched("rewarded", "income_boost", false);
                OnAdFailed?.Invoke("income_boost");
                
                if (debugMode)
                    Debug.Log("Rewarded ad failed for income boost");
            }
        }
        
        private void OnOfflineBonusAdCompleted(bool success)
        {
            if (success)
            {
                // Double the last offline earnings (if any)
                // This would be implemented based on specific game requirements
                lastRewardedAdTime = DateTime.Now;
                
                AnalyticsManager.Instance?.TrackAdWatched("rewarded", "offline_bonus", true);
                OnAdWatched?.Invoke("offline_bonus");
                
                if (debugMode)
                    Debug.Log("Rewarded ad completed: Offline bonus doubled");
            }
            else
            {
                AnalyticsManager.Instance?.TrackAdWatched("rewarded", "offline_bonus", false);
                OnAdFailed?.Invoke("offline_bonus");
                
                if (debugMode)
                    Debug.Log("Rewarded ad failed for offline bonus");
            }
        }
        
        #endregion
        
        #region IAP Integration
        
        public void PurchaseEnergyPack(string productId)
        {
            if (!enableIAP || IAPManager == null) return;
            
            AnalyticsManager.Instance?.TrackPurchaseStarted(productId, GetProductPrice(productId), "USD");
            IAPManager.PurchaseProduct(productId, OnPurchaseSuccess, OnPurchaseFailure);
        }
        
        private void OnPurchaseSuccess(string productId)
        {
            // Handle successful purchase based on product ID
            switch (productId)
            {
                case "energy_pack_small":
                    EconomyManager.Instance?.AddEnergy(25);
                    break;
                case "energy_pack_medium":
                    EconomyManager.Instance?.AddEnergy(150);
                    break;
                case "energy_pack_large":
                    EconomyManager.Instance?.AddEnergy(750);
                    break;
                case "coin_pack_small":
                    EconomyManager.Instance?.AddCoins(10000);
                    break;
                case "coin_pack_medium":
                    EconomyManager.Instance?.AddCoins(75000);
                    break;
                case "coin_pack_large":
                    EconomyManager.Instance?.AddCoins(500000);
                    break;
                case "gem_pack_small":
                    EconomyManager.Instance?.AddGems(50);
                    break;
                case "gem_pack_medium":
                    EconomyManager.Instance?.AddGems(300);
                    break;
                case "gem_pack_large":
                    EconomyManager.Instance?.AddGems(1500);
                    break;
            }
            
            float price = GetProductPrice(productId);
            AnalyticsManager.Instance?.TrackPurchaseCompleted(productId, price, "USD");
            
            // Check if this is the first purchase
            if (!HasMadePurchase())
            {
                SetFirstPurchaseMade();
                AnalyticsManager.Instance?.TrackFirstPurchase(productId, price);
            }
            
            OnPurchaseCompleted?.Invoke(productId);
            
            if (debugMode)
                Debug.Log($"Purchase successful: {productId}");
        }
        
        private void OnPurchaseFailure(string productId, string error)
        {
            OnPurchaseFailed?.Invoke(productId);
            
            if (debugMode)
                Debug.Log($"Purchase failed: {productId} - {error}");
        }
        
        private float GetProductPrice(string productId)
        {
            // Return prices for analytics tracking
            return productId switch
            {
                "energy_pack_small" => 0.99f,
                "energy_pack_medium" => 4.99f,
                "energy_pack_large" => 19.99f,
                "coin_pack_small" => 0.99f,
                "coin_pack_medium" => 4.99f,
                "coin_pack_large" => 19.99f,
                "gem_pack_small" => 0.99f,
                "gem_pack_medium" => 4.99f,
                "gem_pack_large" => 19.99f,
                _ => 0f
            };
        }
        
        #endregion
        
        #region Purchase Tracking
        
        private bool HasMadePurchase()
        {
            return PlayerPrefs.GetInt("first_purchase_made", 0) == 1;
        }
        
        private void SetFirstPurchaseMade()
        {
            PlayerPrefs.SetInt("first_purchase_made", 1);
            PlayerPrefs.Save();
        }
        
        #endregion
        
        #region Save/Load Support
        
        public MonetizationData GetSaveData()
        {
            return new MonetizationData
            {
                lastRewardedAdTime = lastRewardedAdTime.ToBinary(),
                incomeMultiplierEndTime = incomeMultiplierEndTime.ToBinary(),
                hasIncomeMultiplier = hasIncomeMultiplier
            };
        }
        
        public void LoadSaveData(MonetizationData data)
        {
            lastRewardedAdTime = DateTime.FromBinary(data.lastRewardedAdTime);
            incomeMultiplierEndTime = DateTime.FromBinary(data.incomeMultiplierEndTime);
            hasIncomeMultiplier = data.hasIncomeMultiplier;
            
            // Validate income multiplier on load
            if (hasIncomeMultiplier && DateTime.Now >= incomeMultiplierEndTime)
            {
                hasIncomeMultiplier = false;
            }
        }
        
        #endregion
    }
    
    [System.Serializable]
    public class MonetizationData
    {
        public long lastRewardedAdTime;
        public long incomeMultiplierEndTime;
        public bool hasIncomeMultiplier;
    }
}