using UnityEngine;

public class NodePlacement : MonoBehaviour
{
   public bool isOccupied = false;
    public Token currentToken;

    private void OnMouseDown()
    {
        if (!isOccupied && GameManager.Instance.IsPlacingPhase)
        {
            GameManager.Instance.PlaceToken(this);
        }
    }

}
