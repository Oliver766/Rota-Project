using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using TMPro;

public class CurrencyManager : MonoBehaviour
{
    private Dictionary<string, int> currencyCache = new Dictionary<string, int>();

    
        public void GetCurrencyBalance()
    {
        var request = new GetUserInventoryRequest();

        PlayFabClientAPI.GetUserInventory(request, result =>
        {
            foreach (var currency in result.VirtualCurrency)
            {
                Debug.Log($"Currency: {currency.Key}, Balance: {currency.Value}");
            }
        }, error =>
        {
            Debug.LogError($"Failed to get currency: {error.GenerateErrorReport()}");
        });
    }


    public void AddCurrency(string currencyCode, int amount)
    {
        var request = new AddUserVirtualCurrencyRequest
        {
            VirtualCurrency = currencyCode, // e.g. "GD"
            Amount = amount
        };

        PlayFabClientAPI.AddUserVirtualCurrency(request, result =>
        {
            Debug.Log($"Added {amount} {currencyCode} to player.");
        }, error =>
        {
            Debug.LogError($"Failed to add currency: {error.GenerateErrorReport()}");
        });
    }

    public void SubtractCurrency(string currencyCode, int amount)
    {
        var request = new SubtractUserVirtualCurrencyRequest
        {
            VirtualCurrency = currencyCode,
            Amount = amount
        };

        PlayFabClientAPI.SubtractUserVirtualCurrency(request, result =>
        {
            Debug.Log($"Subtracted {amount} {currencyCode} from player.");
        }, error =>
        {
            Debug.LogError($"Failed to subtract currency: {error.GenerateErrorReport()}");
        });
    }

    public bool HasEnoughCurrency(string currencyCode, int amount)
    {
    if (!currencyCache.ContainsKey(currencyCode))
    {
        Debug.LogWarning($"Currency {currencyCode} not found in cache.");
        return false;
    }

    return currencyCache[currencyCode] >= amount;
    }


    
}


  






