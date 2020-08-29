using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace GameServices 
{
    // Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
    public class IAPManager : Singlenton<IAPManager>, IStoreListener, AServices
    {
        private static IStoreController m_StoreController;          // The Unity Purchasing system.
        private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

        // Product identifiers for all products capable of being purchased: 
        public static string kProduct_C_1 = "consumable_100.000";
        public static string kProduct_C_2 = "consumable_1.000.000";
        public static string kProduct_C_3 = "consumable_10.000.000";
        public static string kProduct_C_4 = "consumable_50.000.000";
        public static string kProduct_C_5 = "consumable_100.000.000";

        public static string kProduct_NC_noads = "nonads";

        public void Initialize()
        {
            // If we haven't set up the Unity Purchasing reference
            if (m_StoreController == null)
            {
                // Begin to configure our connection to Purchasing
                InitializePurchasing();
            }
        }

        public void InitializePurchasing()
        {
            // If we have already connected to Purchasing ...
            if (IsInitialized())
            {
                return;
            }

            // Create a builder, first passing in a suite of Unity provided stores.
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            // Add a product to sell / restore by way of its identifier, associating the general identifier
            // with its store-specific identifiers.
            builder.AddProduct(kProduct_C_1, ProductType.Consumable);
            builder.AddProduct(kProduct_C_2, ProductType.Consumable);
            builder.AddProduct(kProduct_C_3, ProductType.Consumable);
            builder.AddProduct(kProduct_C_4, ProductType.Consumable);
            builder.AddProduct(kProduct_C_5, ProductType.Consumable);

            // Continue adding the non-consumable product.
            builder.AddProduct(kProduct_NC_noads, ProductType.NonConsumable);

            UnityPurchasing.Initialize(this, builder);
        }
        private bool IsInitialized()
        {
            // Only say we are initialized if both the Purchasing references are set.
            return m_StoreController != null && m_StoreExtensionProvider != null;
        }

        public void BuyC1()
        {
            BuyProductID(kProduct_C_1);
        }
        public void BuyC2()
        {
            BuyProductID(kProduct_C_2);
        }
        public void BuyC3()
        {
            BuyProductID(kProduct_C_3);
        }
        public void BuyC4()
        {
            BuyProductID(kProduct_C_4);
        }
        public void BuyC5()
        {
            BuyProductID(kProduct_C_5);
        }

        public void BuyNonAds()
        {
            BuyProductID(kProduct_NC_noads);
        }

        private void BuyProductID(string productId)
        {
            // If Purchasing has been initialized ...
            if (IsInitialized())
            {
                // ... look up the Product reference with the general product identifier and the Purchasing 
                // system's products collection.
                Product product = m_StoreController.products.WithID(productId);

                // If the look up found a product for this device's store and that product is ready to be sold ... 
                if (product != null && product.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                    // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                    // asynchronously.
                    m_StoreController.InitiatePurchase(product);
                }
                // Otherwise ...
                else
                {
                    // ... report the product look-up failure situation  
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            // Otherwise ...
            else
            {
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            // Purchasing has succeeded initializing. Collect our Purchasing references.
            Debug.Log("OnInitialized: PASS");

            // Overall Purchasing system, configured with products for this application.
            m_StoreController = controller;
            // Store specific subsystem, for accessing device-specific store features.
            m_StoreExtensionProvider = extensions;
        }
        public void OnInitializeFailed(InitializationFailureReason error)
        {
            // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            // A consumable product has been purchased by this user.          
            if (String.Equals(args.purchasedProduct.definition.id, kProduct_C_1, StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. You've just bought C1. Product: '{0}'", args.purchasedProduct.definition.id));
                ServiceManager.Instance.OnRewardShopFinished.Invoke(true, 100000);
                game_manager.Instance.toggleShop();
            }
            else if (String.Equals(args.purchasedProduct.definition.id, kProduct_C_2, StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. You've just bought C2. Product: '{0}'", args.purchasedProduct.definition.id));
                ServiceManager.Instance.OnRewardShopFinished.Invoke(true, 1000000);
                game_manager.Instance.toggleShop();
            }
            else if (String.Equals(args.purchasedProduct.definition.id, kProduct_C_3, StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. You've just bought C3. Product: '{0}'", args.purchasedProduct.definition.id));
                ServiceManager.Instance.OnRewardShopFinished.Invoke(true, 10000000);
                game_manager.Instance.toggleShop();
            }
            else if (String.Equals(args.purchasedProduct.definition.id, kProduct_C_4, StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. You've just bought C4. Product: '{0}'", args.purchasedProduct.definition.id));
                ServiceManager.Instance.OnRewardShopFinished.Invoke(true, 50000000);
                game_manager.Instance.toggleShop();
            }
            else if (String.Equals(args.purchasedProduct.definition.id, kProduct_C_5, StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. You've just boughtc C5. Product: '{0}'", args.purchasedProduct.definition.id));
                ServiceManager.Instance.OnRewardShopFinished.Invoke(true, 100000000);
                game_manager.Instance.toggleShop();
            }
            // Or ... a non-consumable product has been purchased by this user.
            else if (String.Equals(args.purchasedProduct.definition.id, kProduct_NC_noads, StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. You' ve just bougth. Product: '{0}'", args.purchasedProduct.definition.id));
            }
            // Or ... an unknown product has been purchased by this user. Fill in additional products here....
            else
            {
                Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
            }

            return PurchaseProcessingResult.Complete;
        }
        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
            // this reason with the user to guide their troubleshooting actions.
            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        }
    }

}
