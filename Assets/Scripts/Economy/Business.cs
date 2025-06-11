using System;
using UnityEngine;

namespace EmpireRush.Economy
{
    [System.Serializable]
    public class Business
    {
        public BusinessType Type { get; private set; }
        public string Name { get; private set; }
        public bool IsOwned { get; private set; }
        public int Level { get; private set; }
        
        private long baseCost;
        private long baseIncome;
        private float costMultiplier;
        
        public Business(BusinessType type, string name, long baseCost, long baseIncome, float costMultiplier)
        {
            Type = type;
            Name = name;
            this.baseCost = baseCost;
            this.baseIncome = baseIncome;
            this.costMultiplier = costMultiplier;
            IsOwned = false;
            Level = 0;
        }
        
        public long GetPurchaseCost()
        {
            return baseCost;
        }
        
        public long GetUpgradeCost()
        {
            if (!IsOwned) return 0;
            return (long)(baseCost * Mathf.Pow(costMultiplier, Level));
        }
        
        public long GetIncomePerSecond()
        {
            if (!IsOwned) return 0;
            return (long)(baseIncome * Mathf.Pow(1.2f, Level - 1));
        }
        
        public void Purchase()
        {
            if (!IsOwned)
            {
                IsOwned = true;
                Level = 1;
            }
        }
        
        public void Upgrade()
        {
            if (IsOwned)
            {
                Level++;
            }
        }
        
        public float GetUpgradeProgress()
        {
            // Returns 0-1 progress for visual upgrade timers (future feature)
            return 1f;
        }
        
        public BusinessData GetSaveData()
        {
            return new BusinessData
            {
                type = Type,
                isOwned = IsOwned,
                level = Level
            };
        }
        
        public void LoadSaveData(BusinessData data)
        {
            if (data.type == Type)
            {
                IsOwned = data.isOwned;
                Level = data.level;
            }
        }
    }
    
    public enum BusinessType
    {
        LemonadeStand,
        PizzaShop,
        CarWash,
        Bank,
        TechCompany
    }
    
    [System.Serializable]
    public class BusinessData
    {
        public BusinessType type;
        public bool isOwned;
        public int level;
    }
}