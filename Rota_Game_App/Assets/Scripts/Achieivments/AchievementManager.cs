using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
public class AchievementManager : MonoBehaviour
{
    public List<Achievement> achievements;
    private Dictionary<string, int> progress = new();

    public static AchievementManager Instance;

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

    public void RegisterProgress(string achievementID, int increment = 1)
    {
        if (!progress.ContainsKey(achievementID))
            progress[achievementID] = 0;

        progress[achievementID] += increment;

        Achievement achievement = achievements.Find(a => a.id == achievementID);
        if (achievement != null && ShouldUnlock(achievement, progress[achievementID]))
        {
            UnlockAchievement(achievement);
            Debug.Log("Acghievement progress for Perfect Turn registered.");
        }
    }

    private bool ShouldUnlock(Achievement achievement, int currentProgress)
    {
        return achievement.isRepeatable || currentProgress == 1;
    }

    private void UnlockAchievement(Achievement achievement)
    {
        GameManager.Instance.currencyManager.AddCurrency("DE", achievement.rewardAmount);
        Debug.Log($"Achievement unlocked: {achievement.title} (+{achievement.rewardAmount} DE)");
        // Trigger UI popup or animation here
    }


}
