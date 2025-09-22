using UnityEngine;

public class GameManager : MonoBehaviour
{
  
    public static GameManager Instance;

    public GameObject tokenPrefab;
    public bool IsPlacingPhase = true;
    private int currentPlayer = 0; // 0 = Player 1, 1 = Player 2
    private int tokensPlaced = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void PlaceToken(NodePlacement node)
    {
        GameObject tokenObj = Instantiate(tokenPrefab, node.transform.position, Quaternion.identity);
        Token token = tokenObj.GetComponent<Token>();
        token.ownerID = currentPlayer;

        node.isOccupied = true;
        node.currentToken = token;

        tokensPlaced++;
        if (tokensPlaced >= 6)
        {
            IsPlacingPhase = false;
            Debug.Log("Switching to movement phase");
        }

        currentPlayer = 1 - currentPlayer; // Switch turn
    }

}
