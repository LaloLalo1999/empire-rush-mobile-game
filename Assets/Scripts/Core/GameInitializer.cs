using UnityEngine;
using EmpireRush.Core;
using EmpireRush.Economy;
using EmpireRush.UI;
using EmpireRush.Analytics;
using EmpireRush.Monetization;

namespace EmpireRush.Core
{
    public class GameInitializer : MonoBehaviour
    {
        [Header("Core Managers")]
        [SerializeField] private GameObject gameManagerPrefab;
        [SerializeField] private GameObject economyManagerPrefab;
        [SerializeField] private GameObject saveSystemPrefab;
        [SerializeField] private GameObject analyticsManagerPrefab;
        [SerializeField] private GameObject monetizationManagerPrefab;
        [SerializeField] private GameObject uiManagerPrefab;
        
        [Header("Initialization Settings")]
        [SerializeField] private bool initializeInAwake = true;
        [SerializeField] private bool debugMode = false;
        
        private void Awake()
        {
            if (initializeInAwake)
            {
                InitializeGame();
            }
        }
        
        public void InitializeGame()
        {
            if (debugMode)
                Debug.Log("Starting game initialization...");
            
            // Initialize core managers in order
            InitializeGameManager();
            InitializeSaveSystem();
            InitializeEconomyManager();
            InitializeAnalyticsManager();
            InitializeMonetizationManager();
            InitializeUIManager();
            
            if (debugMode)
                Debug.Log("Game initialization complete!");
        }
        
        private void InitializeGameManager()
        {
            if (GameManager.Instance == null && gameManagerPrefab != null)
            {
                Instantiate(gameManagerPrefab);
                if (debugMode) Debug.Log("GameManager initialized");
            }
        }
        
        private void InitializeSaveSystem()
        {
            if (SaveSystem.Instance == null && saveSystemPrefab != null)
            {
                Instantiate(saveSystemPrefab);
                if (debugMode) Debug.Log("SaveSystem initialized");
            }
        }
        
        private void InitializeEconomyManager()
        {
            if (EconomyManager.Instance == null && economyManagerPrefab != null)
            {
                Instantiate(economyManagerPrefab);
                if (debugMode) Debug.Log("EconomyManager initialized");
            }
        }
        
        private void InitializeAnalyticsManager()
        {
            if (AnalyticsManager.Instance == null && analyticsManagerPrefab != null)
            {
                Instantiate(analyticsManagerPrefab);
                if (debugMode) Debug.Log("AnalyticsManager initialized");
            }
        }
        
        private void InitializeMonetizationManager()
        {
            if (MonetizationManager.Instance == null && monetizationManagerPrefab != null)
            {
                Instantiate(monetizationManagerPrefab);
                if (debugMode) Debug.Log("MonetizationManager initialized");
            }
        }
        
        private void InitializeUIManager()
        {
            if (UIManager.Instance == null && uiManagerPrefab != null)
            {
                Instantiate(uiManagerPrefab);
                if (debugMode) Debug.Log("UIManager initialized");
            }
        }
        
        #region Debug Methods
        
        [ContextMenu("Force Initialize")]
        public void ForceInitialize()
        {
            InitializeGame();
        }
        
        [ContextMenu("Reset Game Data")]
        public void ResetGameData()
        {
            if (SaveSystem.Instance != null)
            {
                SaveSystem.Instance.DeleteSave();
                Debug.Log("Game data reset!");
            }
        }
        
        [ContextMenu("Add Test Resources")]
        public void AddTestResources()
        {
            if (EconomyManager.Instance != null)
            {
                EconomyManager.Instance.AddCoins(100000);
                EconomyManager.Instance.AddEnergy(10);
                EconomyManager.Instance.AddGems(100);
                Debug.Log("Test resources added!");
            }
        }
        
        #endregion
    }
}