using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using TMPro;

public enum AIDifficulty
{
    Easy,
    Medium,
    Hard
}

public class GameManager : MonoBehaviour
{
    public GameObject tokenPrefab;
    public List<Node> allNodes;
    public int currentPlayer = 0;
    public int aiPlayerID = 1;
    public bool isAIEnabled = true;
    public bool IsPlacingPhase = true;
    public bool gameOver = false;

    private int tokensPlaced = 0;
    private Token selectedToken;
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI WinText;

    public AIDifficulty aiDifficulty = AIDifficulty.Medium;



    public static GameManager Instance;
    void Awake() => Instance = this;

    void Start()
    {
        // Confirm node connections
        foreach (Node node in allNodes)
        {
            string connectedNames = string.Join(", ", node.connectedNodes.Select(n => n.name));
            Debug.Log($"{node.name} connected to: {connectedNames}");
        }
        WinText.text = "";
        turnText.text = $"Player {(currentPlayer == 0 ? "I" : "II")}'s Turn";
    }


    public void PlaceToken(Node node)
    {
        if (node.isOccupied || gameOver || !IsPlacingPhase) return;

        GameObject tokenObj = Instantiate(tokenPrefab, node.transform.position, Quaternion.identity);
        Token token = tokenObj.GetComponent<Token>();
        token.ownerID = currentPlayer;
 


        SpriteRenderer sr = tokenObj.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = (currentPlayer == 0) ? Color.red : Color.blue;

        token.transform.parent = node.transform;
        node.isOccupied = true;
        node.currentToken = token;

        tokensPlaced++;
        if (tokensPlaced >= 6)
            IsPlacingPhase = false;
          

            

        CheckWinCondition();
        ClearNodeHighlights();

        currentPlayer = 1 - currentPlayer;
          if (IsPlacingPhase)
             HighlightValidPlacementNodes();


        turnText.text = $"Player {(currentPlayer == 0 ? "I" : "II")}'s Turn";

        if (isAIEnabled && currentPlayer == aiPlayerID && !gameOver)
            StartCoroutine(SafeExecuteAITurn());

    }
    
     public void HighlightValidPlacementNodes()
    {
    foreach (Node node in allNodes)
        node.Highlight(!node.isOccupied && IsPlacingPhase);
    }
     public void HighlightValidMovementTargets(Token token)
    {
    Node currentNode = token.transform.parent.GetComponent<Node>();
    foreach (Node node in allNodes)
        node.Highlight(currentNode.connectedNodes.Contains(node) && !node.isOccupied);
    }
    public void ClearNodeHighlights()
    {
    foreach (Node node in allNodes)
        node.Highlight(false);
    }






    public void MoveToken(Token token, Node newNode)
    {
        if (token == null || newNode == null || newNode.isOccupied || gameOver || !IsConnected(token, newNode))
        {
            Debug.LogError("Invalid move attempt.");
            return;
        }

        Node oldNode = token.transform.parent?.GetComponent<Node>();
        if (oldNode == null)
        {
            Debug.LogWarning("Token's parent node is missing.");
            return;
        }

        oldNode.isOccupied = false;
        oldNode.currentToken = null;

        newNode.isOccupied = true;
        newNode.currentToken = token;

        token.transform.position = newNode.transform.position;
        token.transform.parent = newNode.transform;

        token.Deselect();
        selectedToken = null;
        ClearNodeHighlights(); // ← clears old highlights
        HighlightValidPlacementNodes(); // ← shows new movable tokens for next player

       


        CheckWinCondition();

        if (IsPlacingPhase)
        HighlightValidPlacementNodes();


        currentPlayer = 1 - currentPlayer;

        turnText.text = $"Player {(currentPlayer == 0 ? "I" : "II")}'s Turn";


        if (isAIEnabled && currentPlayer == aiPlayerID && !gameOver)
            StartCoroutine(SafeExecuteAITurn());

    }

    public void HandleNodeClick(Node node)
    {
        if (gameOver) return;

        if (IsPlacingPhase)
        {
            if (currentPlayer == aiPlayerID) return;
            PlaceToken(node);
        }
        else
        {
            if (selectedToken == null)
            {

                if (node.isOccupied && node.currentToken.ownerID == currentPlayer)
                {
                    selectedToken = node.currentToken;
                    selectedToken.Select();
                    HighlightValidMovementTargets(selectedToken);

                }
            }
            else
            {
                if (!node.isOccupied && IsConnected(selectedToken, node))
                    MoveToken(selectedToken, node);
                else
                    Debug.Log("Invalid move or node occupied");
            }
        }

    }

    bool IsConnected(Token token, Node targetNode)
    {
        if (token == null || token.transform.parent == null) return false;
        Node currentNode = token.transform.parent.GetComponent<Node>();
        return currentNode != null && currentNode.connectedNodes.Contains(targetNode);
    }



    void CheckWinCondition()
    {
        foreach (int[] line in GetWinningLines())
        {
            Token t0 = allNodes[line[0]].currentToken;
            Token t1 = allNodes[line[1]].currentToken;
            Token t2 = allNodes[line[2]].currentToken;

            if (t0 != null && t1 != null && t2 != null &&
                t0.ownerID == t1.ownerID && t1.ownerID == t2.ownerID)
            {
                EndGame(t0.ownerID);
                return;
            }
        }

    }

    void EndGame(int winnerID)
    {
        gameOver = true;
        Debug.Log($"Player {winnerID + 1} wins!");
        WinText.text = $"Player {(winnerID == 0 ? "I" : "II")} Wins!";
        turnText.text = "";
        turnText.gameObject.SetActive(false);
        // TODO: Trigger win banner, sound, restart button

    }

  


    IEnumerator ExecuteAITurn()
    {
        yield return new WaitForSeconds(0.5f);

        if (gameOver) yield break;

        if (IsPlacingPhase)
            AITokenPlacement();
        else
            AITokenMovement();


        turnText.text = $"Player {(currentPlayer == 0 ? "I" : "II")}'s Turn";
    }


            
    

    IEnumerator SafeExecuteAITurn()
    {
        IEnumerator aiRoutine = ExecuteAITurn();
        while (true)
        {
            try
            {
                if (!aiRoutine.MoveNext()) yield break;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"AI Turn crashed: {ex.Message}\n{ex.StackTrace}");
                yield break;
            }

            yield return aiRoutine.Current;
        }
    }


    void AITokenPlacement()
    {
        Node bestNode = aiDifficulty switch
        {
            AIDifficulty.Hard => FindWinningPlacement(aiPlayerID)
                              ?? FindBlockingPlacement(1 - aiPlayerID)
                              ?? FindRandomEmptyNode(),
            AIDifficulty.Medium => FindBlockingPlacement(1 - aiPlayerID)
                                ?? FindRandomEmptyNode(),
            _ => FindRandomEmptyNode()
        };

        if (bestNode != null)
         bestNode.Highlight(true); // show where AI will place
         PlaceToken(bestNode);

    }

    void AITokenMovement()
    {

        switch (aiDifficulty)
        {
            case AIDifficulty.Hard: TryWinOrBlockOrMove(); break;
            case AIDifficulty.Medium: TryBlockOrMoveRandom(); break;
            default: TryEasyMove(); break;
        }
    }

    void TryEasyMove()
    {
        foreach (Node node in allNodes)
        {
            if (node.isOccupied && node.currentToken.ownerID == aiPlayerID)
            {
                foreach (Node target in node.connectedNodes)
                {
                    if (!target.isOccupied)
                    {
                        MoveToken(node.currentToken, target);
                        return;
                    }
                }
            }
        }
        Debug.Log("AI could not find a valid move.");
    }



    Node FindWinningPlacement(int playerID)
    {
        foreach (int[] line in GetWinningLines())
        {
            int count = 0;
            Node emptyNode = null;

            foreach (int index in line)
            {
                Node node = allNodes[index];
                if (node.isOccupied)
                {
                    if (node.currentToken.ownerID == playerID)
                        count++;
                }
                else
                {
                    emptyNode = node;
                }
            }

            if (count == 2 && emptyNode != null)
                return emptyNode;
        }

        return null;
    }

    Node FindBlockingPlacement(int opponentID)
    {
        return FindWinningPlacement(opponentID);
    }

    Node FindRandomEmptyNode()
    {
        List<Node> emptyNodes = new List<Node>();
        foreach (Node node in allNodes)
        {
            if (!node.isOccupied)
                emptyNodes.Add(node);
        }

        if (emptyNodes.Count > 0)
            return emptyNodes[UnityEngine.Random.Range(0, emptyNodes.Count)];

        return null;
    }

    bool IsWinningLine(int playerID)
    {
        foreach (int[] line in GetWinningLines())
        {
            Token t0 = allNodes[line[0]].currentToken;
            Token t1 = allNodes[line[1]].currentToken;
            Token t2 = allNodes[line[2]].currentToken;

            if (t0 != null && t1 != null && t2 != null)
            {
                if (t0.ownerID == playerID && t1.ownerID == playerID && t2.ownerID == playerID)
                    return true;
            }
        }

        return false;
    }

    List<int[]> GetWinningLines()
    {
        return new List<int[]>
        {
            new int[] {0, 1, 2},
            new int[] {3, 4, 5},
            new int[] {6, 7, 8},
            new int[] {0, 3, 6},
            new int[] {1, 4, 7},
            new int[] {2, 5, 8},
            new int[] {0, 4, 8},
            new int[] {2, 4, 6}
        };
    }

    void TryBlockOrMoveRandom()
    {
        foreach (Node node in allNodes)
        {
            if (node.isOccupied && node.currentToken.ownerID == aiPlayerID)
            {
                Token token = node.currentToken;
                foreach (Node target in node.connectedNodes)
                {
                    if (!target.isOccupied)
                    {
                        Node originalNode = token.transform.parent.GetComponent<Node>();
                        SimulateMove(token, target);

                        if (IsWinningLine(1 - aiPlayerID))
                        {
                            UndoSimulatedMove(token, originalNode, target);
                            MoveToken(token, target);
                            return;
                        }

                        UndoSimulatedMove(token, originalNode, target);
                    }
                }
            }
        }

        TryEasyMove(); // fallback to easy
    }

    void TryWinOrBlockOrMove()
    {
        foreach (Node node in allNodes)
        {
            if (node.isOccupied && node.currentToken.ownerID == aiPlayerID)
            {
                Token token = node.currentToken;
                foreach (Node target in node.connectedNodes)
                {
                    if (!target.isOccupied)
                    {
                        Node originalNode = token.transform.parent.GetComponent<Node>();
                        SimulateMove(token, target);

                        if (IsWinningLine(aiPlayerID))
                        {
                            UndoSimulatedMove(token, originalNode, target);
                            MoveToken(token, target);
                            return;
                        }

                        UndoSimulatedMove(token, originalNode, target);
                    }
                }
            }
        }

        TryBlockOrMoveRandom(); // fallback

    }

    void SimulateMove(Token token, Node target)
    {
        Node original = token.transform.parent.GetComponent<Node>();
        original.isOccupied = false;
        original.currentToken = null;

        target.isOccupied = true;
        target.currentToken = token;
        // No reparenting or position change during simulation

    }

    void UndoSimulatedMove(Token token, Node original, Node target)
    {
        target.isOccupied = false;
        target.currentToken = null;

        original.isOccupied = true;
        original.currentToken = token;

    }




}


