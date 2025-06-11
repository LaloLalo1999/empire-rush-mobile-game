using System.Collections.Generic;
using UnityEngine;

namespace EmpireRush.Analytics
{
    public class AnalyticsManager : MonoBehaviour
    {
        public static AnalyticsManager Instance { get; private set; }
        
        [Header("Analytics Settings")]
        [SerializeField] private bool enableAnalytics = true;
        [SerializeField] private bool debugMode = false;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void Initialize()
        {
            if (!enableAnalytics) return;
            
            // TODO: Initialize Firebase Analytics
            // FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            //     FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            // });
            
            if (debugMode)
                Debug.Log("Analytics Manager initialized");
        }
        
        #region Core Game Events
        
        public void TrackTutorialStep(int step)
        {
            if (!enableAnalytics) return;
            
            var parameters = new Dictionary<string, object>
            {
                { "step", step }
            };
            
            LogEvent("tutorial_step", parameters);
        }
        
        public void TrackTutorialComplete()
        {
            if (!enableAnalytics) return;
            LogEvent("tutorial_complete");
        }
        
        public void TrackSpinWheel(string rewardType, long rewardAmount)
        {
            if (!enableAnalytics) return;
            
            var parameters = new Dictionary<string, object>
            {
                { "reward_type", rewardType },
                { "reward_amount", rewardAmount }
            };
            
            LogEvent("spin_wheel", parameters);
        }
        
        public void TrackBusinessPurchased(string businessType, long cost)
        {
            if (!enableAnalytics) return;
            
            var parameters = new Dictionary<string, object>
            {
                { "business_type", businessType },
                { "cost", cost }
            };
            
            LogEvent("business_purchased", parameters);
        }
        
        public void TrackBusinessUpgraded(string businessType, int level, long cost)
        {
            if (!enableAnalytics) return;
            
            var parameters = new Dictionary<string, object>
            {
                { "business_type", businessType },
                { "level", level },
                { "cost", cost }
            };
            
            LogEvent("business_upgraded", parameters);
        }
        
        public void TrackOfflineEarnings(long earnings, float offlineMinutes)
        {
            if (!enableAnalytics) return;
            
            var parameters = new Dictionary<string, object>
            {
                { "earnings", earnings },
                { "offline_minutes", offlineMinutes }
            };
            
            LogEvent("offline_earnings", parameters);
        }
        
        #endregion
        
        #region Monetization Events
        
        public void TrackPurchaseStarted(string itemId, float price, string currency)
        {
            if (!enableAnalytics) return;
            
            var parameters = new Dictionary<string, object>
            {
                { "item_id", itemId },
                { "price", price },
                { "currency", currency }
            };
            
            LogEvent("purchase_started", parameters);
        }
        
        public void TrackPurchaseCompleted(string itemId, float price, string currency)
        {
            if (!enableAnalytics) return;
            
            var parameters = new Dictionary<string, object>
            {
                { "item_id", itemId },
                { "price", price },
                { "currency", currency }
            };
            
            LogEvent("purchase_completed", parameters);
        }
        
        public void TrackFirstPurchase(string itemId, float price)
        {
            if (!enableAnalytics) return;
            
            var parameters = new Dictionary<string, object>
            {
                { "item_id", itemId },
                { "price", price }
            };
            
            LogEvent("first_purchase", parameters);
        }
        
        public void TrackAdWatched(string adType, string placement, bool completed)
        {
            if (!enableAnalytics) return;
            
            var parameters = new Dictionary<string, object>
            {
                { "ad_type", adType },
                { "placement", placement },
                { "completed", completed }
            };
            
            LogEvent("ad_watched", parameters);
        }
        
        public void TrackAdOffered(string adType, string placement, bool accepted)
        {
            if (!enableAnalytics) return;
            
            var parameters = new Dictionary<string, object>
            {
                { "ad_type", adType },
                { "placement", placement },
                { "accepted", accepted }
            };
            
            LogEvent("ad_offered", parameters);
        }
        
        #endregion
        
        #region Resource Events
        
        public void TrackResourceGained(string resourceType, long amount)
        {
            if (!enableAnalytics) return;
            
            var parameters = new Dictionary<string, object>
            {
                { "resource_type", resourceType },
                { "amount", amount }
            };
            
            LogEvent("resource_gained", parameters);
        }
        
        public void TrackResourceSpent(string resourceType, long amount)
        {
            if (!enableAnalytics) return;
            
            var parameters = new Dictionary<string, object>
            {
                { "resource_type", resourceType },
                { "amount", amount }
            };
            
            LogEvent("resource_spent", parameters);
        }
        
        #endregion
        
        #region Session Events
        
        public void TrackSessionStart()
        {
            if (!enableAnalytics) return;
            LogEvent("session_start");
        }
        
        public void TrackSessionEnd(float sessionLength)
        {
            if (!enableAnalytics) return;
            
            var parameters = new Dictionary<string, object>
            {
                { "session_length", sessionLength }
            };
            
            LogEvent("session_end", parameters);
        }
        
        public void TrackLevelStart(int level)
        {
            if (!enableAnalytics) return;
            
            var parameters = new Dictionary<string, object>
            {
                { "level", level }
            };
            
            LogEvent("level_start", parameters);
        }
        
        public void TrackLevelComplete(int level, float timeSpent)
        {
            if (!enableAnalytics) return;
            
            var parameters = new Dictionary<string, object>
            {
                { "level", level },
                { "time_spent", timeSpent }
            };
            
            LogEvent("level_complete", parameters);
        }
        
        #endregion
        
        #region Custom Events
        
        public void TrackCustomEvent(string eventName, Dictionary<string, object> parameters = null)
        {
            if (!enableAnalytics) return;
            LogEvent(eventName, parameters);
        }
        
        #endregion
        
        #region User Properties
        
        public void SetUserProperty(string name, string value)
        {
            if (!enableAnalytics) return;
            
            // TODO: Implement Firebase Analytics user properties
            // FirebaseAnalytics.SetUserProperty(name, value);
            
            if (debugMode)
                Debug.Log($"User property set: {name} = {value}");
        }
        
        public void SetUserID(string userID)
        {
            if (!enableAnalytics) return;
            
            // TODO: Implement Firebase Analytics user ID
            // FirebaseAnalytics.SetUserId(userID);
            
            if (debugMode)
                Debug.Log($"User ID set: {userID}");
        }
        
        #endregion
        
        #region Private Methods
        
        private void LogEvent(string eventName, Dictionary<string, object> parameters = null)
        {
            if (!enableAnalytics) return;
            
            // TODO: Implement Firebase Analytics event logging
            // FirebaseAnalytics.LogEvent(eventName, parameters?.ToFirebaseParameters());
            
            if (debugMode)
            {
                string paramString = "";
                if (parameters != null)
                {
                    paramString = " with parameters: " + string.Join(", ", parameters);
                }
                Debug.Log($"Analytics Event: {eventName}{paramString}");
            }
        }
        
        #endregion
    }
}