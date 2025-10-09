using UnityEngine;
using System.Collections;
using TMPro;

public class AIDice : MonoBehaviour
{
    [Header("Physics & Visuals")]
    public Rigidbody rbAI;                  // Rigidbody controlling the AI's dice
    public GameObject aiDiceVisual;        // Visual representation of the dice

    [Header("UI Elements")]
    public TextMeshProUGUI statusText;     // Status message during AI roll
    public GameObject checkIndicator;      // Object shown when checking for draw
    public GameObject resultPanel;         // Panel shown after roll completes

    [Header("Dice Manager Reference")]
    public DiceManager diceManager;        // Reference to DiceManager to sync AI roll

    // Tracks current velocity of the AI dice (for potential physics-based detection)
    public static Vector3 diceAIVelocity;

    void Start()
    {
        StartAIRoll(); // Begin AI roll sequence on start
    }

    void Update()
    {
        // Continuously update dice velocity for external access
        diceAIVelocity = rbAI.linearVelocity;
    }

    /// <summary>
    /// Begins the AI roll sequence with delay and feedback.
    /// </summary>
    public void StartAIRoll()
    {
        StartCoroutine(RollSequence());
    }

    /// <summary>
    /// Handles the timed sequence of AI rolling the dice.
    /// </summary>
    private IEnumerator RollSequence()
    {
        yield return new WaitForSeconds(2f); // Delay before roll
        statusText.text = "Time for AI to roll!";
        checkIndicator.SetActive(true);     // Show check object

        Roll(); // Physically roll the dice

        yield return new WaitForSeconds(5f); // Wait for dice to settle
        statusText.text = "";
        resultPanel.SetActive(true);        // Show result panel

        diceManager.CheckForDraw();         // Check if player and AI rolled the same
    }

    /// <summary>
    /// Applies force and torque to simulate a dice roll.
    /// </summary>
    private void Roll()
    {
        diceManager.aiNumber = 0; // Reset AI roll value

        // Generate random torque values
        float torqueX = Random.Range(0f, 500f);
        float torqueY = Random.Range(0f, 500f);
        float torqueZ = Random.Range(0f, 500f);

        // Reset dice position and rotation
        transform.position = new Vector3(0f, 2f, 0f);
        transform.rotation = Quaternion.identity;

        // Apply upward force and random spin
        rbAI.AddForce(Vector3.up * 500f);
        rbAI.AddTorque(torqueX, torqueY, torqueZ);
    }
}