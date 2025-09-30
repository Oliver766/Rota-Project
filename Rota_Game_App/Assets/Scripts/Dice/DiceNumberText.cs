using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DiceNumberText : MonoBehaviour
{

    public TextMeshProUGUI diceNumberText;
    public int diceNumber;

    public int PlayerRoll { get; private set; }

    public int AIRoll { get; private set; }




    // Update is called once per frame
    void Update()
    {
        diceNumberText.text = diceNumber.ToString();
        PlayerRoll = diceNumber;
        PlayerPrefs.SetInt("PlayerRoll", PlayerRoll);
        PlayerPrefs.SetInt("AIRoll", AIRoll);
    }
    
    public void RollAIDice()
    {
        AIRoll = Random.Range(1, 4);
        Debug.Log($"AI rolled: {AIRoll}");
    }

}
