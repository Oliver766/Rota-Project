using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class DiceCheckZoneAI : MonoBehaviour
{
    Vector3 diceVelocityAtEnter;

    public DiceManager DiceNumberText;

    public AIDice AIDiceScript;

    public int num;



    void FixedUpdate()
    {
        diceVelocityAtEnter = AIDice.diceAIVelocity;
    }

    void Update()
    {
        num = DiceNumberText.aiNumber;
    }

    void OnTriggerStay(Collider col)
    {
        if (diceVelocityAtEnter.x == 0f && diceVelocityAtEnter.y == 0f && diceVelocityAtEnter.z == 0f) {
            switch (col.gameObject.name)
            {
                case "AISide 1":
                    DiceNumberText.aiNumber = 1; 

                    break;
                case "AISide 2":
                    DiceNumberText.aiNumber = 2;
                    break;
                case "AISide 3":
                    DiceNumberText.aiNumber = 4;
                    break;
                case "AISide 4":
                    DiceNumberText.aiNumber = 3;
                    break;
                case "AISide 5":
                    DiceNumberText.aiNumber = 6;
                    break;
                case "AISide 6":
                    DiceNumberText.aiNumber = 5;
                    break;
                    
            }

        }
    }
}
