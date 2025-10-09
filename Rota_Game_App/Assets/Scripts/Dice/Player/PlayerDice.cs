using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerDice : MonoBehaviour
{
    #region 🔧 References & UI Elements

    [Header("Physics & Visuals")]
    public Rigidbody rb;                        // Rigidbody controlling the player's dice
    public GameObject playerDiceVisual;         // Visual representation of the dice

    [Header("UI & Game Flow")]
    public GameObject rollButton;               // Button to trigger dice roll
    public GameObject checkIndicator;           // Object shown when checking for draw
    public GameObject aiDiceObject;             // AI dice GameObject to activate after roll
    public GameObject aiChecker;                // Optional AI checker object

    [Header("Managers")]
    public DiceManager diceManager;             // Reference to DiceManager for syncing roll
    public AIDice aiDiceScript;                 // Reference to AI dice script

    #endregion

    #region 🎲 Dice State

    public static Vector3 diceVelocity;         // Current velocity of the player's dice

    #endregion

    #region 🚀 Unity Lifecycle

    void Start()
    {
        aiChecker.SetActive(false);             // Hide AI checker at start
    }

    void Update()
    {
        diceVelocity = rb.linearVelocity;       // Continuously track dice velocity
    }

    #endregion

    #region 🎯 Dice Roll Logic

    /// <summary>
    /// Initiates the player's dice roll with physics and coroutine.
    /// </summary>
    public void Roll()
    {
        StartCoroutine(RollSequence());

        diceManager.diceNumber = 0;             // Reset roll value

        // Random torque values for spin
        float torqueX = Random.Range(0f, 500f);
        float torqueY = Random.Range(0f, 500f);
        float torqueZ = Random.Range(0f, 500f);

        // Reset dice position and rotation
        transform.position = new Vector3(0f, 2f, 0f);
        transform.rotation = Quaternion.identity;

        // Apply upward force and spin
        rb.AddForce(Vector3.up * 500f);
        rb.AddTorque(torqueX, torqueY, torqueZ);
    }

    /// <summary>
    /// Handles post-roll sequence: disables UI and activates AI dice.
    /// </summary>
    private IEnumerator RollSequence()
    {
        yield return new WaitForSeconds(4f);    // Wait for dice to settle

        gameObject.SetActive(false);            // Hide player dice
        rollButton.SetActive(false);            // Hide roll button
        checkIndicator.SetActive(false);        // Hide check indicator
        aiDiceObject.SetActive(true);           // Activate AI dice
    }

    #endregion
}