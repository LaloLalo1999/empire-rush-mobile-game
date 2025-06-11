using System;
using System.Collections.Generic;
using UnityEngine;

namespace EmpireRush.Economy
{
    public class EconomyManager : MonoBehaviour
    {
        public static EconomyManager Instance { get; private set; }
        
        [Header("Starting Resources")]
        [SerializeField] private long startingCoins = 100;
        [SerializeField] private int startingEnergy = 5;
        [SerializeField] private int startingGems = 0;
        
        [Header("Energy Settings")]
        [SerializeField] private int maxEnergy = 10;
        [SerializeField] private float energyRegenTimeMinutes = 5f;
        [SerializeField] private int energyPerSpin = 1;
        
        [Header("Offline Settings")]
        [SerializeField] private float maxOfflineHours = 4f;
        
        // Current resources
        public long Coins { get; private set; }
        public int Energy { get; private set; }
        public int Gems { get; private set; }
        
        // Business system
        public List<Business> Businesses { get; private set; }
        
        // Events
        public static event Action<long> OnCoinsChanged;
        public static event Action<int> OnEnergyChanged;
        public static event Action<int> OnGemsChanged;
        public static event Action<Business> OnBusinessPurchased;
        public static event Action<Business> OnBusinessUpgraded;
        public static event Action<long> OnOfflineEarningsCalculated;
        
        private DateTime lastEnergyRegenTime;
        private DateTime lastSaveTime;
        
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
            InitializeBusinesses();
            
            // Set starting resources if new game
            if (SaveSystem.Instance.IsNewGame)
            {
                SetCoins(startingCoins);
                SetEnergy(startingEnergy);
                SetGems(startingGems);
                lastEnergyRegenTime = DateTime.Now;
                lastSaveTime = DateTime.Now;
            }
            
            StartCoroutine(EnergyRegenCoroutine());
            StartCoroutine(PassiveIncomeCoroutine());
        }
        
        private void InitializeBusinesses()
        {
            Businesses = new List<Business>
            {
                new Business(BusinessType.LemonadeStand, "Lemonade Stand", 15, 1, 1.15f),
                new Business(BusinessType.PizzaShop, "Pizza Shop", 100, 8, 1.15f),
                new Business(BusinessType.CarWash, "Car Wash", 1100, 47, 1.15f),
                new Business(BusinessType.Bank, "Bank", 12000, 260, 1.15f),
                new Business(BusinessType.TechCompany, "Tech Company", 130000, 1400, 1.15f)
            };
        }
        
        #region Resource Management
        
        public void AddCoins(long amount)
        {
            Coins += amount;
            OnCoinsChanged?.Invoke(Coins);
            AnalyticsManager.Instance?.TrackResourceGained("coins", amount);
        }
        
        public void SetCoins(long amount)
        {
            Coins = amount;
            OnCoinsChanged?.Invoke(Coins);
        }
        
        public bool SpendCoins(long amount)
        {
            if (Coins >= amount)
            {
                Coins -= amount;
                OnCoinsChanged?.Invoke(Coins);
                AnalyticsManager.Instance?.TrackResourceSpent("coins", amount);
                return true;
            }
            return false;
        }
        
        public void AddEnergy(int amount)
        {
            Energy = Mathf.Min(Energy + amount, maxEnergy);
            OnEnergyChanged?.Invoke(Energy);
            AnalyticsManager.Instance?.TrackResourceGained("energy", amount);
        }
        
        public void SetEnergy(int amount)
        {
            Energy = Mathf.Min(amount, maxEnergy);
            OnEnergyChanged?.Invoke(Energy);
        }
        
        public bool SpendEnergy(int amount = 1)
        {
            if (Energy >= amount)
            {
                Energy -= amount;
                OnEnergyChanged?.Invoke(Energy);
                AnalyticsManager.Instance?.TrackResourceSpent("energy", amount);
                return true;
            }
            return false;
        }
        
        public void AddGems(int amount)
        {
            Gems += amount;
            OnGemsChanged?.Invoke(Gems);
            AnalyticsManager.Instance?.TrackResourceGained("gems", amount);
        }
        
        public bool SpendGems(int amount)
        {
            if (Gems >= amount)
            {
                Gems -= amount;
                OnGemsChanged?.Invoke(Gems);
                AnalyticsManager.Instance?.TrackResourceSpent("gems", amount);
                return true;
            }
            return false;
        }
        
        #endregion
        
        #region Business System
        
        public bool PurchaseBusiness(BusinessType businessType)
        {
            Business business = GetBusiness(businessType);
            if (business == null || business.IsOwned) return false;
            
            long cost = business.GetPurchaseCost();
            if (SpendCoins(cost))
            {
                business.Purchase();
                OnBusinessPurchased?.Invoke(business);
                AnalyticsManager.Instance?.TrackBusinessPurchased(businessType.ToString(), cost);
                return true;
            }
            return false;
        }
        
        public bool UpgradeBusiness(BusinessType businessType)
        {
            Business business = GetBusiness(businessType);
            if (business == null || !business.IsOwned) return false;
            
            long cost = business.GetUpgradeCost();
            if (SpendCoins(cost))
            {
                business.Upgrade();
                OnBusinessUpgraded?.Invoke(business);
                AnalyticsManager.Instance?.TrackBusinessUpgraded(businessType.ToString(), business.Level, cost);
                return true;
            }
            return false;
        }
        
        public Business GetBusiness(BusinessType businessType)
        {
            return Businesses?.Find(b => b.Type == businessType);
        }
        
        public long GetTotalIncomePerSecond()
        {
            long totalIncome = 0;
            foreach (var business in Businesses)
            {
                if (business.IsOwned)
                    totalIncome += business.GetIncomePerSecond();
            }
            return totalIncome;
        }
        
        #endregion
        
        #region Offline Earnings
        
        public void CalculateOfflineEarnings(TimeSpan offlineTime)
        {
            double offlineSeconds = offlineTime.TotalSeconds;
            double maxOfflineSeconds = maxOfflineHours * 3600;
            double effectiveOfflineTime = Math.Min(offlineSeconds, maxOfflineSeconds);
            
            long totalIncomePerSecond = GetTotalIncomePerSecond();
            long offlineEarnings = (long)(totalIncomePerSecond * effectiveOfflineTime);
            
            if (offlineEarnings > 0)
            {
                AddCoins(offlineEarnings);
                OnOfflineEarningsCalculated?.Invoke(offlineEarnings);
                AnalyticsManager.Instance?.TrackOfflineEarnings(offlineEarnings, (float)offlineTime.TotalMinutes);
            }
        }
        
        #endregion
        
        #region Coroutines
        
        private System.Collections.IEnumerator EnergyRegenCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(60f); // Check every minute
                
                if (Energy < maxEnergy)
                {
                    TimeSpan timeSinceLastRegen = DateTime.Now - lastEnergyRegenTime;
                    int energyToAdd = (int)(timeSinceLastRegen.TotalMinutes / energyRegenTimeMinutes);
                    
                    if (energyToAdd > 0)
                    {
                        AddEnergy(energyToAdd);
                        lastEnergyRegenTime = DateTime.Now;
                    }
                }
            }
        }
        
        private System.Collections.IEnumerator PassiveIncomeCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                
                long incomePerSecond = GetTotalIncomePerSecond();
                if (incomePerSecond > 0)
                {
                    AddCoins(incomePerSecond);
                }
            }
        }
        
        #endregion
        
        #region Save/Load Support
        
        public EconomyData GetSaveData()
        {
            return new EconomyData
            {
                coins = Coins,
                energy = Energy,
                gems = Gems,
                lastEnergyRegenTime = lastEnergyRegenTime.ToBinary(),
                lastSaveTime = DateTime.Now.ToBinary(),
                businesses = Businesses.ConvertAll(b => b.GetSaveData())
            };
        }
        
        public void LoadSaveData(EconomyData data)
        {
            SetCoins(data.coins);
            SetEnergy(data.energy);
            Gems = data.gems;
            lastEnergyRegenTime = DateTime.FromBinary(data.lastEnergyRegenTime);
            lastSaveTime = DateTime.FromBinary(data.lastSaveTime);
            
            // Load business data
            if (data.businesses != null && data.businesses.Count == Businesses.Count)
            {
                for (int i = 0; i < Businesses.Count; i++)
                {
                    Businesses[i].LoadSaveData(data.businesses[i]);
                }
            }
            
            // Calculate offline earnings
            TimeSpan offlineTime = DateTime.Now - lastSaveTime;
            if (offlineTime.TotalMinutes > 1) // Only if offline for more than 1 minute
            {
                CalculateOfflineEarnings(offlineTime);
            }
            
            // Regenerate energy based on offline time
            TimeSpan energyOfflineTime = DateTime.Now - lastEnergyRegenTime;
            int energyToAdd = (int)(energyOfflineTime.TotalMinutes / energyRegenTimeMinutes);
            if (energyToAdd > 0)
            {
                AddEnergy(energyToAdd);
                lastEnergyRegenTime = DateTime.Now;
            }
        }
        
        #endregion
    }
    
    [System.Serializable]
    public class EconomyData
    {
        public long coins;
        public int energy;
        public int gems;
        public long lastEnergyRegenTime;
        public long lastSaveTime;
        public List<BusinessData> businesses;
    }
}