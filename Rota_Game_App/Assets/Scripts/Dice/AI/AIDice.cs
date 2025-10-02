using UnityEngine;
using System.Collections;
using TMPro;

public class AIDice : MonoBehaviour
{
    public Rigidbody rbAI;
    public static Vector3 diceAIVelocity;
    public DiceManager DiceNumberText;
    public GameObject AIDiceVisual;
    public GameObject checkObj;
    public GameObject resultPanel;
    public TextMeshProUGUI statusText;


    void Start()
    {
        StartAIRoll();
    }
    public void StartAIRoll()
    {
        StartCoroutine(RollSequence());
    }
    public void Update()
    {
         diceAIVelocity = rbAI.linearVelocity;
    }

    private IEnumerator RollSequence()
    {
        yield return new WaitForSeconds(3);
        statusText.text = "Time for AI to roll!";
        checkObj.SetActive(true);
        Roll();
        yield return new WaitForSeconds(5);
         statusText.text = "";
        resultPanel.SetActive(true);
        DiceNumberText.checkfordraw();
    }

    private void Roll()
    {
        DiceNumberText.aiNumber = 0;
        diceAIVelocity = rbAI.linearVelocity;
        float dirx = Random.Range(0, 500);
        float diry = Random.Range(0, 500);
        float dirz = Random.Range(0, 500);

        transform.position = new Vector3(0, 2, 0);
        transform.rotation = Quaternion.identity;

        rbAI.AddForce(transform.up * 500);
        rbAI.AddTorque(dirx, diry, dirz);
    }
}