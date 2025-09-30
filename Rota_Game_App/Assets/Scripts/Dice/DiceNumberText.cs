using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DiceNumberText : MonoBehaviour
{


    public TextMeshProUGUI playerResultText;
    public TextMeshProUGUI AIResultText;
    public int diceNumber;

    public int PlayerRoll { get; private set; }

    public int AIRoll { get; private set; }




    // Update is called once per frame
    void Update()
    {
        PlayerRoll = diceNumber;
        playerResultText.text = PlayerRoll.ToString();
        AIResultText.text = AIRoll.ToString();
        PlayerPrefs.SetInt("PlayerRoll", PlayerRoll);
        PlayerPrefs.SetInt("AIRoll", AIRoll);
    }
    
    public void RollAIDice()
    {
        AIRoll = Random.Range(1, 4);
        Debug.Log($"AI rolled: {AIRoll}");
    }

}
