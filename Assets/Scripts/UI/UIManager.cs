using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EmpireRush.Core;
using EmpireRush.Economy;

namespace EmpireRush.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        
        [Header("Main UI Panels")]
        [SerializeField] private GameObject mainGamePanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject storePanel;
        [SerializeField] private GameObject businessPanel;
        [SerializeField] private GameObject offlineEarningsPanel;
        
        [Header("Resource Display")]
        [SerializeField] private Text coinsText;
        [SerializeField] private Text gemsText;
        [SerializeField] private EnergyUI energyUI;
        
        [Header("Business UI")]
        [SerializeField] private Transform businessContainer;
        [SerializeField] private GameObject businessUIPrefab;
        
        [Header("Notification System")]
        [SerializeField] private GameObject notificationPrefab;
        [SerializeField] private Transform notificationContainer;
        [SerializeField] private float notificationDuration = 3f;
        
        [Header("Offline Earnings")]
        [SerializeField] private Text offlineEarningsAmountText;
        [SerializeField] private Text offlineTimeText;
        [SerializeField] private Button collectOfflineButton;
        [SerializeField] private Button doubleOfflineButton;
        
        [Header("Buttons")]
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button storeButton;
        [SerializeField] private Button businessesButton;
        
        private List<BusinessUI> businessUIs = new List<BusinessUI>();
        private Queue<GameObject> notificationPool = new Queue<GameObject>();
        
        public static event Action OnUIInitialized;
        
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
        
        private void Start()
        {
            InitializeUI();
            SetupEvents();
            SetupButtons();
        }
        
        private void InitializeUI()
        {
            // Initialize resource displays
            UpdateResourceDisplays();
            
            // Create business UI elements
            CreateBusinessUIs();
            
            // Show main panel by default
            ShowPanel(mainGamePanel);
            
            OnUIInitialized?.Invoke();
        }
        
        private void SetupEvents()
        {
            // Economy events
            EconomyManager.OnCoinsChanged += OnCoinsChanged;
            EconomyManager.OnGemsChanged += OnGemsChanged;
            EconomyManager.OnBusinessPurchased += OnBusinessPurchased;
            EconomyManager.OnBusinessUpgraded += OnBusinessUpgraded;
            EconomyManager.OnOfflineEarningsCalculated += OnOfflineEarningsCalculated;
            
            // Game state events
            GameManager.OnGameStateChanged += OnGameStateChanged;
            
            // Spin wheel events
            SpinWheelController.OnRewardReceived += OnRewardReceived;
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            EconomyManager.OnCoinsChanged -= OnCoinsChanged;
            EconomyManager.OnGemsChanged -= OnGemsChanged;
            EconomyManager.OnBusinessPurchased -= OnBusinessPurchased;
            EconomyManager.OnBusinessUpgraded -= OnBusinessUpgraded;
            EconomyManager.OnOfflineEarningsCalculated -= OnOfflineEarningsCalculated;
            
            GameManager.OnGameStateChanged -= OnGameStateChanged;
            SpinWheelController.OnRewardReceived -= OnRewardReceived;
        }
        
        private void SetupButtons()
        {
            if (settingsButton != null)
                settingsButton.onClick.AddListener(() => ShowSettings());
                
            if (storeButton != null)
                storeButton.onClick.AddListener(() => ShowStore());
                
            if (businessesButton != null)
                businessesButton.onClick.AddListener(() => ShowBusinesses());
                
            if (collectOfflineButton != null)
                collectOfflineButton.onClick.AddListener(CollectOfflineEarnings);
                
            if (doubleOfflineButton != null)
                doubleOfflineButton.onClick.AddListener(DoubleOfflineEarnings);
        }
        
        #region Panel Management
        
        public void ShowPanel(GameObject panelToShow)
        {
            // Hide all panels
            if (mainGamePanel != null) mainGamePanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (storePanel != null) storePanel.SetActive(false);
            if (businessPanel != null) businessPanel.SetActive(false);
            
            // Show target panel
            if (panelToShow != null)
                panelToShow.SetActive(true);
        }
        
        public void ShowMainGame()
        {
            ShowPanel(mainGamePanel);
            GameManager.Instance?.ChangeGameState(GameManager.GameState.Playing);
        }
        
        public void ShowSettings()
        {
            ShowPanel(settingsPanel);
            GameManager.Instance?.ChangeGameState(GameManager.GameState.Settings);
        }
        
        public void ShowStore()
        {
            ShowPanel(storePanel);
            GameManager.Instance?.ChangeGameState(GameManager.GameState.Store);
        }
        
        public void ShowBusinesses()
        {
            ShowPanel(businessPanel);
            RefreshBusinessUIs();
        }
        
        public void ShowEnergyStore()
        {
            ShowStore();
            // TODO: Navigate to energy section of store
        }
        
        #endregion
        
        #region Resource Display Updates
        
        private void UpdateResourceDisplays()
        {
            if (EconomyManager.Instance != null)
            {
                OnCoinsChanged(EconomyManager.Instance.Coins);
                OnGemsChanged(EconomyManager.Instance.Gems);
            }
        }
        
        private void OnCoinsChanged(long newCoins)
        {
            if (coinsText != null)
                coinsText.text = FormatNumber(newCoins);
        }
        
        private void OnGemsChanged(int newGems)
        {
            if (gemsText != null)
                gemsText.text = newGems.ToString();
        }
        
        private string FormatNumber(long number)
        {
            if (number >= 1000000000)
                return (number / 1000000000f).ToString("F1") + "B";
            else if (number >= 1000000)
                return (number / 1000000f).ToString("F1") + "M";
            else if (number >= 1000)
                return (number / 1000f).ToString("F1") + "K";
            else
                return number.ToString();
        }
        
        #endregion
        
        #region Business UI Management
        
        private void CreateBusinessUIs()
        {
            if (EconomyManager.Instance == null || businessContainer == null || businessUIPrefab == null)
                return;
            
            businessUIs.Clear();
            
            foreach (var business in EconomyManager.Instance.Businesses)
            {
                GameObject businessUIObj = Instantiate(businessUIPrefab, businessContainer);
                BusinessUI businessUI = businessUIObj.GetComponent<BusinessUI>();
                
                if (businessUI != null)
                {
                    businessUI.Initialize(business);
                    businessUIs.Add(businessUI);
                }
            }
        }
        
        private void RefreshBusinessUIs()
        {
            foreach (var businessUI in businessUIs)
            {
                businessUI.RefreshUI();
            }
        }
        
        private void OnBusinessPurchased(Business business)
        {
            RefreshBusinessUIs();
            ShowNotification($"Purchased {business.Name}!", Color.green);
        }
        
        private void OnBusinessUpgraded(Business business)
        {
            RefreshBusinessUIs();
            ShowNotification($"Upgraded {business.Name} to Level {business.Level}!", Color.cyan);
        }
        
        #endregion
        
        #region Offline Earnings
        
        private void OnOfflineEarningsCalculated(long earnings)
        {
            if (earnings <= 0) return;
            
            if (offlineEarningsPanel != null)
            {
                offlineEarningsPanel.SetActive(true);
                
                if (offlineEarningsAmountText != null)
                    offlineEarningsAmountText.text = FormatNumber(earnings);
                    
                // Calculate offline time (simplified)
                if (offlineTimeText != null)
                    offlineTimeText.text = "Welcome back!";
            }
        }
        
        private void CollectOfflineEarnings()
        {
            if (offlineEarningsPanel != null)
                offlineEarningsPanel.SetActive(false);
        }
        
        private void DoubleOfflineEarnings()
        {
            // TODO: Show rewarded ad and double the offline earnings
            CollectOfflineEarnings();
        }
        
        #endregion
        
        #region Notification System
        
        public void ShowNotification(string message, Color color)
        {
            GameObject notification = GetNotificationFromPool();
            
            if (notification != null)
            {
                Text notificationText = notification.GetComponentInChildren<Text>();
                if (notificationText != null)
                {
                    notificationText.text = message;
                    notificationText.color = color;
                }
                
                notification.SetActive(true);
                StartCoroutine(HideNotificationAfterDelay(notification));
            }
        }
        
        private GameObject GetNotificationFromPool()
        {
            if (notificationPool.Count > 0)
            {
                return notificationPool.Dequeue();
            }
            else if (notificationPrefab != null && notificationContainer != null)
            {
                return Instantiate(notificationPrefab, notificationContainer);
            }
            
            return null;
        }
        
        private IEnumerator HideNotificationAfterDelay(GameObject notification)
        {
            yield return new WaitForSeconds(notificationDuration);
            
            notification.SetActive(false);
            notificationPool.Enqueue(notification);
        }
        
        #endregion
        
        #region Event Handlers
        
        private void OnGameStateChanged(GameManager.GameState newState)
        {
            // Handle UI changes based on game state
            switch (newState)
            {
                case GameManager.GameState.Playing:
                    ShowMainGame();
                    break;
            }
        }
        
        private void OnRewardReceived(WheelReward reward)
        {
            string message = $"+{reward.amount} {reward.type}!";
            Color rewardColor = reward.type switch
            {
                RewardType.Coins => Color.yellow,
                RewardType.Energy => Color.blue,
                RewardType.Gems => Color.magenta,
                _ => Color.white
            };
            
            ShowNotification(message, rewardColor);
        }
        
        #endregion
    }
}