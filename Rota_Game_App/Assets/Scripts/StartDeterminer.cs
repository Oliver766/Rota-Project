using UnityEngine;
using UnityEngine.Events;


public enum PlayerType
{
    Human,
    AI
}


public class StartDeterminer : MonoBehaviour
{
    public int PlayerRoll;
    public int AIRoll;

    public UnityEvent<PlayerType> OnStartPlayerDecided;

    void Start()
    {
        PlayerRoll = PlayerPrefs.GetInt("PlayerRoll", 0);
        AIRoll = PlayerPrefs.GetInt("AIRoll", 0);
      
    }

    public void DetermineStarter()
    {
        if (PlayerRoll > AIRoll)
        {
            OnStartPlayerDecided?.Invoke(PlayerType.Human);
            Debug.Log(PlayerType.Human);
        }
        else if (AIRoll > PlayerRoll)
        {
            OnStartPlayerDecided?.Invoke(PlayerType.AI);
            Debug.Log(PlayerType.AI);
        }
        else
        {
            HandleTie();
        }
    }

    private void HandleTie()
    {
        // Optional: reroll, prompt UI, or default to a player
        Debug.Log("Tie detected — rerolling...");
        // Trigger reroll logic or fallback
    }


}
