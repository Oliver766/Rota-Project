using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DiceScript : MonoBehaviour
{

    private Rigidbody rb;
    public static Vector3 diceVelocity;

    public DiceNumberText DiceNumberText;
    public GameObject ResultPanel;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    void Update()
    {
        diceVelocity = rb.linearVelocity;

        if (Input.GetKeyDown(KeyCode.Space))
        {

        }
    }

    public void Roll()
    {
        StartCoroutine(WaitForRoll());
        DiceNumberText.diceNumber = 0;
        float dirx = Random.Range(0, 500);
        float diry = Random.Range(0, 500);
        float dirz = Random.Range(0, 500);
        transform.position = new Vector3(0, 2, 0);
        transform.rotation = Quaternion.identity;
        rb.AddForce(transform.up * 500);
        rb.AddTorque(dirx, diry, dirz);
        DiceNumberText.RollAIDice();
    }
     

     IEnumerator WaitForRoll()
     {
         yield return new WaitForSeconds(3);
         ResultPanel.SetActive(true);
     }

}
