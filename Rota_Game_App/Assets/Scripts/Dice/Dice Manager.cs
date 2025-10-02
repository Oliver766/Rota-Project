using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DiceManager : MonoBehaviour
{
     public TextMeshProUGUI playerResultText;
    public TextMeshProUGUI AIResultText;

     public TextMeshProUGUI ReRoillText;
    public int diceNumber;
    public int aiNumber;


    public Button ContinueButton;
    public Button rerollButton;


    public int PlayerRoll { get; private set; }

    public int AIRoll { get; private set; }




    // Update is called once per frame
    void Update()
    {
        PlayerRoll = diceNumber;
        AIRoll = aiNumber;
        playerResultText.text = PlayerRoll.ToString();
        AIResultText.text = AIRoll.ToString();
        PlayerPrefs.SetInt("PlayerRoll", PlayerRoll);
        PlayerPrefs.SetInt("AIRoll", AIRoll);
    }

    public void Start()
    {
        ReRoillText.text = "";
    }



    public void checkfordraw()
    {
        if (PlayerRoll == AIRoll)
        {
            Debug.Log("It's a draw! Rerolling...");
            rerollButton.gameObject.SetActive(true);
            ContinueButton.gameObject.SetActive(false);

        }
        else
        {
            ContinueButton.gameObject.SetActive(true);
            rerollButton.gameObject.SetActive(false);
        }
    }
    
    public void Reroll()
    {
       LevelLoaading.levelLoaading.LoadScene(1);
    }
}
