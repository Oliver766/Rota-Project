using UnityEngine;
using UnityEngine.Events;

public enum PlayerType
{
    Human,
    AI
}

public class StartDeterminer : MonoBehaviour
{
    [Header("Roll Values")]
    public int PlayerRoll;
    public int AIRoll;

    [Header("Events")]
    public UnityEvent<PlayerType> OnStartPlayerDecided;

    private void Start()
    {
        LoadRolls();
        DetermineStarter();
    }

    private void LoadRolls()
    {
        PlayerRoll = PlayerPrefs.GetInt("PlayerRoll", 0);
        AIRoll = PlayerPrefs.GetInt("AIRoll", 0);
    }

    public void DetermineStarter()
    {
        if (PlayerRoll > AIRoll)
        {
            DecideStarter(PlayerType.Human);
        }
        else if (AIRoll > PlayerRoll)
        {
            DecideStarter(PlayerType.AI);
        }
        else
        {
            HandleTie();
        }
    }

    private void DecideStarter(PlayerType starter)
    {
        OnStartPlayerDecided?.Invoke(starter);
        Debug.Log($"Starter decided: {starter}");
    }

    private void HandleTie()
    {
        Debug.Log("Tie detected — rerolling...");
        // Example fallback: random choice
        PlayerType fallback = (Random.value < 0.5f) ? PlayerType.Human : PlayerType.AI;
        DecideStarter(fallback);
    }
}