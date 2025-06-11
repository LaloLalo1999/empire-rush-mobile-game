using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EmpireRush.Economy;

namespace EmpireRush.UI
{
    public class BusinessUI : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI businessNameText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI incomeText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private Button purchaseUpgradeButton;
        [SerializeField] private Image businessIcon;
        [SerializeField] private GameObject ownedIndicator;
        [SerializeField] private Slider progressBar;
        
        [Header("Visual States")]
        [SerializeField] private Color affordableColor = Color.green;
        [SerializeField] private Color unaffordableColor = Color.red;
        [SerializeField] private Color ownedColor = Color.white;
        
        private Business business;
        
        public void Initialize(Business businessData)
        {
            business = businessData;
            
            if (purchaseUpgradeButton != null)
                purchaseUpgradeButton.onClick.AddListener(OnPurchaseUpgradeClicked);
            
            RefreshUI();
        }
        
        public void RefreshUI()
        {
            if (business == null) return;
            
            // Update business name
            if (businessNameText != null)
                businessNameText.text = business.Name;
            
            // Update level display
            if (levelText != null)
            {
                if (business.IsOwned)
                    levelText.text = $"Level {business.Level}";
                else
                    levelText.text = "Not Owned";
            }
            
            // Update income display
            if (incomeText != null)
            {
                if (business.IsOwned)
                {
                    long income = business.GetIncomePerSecond();
                    incomeText.text = $"{FormatNumber(income)}/sec";
                }
                else
                {
                    incomeText.text = "Purchase to unlock";
                }
            }
            
            // Update cost and button
            UpdateCostAndButton();
            
            // Update owned indicator
            if (ownedIndicator != null)
                ownedIndicator.SetActive(business.IsOwned);
            
            // Update progress bar (for future upgrade timers)
            if (progressBar != null)
            {
                if (business.IsOwned)
                {
                    progressBar.gameObject.SetActive(true);
                    progressBar.value = business.GetUpgradeProgress();
                }
                else
                {
                    progressBar.gameObject.SetActive(false);
                }
            }
            
            // Update visual state
            UpdateVisualState();
        }
        
        private void UpdateCostAndButton()
        {
            if (costText == null || purchaseUpgradeButton == null) return;
            
            long cost = business.IsOwned ? business.GetUpgradeCost() : business.GetPurchaseCost();
            bool canAfford = EconomyManager.Instance != null && EconomyManager.Instance.Coins >= cost;
            
            // Update cost text
            costText.text = FormatNumber(cost);
            
            // Update button text
            TextMeshProUGUI buttonText = purchaseUpgradeButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = business.IsOwned ? "UPGRADE" : "BUY";
            }
            
            // Update button interactability
            purchaseUpgradeButton.interactable = canAfford;
            
            // Update cost text color
            costText.color = canAfford ? affordableColor : unaffordableColor;
        }
        
        private void UpdateVisualState()
        {
            // Update overall color scheme based on ownership and affordability
            Image backgroundImage = GetComponent<Image>();
            if (backgroundImage != null)
            {
                if (business.IsOwned)
                {
                    backgroundImage.color = ownedColor;
                }
                else
                {
                    bool canAfford = EconomyManager.Instance != null && 
                                   EconomyManager.Instance.Coins >= business.GetPurchaseCost();
                    backgroundImage.color = canAfford ? affordableColor * 0.3f : unaffordableColor * 0.3f;
                }
            }
        }
        
        private void OnPurchaseUpgradeClicked()
        {
            if (business == null || EconomyManager.Instance == null) return;
            
            bool success = false;
            
            if (business.IsOwned)
            {
                success = EconomyManager.Instance.UpgradeBusiness(business.Type);
            }
            else
            {
                success = EconomyManager.Instance.PurchaseBusiness(business.Type);
            }
            
            if (success)
            {
                RefreshUI();
                
                // Play purchase/upgrade sound effect
                // TODO: Add audio feedback
            }
            else
            {
                // Show insufficient funds feedback
                UIManager.Instance?.ShowNotification("Insufficient coins!", Color.red);
            }
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
        
        private void Update()
        {
            // Continuously update affordability (could be optimized to only update on coin changes)
            if (business != null && EconomyManager.Instance != null)
            {
                UpdateCostAndButton();
                UpdateVisualState();
            }
        }
    }
}