using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class DiceCheckZone : MonoBehaviour
{
    Vector3 diceVelocityAtEnter;

    void FixedUpdate()
    {
        diceVelocityAtEnter = DiceScript.diceVelocity;
    }

    void OnTriggerStay(Collider col)
    {
        if(diceVelocityAtEnter.x == 0f && diceVelocityAtEnter.y == 0f && diceVelocityAtEnter.z ==0f){
            switch (col.gameObject.name)
            {
                case "Side 1":
                    DiceNumberText.diceNumber = 1;
                    break;
                case "Side 2":
                    DiceNumberText.diceNumber = 2;
                    break;
                case "Side 3":
                    DiceNumberText.diceNumber = 4;
                    break;
                case "Side 4":
                    DiceNumberText.diceNumber = 3;
                    break;
                case "Side 5":
                    DiceNumberText.diceNumber = 6;
                    break;
                case "Side 6":
                    DiceNumberText.diceNumber = 5;
                    break;
                    
            }

        }
    }
}
