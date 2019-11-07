using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour, IStoreListener
{
    public Button NoAdsButton;
    public ServerConnection ServerConnection;

    private static IStoreController storeController;
    private static IExtensionProvider storeExtensionProvider;

    void Start()
    {
        if (storeController == null)
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            builder.AddProduct(ExternalServices.NO_ADS_PRODUCT_ID, ProductType.NonConsumable);
            UnityPurchasing.Initialize(this, builder);
        }

        if (AdsRemoved())
        {
            NoAdsButton.gameObject.SetActive(false);
        }
        else
        {
            NoAdsButton.onClick.AddListener(NoAdsClicked);
        }
    }

    public bool AdsRemoved()
    {
        return PlayerPrefs.GetInt("UnlockedNoAds", 0) == 1;
    }

    public void SetAdsRemoved()
    {
        PlayerPrefs.SetInt("UnlockedNoAds", 1);
    }

    public void DoRestore()
    {
#if UNITY_IOS
        IAppleExtensions appleExtension = storeExtensionProvider.GetExtension<IAppleExtensions>();
        appleExtension.RestoreTransactions((result) =>
            {
                if (result)
                    Debug.Log("Restore finished!");
                else
                    Debug.LogWarning("Restore failed!");
            });
#endif
    }

    public void NoAdsClicked()
    {
        if (storeController != null)
        {
            Product product = storeController.products.WithID(ExternalServices.NO_ADS_PRODUCT_ID);

            if (product != null && product.availableToPurchase)
            {
                storeController.InitiatePurchase(product);
            }
            else
            {
                Debug.Log("");
                NoAdsButton.gameObject.SetActive(false);
            }
        }
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogFormat("IAP Init failed: {0}", error);
        NoAdsButton.gameObject.SetActive(false);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        Debug.LogFormat("Purchased: {0}", e.purchasedProduct.definition.id);

        ServerConnection.SubmitEvent(new Messages.TrackPurchace
        {
            transaction = e.purchasedProduct.transactionID,
            product = e.purchasedProduct.definition.id,
            price = e.purchasedProduct.metadata.localizedPrice,
            currency = e.purchasedProduct.metadata.isoCurrencyCode
        });

        if (e.purchasedProduct.definition.id == ExternalServices.NO_ADS_PRODUCT_ID)
        {
            SetAdsRemoved();
            NoAdsButton.gameObject.SetActive(false);
        }
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        Debug.LogFormat("Purchase failed: {0} - {1}", i.definition.id, p);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("Store initialized OK");
        storeController = controller;
        storeExtensionProvider = extensions;
    }
}
