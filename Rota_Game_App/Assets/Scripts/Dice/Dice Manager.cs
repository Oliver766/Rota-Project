using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DiceManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI playerResultText;
    public TextMeshProUGUI aiResultText;
    public TextMeshProUGUI rerollText;
    public Button continueButton;
    public Button rerollButton;




    [Header("Dice Values")]
    [SerializeField] public int diceNumber;
    [SerializeField] public int aiNumber;

    public int PlayerRoll { get; private set; }
    public int AIRoll { get; private set; }

    void Start()
    {
        rerollText.text = "";
        //UpdateRolls();
        // CheckForDraw();
    }

    void Update()
    {
        UpdateRolls();
    }

    public void UpdateRolls()
    {
        PlayerRoll = diceNumber;
        AIRoll = aiNumber;

        playerResultText.text = PlayerRoll.ToString();
        aiResultText.text = AIRoll.ToString();

        PlayerPrefs.SetInt("PlayerRoll", PlayerRoll);
        PlayerPrefs.SetInt("AIRoll", AIRoll);
    }

    public void CheckForDraw()
    {
        bool isDraw = PlayerRoll == AIRoll;

        rerollButton.gameObject.SetActive(isDraw);
        continueButton.gameObject.SetActive(!isDraw);

        if (isDraw)
        {
            Debug.Log("It's a draw! Rerolling...");
            rerollText.text = "Draw! Please reroll.";
        }
        else
        {
            rerollText.text = "";
        }
    }

    public void Reroll()
    {
        // Replace with your actual scene loading logic
        LevelLoaading.levelLoaading.LoadScene(1);
    }
}