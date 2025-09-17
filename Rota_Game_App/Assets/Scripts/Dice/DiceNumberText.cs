using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DiceNumberText : MonoBehaviour
{

    public TextMeshProUGUI diceNumberText;
    public static int diceNumber;
   

    // Update is called once per frame
    void Update()
    {
        diceNumberText.text = diceNumber.ToString();
    }
}
