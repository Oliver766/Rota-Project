using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerDice : MonoBehaviour
{
    public Rigidbody rb;

    public static Vector3 diceVelocity;
    public DiceManager DiceNumberText;
    public GameObject playerDiceVisual;
    public GameObject rollButton;
    public GameObject checkObj;

    public GameObject aidice;

    public AIDice AIDiceScript;

    public GameObject aichecker;


    public void Update()
    {
        diceVelocity = rb.linearVelocity;
    }


    void Start()
    {
        aichecker.SetActive(false);
    }


    public void Roll()
    {
        StartCoroutine(RollSequence());
        DiceNumberText.diceNumber = 0;
        diceVelocity = rb.linearVelocity;
        float dirx = Random.Range(0, 500);
        float diry = Random.Range(0, 500);
        float dirz = Random.Range(0, 500);

        transform.position = new Vector3(0, 2, 0);
        transform.rotation = Quaternion.identity;

        rb.AddForce(transform.up * 500);
        rb.AddTorque(dirx, diry, dirz);


    }

    IEnumerator RollSequence()
    {
        yield return new WaitForSeconds(7);
        this.gameObject.SetActive(false);
        rollButton.SetActive(false);
        checkObj.SetActive(false);
        aidice.SetActive(true);

        
    }
}