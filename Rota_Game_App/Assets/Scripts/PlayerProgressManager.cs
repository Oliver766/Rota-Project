using UnityEngine;
using TMPro;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class PlayerProgressManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI levelText;
    public Slider xpBar;
    public TextMeshProUGUI xpText;

    [Header("Progress Settings")]
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 200;
    public int xpIncrementPerLevel = 50;

    private void Start()
    {
        LoadProgressFromPlayFab();
        UpdateXPUI();
    }

    void Update()
    {
        UpdateXPUI(); 
         
    }

    public void AddXP(int amount)
    {
        currentXP += amount;

        if (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            currentLevel++;
            xpToNextLevel += xpIncrementPerLevel;

            // Optional: trigger achievement or reward
            AchievementManager.Instance?.RegisterProgress("level_up");
        }

        UpdateXPUI();
        SaveProgressToPlayFab();
    }

    public void UpdateXPUI()
    {
        //levelText.text = $"Level {currentLevel}";
        levelText.text = currentLevel.ToString();
        xpBar.maxValue = xpToNextLevel;
        xpBar.value = currentXP;
        xpText.text = $"{currentXP} / {xpToNextLevel}";
    }

    private void SaveProgressToPlayFab()
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "Level", currentLevel.ToString() },
                { "XP", currentXP.ToString() },
                { "XPToNext", xpToNextLevel.ToString() }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, result =>
        {
            Debug.Log("Progress saved to PlayFab.");
        }, error =>
        {
            Debug.LogError($"Failed to save progress: {error.GenerateErrorReport()}");
        });
    }

    private void LoadProgressFromPlayFab()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
        {
            var data = result.Data;

            if (data != null)
            {
                if (data.ContainsKey("Level"))
                    currentLevel = int.Parse(data["Level"].Value);
                if (data.ContainsKey("XP"))
                    currentXP = int.Parse(data["XP"].Value);
                if (data.ContainsKey("XPToNext"))
                    xpToNextLevel = int.Parse(data["XPToNext"].Value);
            }

            UpdateXPUI();
        }, error =>
        {
            Debug.LogError($"Failed to load progress: {error.GenerateErrorReport()}");
        });
    }
}