using System;
using System.Collections.Generic;
using UnityEngine;

namespace EmpireRush.Monetization
{
    public class IAPManager : MonoBehaviour
    {
        [Header("IAP Products")]
        [SerializeField] private List<IAPProduct> products = new List<IAPProduct>();
        
        [Header("Settings")]
        [SerializeField] private bool debugMode = false;
        
        // Unity IAP will be integrated here
        // private IStoreController storeController;
        // private IExtensionProvider storeExtensionProvider;
        
        private Dictionary<string, IAPProduct> productCatalog = new Dictionary<string, IAPProduct>();
        
        public static event Action OnIAPInitialized;
        public static event Action<string> OnIAPInitializationFailed;
        
        private void Start()
        {
            InitializeIAP();
        }
        
        private void InitializeIAP()
        {
            CreateProductCatalog();
            
            // TODO: Initialize Unity IAP
            // var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            // 
            // foreach (var product in products)
            // {
            //     builder.AddProduct(product.id, product.type);
            // }
            // 
            // UnityPurchasing.Initialize(this, builder);
            
            // For now, simulate successful initialization
            if (debugMode)
                Debug.Log("IAP System initialized (simulated)");
                
            OnIAPInitialized?.Invoke();
        }
        
        private void CreateProductCatalog()
        {
            productCatalog.Clear();
            
            // Create default products if none are set
            if (products.Count == 0)
            {
                CreateDefaultProducts();
            }
            
            foreach (var product in products)
            {
                productCatalog[product.id] = product;
            }
        }
        
        private void CreateDefaultProducts()
        {
            products = new List<IAPProduct>
            {
                // Energy Packs
                new IAPProduct("energy_pack_small", "Small Energy Pack", "25 Energy", 0.99f, IAPProductType.Consumable),
                new IAPProduct("energy_pack_medium", "Medium Energy Pack", "150 Energy", 4.99f, IAPProductType.Consumable),
                new IAPProduct("energy_pack_large", "Large Energy Pack", "750 Energy", 19.99f, IAPProductType.Consumable),
                
                // Coin Packs
                new IAPProduct("coin_pack_small", "Small Coin Pack", "10,000 Coins", 0.99f, IAPProductType.Consumable),
                new IAPProduct("coin_pack_medium", "Medium Coin Pack", "75,000 Coins", 4.99f, IAPProductType.Consumable),
                new IAPProduct("coin_pack_large", "Large Coin Pack", "500,000 Coins", 19.99f, IAPProductType.Consumable),
                
                // Gem Packs
                new IAPProduct("gem_pack_small", "Small Gem Pack", "50 Gems", 0.99f, IAPProductType.Consumable),
                new IAPProduct("gem_pack_medium", "Medium Gem Pack", "300 Gems", 4.99f, IAPProductType.Consumable),
                new IAPProduct("gem_pack_large", "Large Gem Pack", "1,500 Gems", 19.99f, IAPProductType.Consumable),
                
                // Special Offers
                new IAPProduct("starter_pack", "Starter Pack", "100 Energy + 50,000 Coins + 100 Gems", 9.99f, IAPProductType.Consumable),
                new IAPProduct("remove_ads", "Remove Ads", "Remove all interstitial ads", 2.99f, IAPProductType.NonConsumable)
            };
        }
        
        public void PurchaseProduct(string productId, Action<string> onSuccess, Action<string, string> onFailure)
        {
            if (!productCatalog.ContainsKey(productId))
            {
                onFailure?.Invoke(productId, "Product not found");
                return;
            }
            
            // TODO: Implement Unity IAP purchase
            // storeController?.InitiatePurchase(productId);
            
            // For now, simulate purchase process
            if (debugMode)
                Debug.Log($"Simulating purchase of {productId}");
            
            // Simulate random success/failure for testing
            if (UnityEngine.Random.Range(0f, 1f) > 0.1f) // 90% success rate
            {
                onSuccess?.Invoke(productId);
            }
            else
            {
                onFailure?.Invoke(productId, "Simulated purchase failure");
            }
        }
        
        public IAPProduct GetProduct(string productId)
        {
            productCatalog.TryGetValue(productId, out IAPProduct product);
            return product;
        }
        
        public List<IAPProduct> GetAllProducts()
        {
            return new List<IAPProduct>(products);
        }
        
        public List<IAPProduct> GetProductsByType(IAPProductType type)
        {
            return products.FindAll(p => p.type == type);
        }
        
        public bool IsProductOwned(string productId)
        {
            // TODO: Check with Unity IAP if product is owned
            // This is mainly for non-consumable products
            
            if (productId == "remove_ads")
            {
                return PlayerPrefs.GetInt("remove_ads_purchased", 0) == 1;
            }
            
            return false;
        }
        
        public void RestorePurchases(Action onSuccess, Action<string> onFailure)
        {
            // TODO: Implement Unity IAP restore purchases
            // storeExtensionProvider?.GetExtension<IAppleExtensions>()?.RestoreTransactions(result => {
            //     if (result)
            //         onSuccess?.Invoke();
            //     else
            //         onFailure?.Invoke("Failed to restore purchases");
            // });
            
            if (debugMode)
                Debug.Log("Restore purchases simulated");
                
            onSuccess?.Invoke();
        }
        
        // Unity IAP Interface Implementation (TODO)
        // public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        // {
        //     storeController = controller;
        //     storeExtensionProvider = extensions;
        //     OnIAPInitialized?.Invoke();
        // }
        // 
        // public void OnInitializeFailed(InitializationFailureReason error)
        // {
        //     OnIAPInitializationFailed?.Invoke(error.ToString());
        // }
        // 
        // public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        // {
        //     var product = args.purchasedProduct;
        //     
        //     // Validate purchase receipt here if needed
        //     
        //     // Handle non-consumable products
        //     if (product.definition.id == "remove_ads")
        //     {
        //         PlayerPrefs.SetInt("remove_ads_purchased", 1);
        //         PlayerPrefs.Save();
        //     }
        //     
        //     // Notify successful purchase
        //     OnPurchaseSuccess?.Invoke(product.definition.id);
        //     
        //     return PurchaseProcessingResult.Complete;
        // }
        // 
        // public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        // {
        //     OnPurchaseFailure?.Invoke(product.definition.id, failureReason.ToString());
        // }
        
        #region Pricing and Localization
        
        public string GetLocalizedPrice(string productId)
        {
            // TODO: Get localized price from Unity IAP
            // var product = storeController?.products?.WithID(productId);
            // return product?.metadata?.localizedPriceString ?? "$0.00";
            
            var iapProduct = GetProduct(productId);
            return iapProduct != null ? $"${iapProduct.price:F2}" : "$0.00";
        }
        
        public string GetLocalizedTitle(string productId)
        {
            // TODO: Get localized title from Unity IAP
            // var product = storeController?.products?.WithID(productId);
            // return product?.metadata?.localizedTitle ?? "Unknown Product";
            
            var iapProduct = GetProduct(productId);
            return iapProduct?.title ?? "Unknown Product";
        }
        
        public string GetLocalizedDescription(string productId)
        {
            // TODO: Get localized description from Unity IAP
            // var product = storeController?.products?.WithID(productId);
            // return product?.metadata?.localizedDescription ?? "No description available";
            
            var iapProduct = GetProduct(productId);
            return iapProduct?.description ?? "No description available";
        }
        
        #endregion
    }
    
    [System.Serializable]
    public class IAPProduct
    {
        public string id;
        public string title;
        public string description;
        public float price;
        public IAPProductType type;
        
        public IAPProduct(string id, string title, string description, float price, IAPProductType type)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.price = price;
            this.type = type;
        }
    }
    
    public enum IAPProductType
    {
        Consumable,
        NonConsumable,
        Subscription
    }
}