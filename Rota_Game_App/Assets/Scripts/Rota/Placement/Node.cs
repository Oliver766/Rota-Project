using UnityEngine;
using System.Collections.Generic;


public class Node : MonoBehaviour
{
    public bool isOccupied = false;
    public Token currentToken;
    public List<Node> connectedNodes = new List<Node>();

    public void OnMouseDown()
    {
        if (GameManager.Instance.IsPlacingPhase)
        {
            if (!isOccupied)
                GameManager.Instance.PlaceToken(this);
        }
        else
        {
            GameManager.Instance.HandleNodeClick(this);
        }
    }
    
    public void SetHighlight(bool active)
    {
    SpriteRenderer sr = GetComponent<SpriteRenderer>();
    if (sr != null)
    {
        sr.color = active ? Color.yellow : Color.white;
    }
    }




}
