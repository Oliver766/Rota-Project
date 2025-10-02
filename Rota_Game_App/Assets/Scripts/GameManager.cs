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
    [Header("Game settings")]
    public GameObject tokenPrefab;
    public List<Node> allNodes;
    public AIDifficulty aiDifficulty = AIDifficulty.Medium;

    [Header("Managers")]
    public RotaAIManager aiManager;
    public StartDeterminer StartDeterminer;
    public CurrencyManager currencyManager;

    [Header("Bools")]
    public bool isAIEnabled = true;
    public bool IsPlacingPhase = true;
    public bool gameOver = false;
    public bool nodeHighlightEnabled = true;

    [Header("Game Ints")]
    public int currentPlayer = 0;
    public int aiPlayerID = 1;
    private int tokensPlaced = 0;
    public int winnerID = -1;
    private int? lastWinnerID = null;

    [Header("Game Texts")]
    public TextMeshProUGUI turnText;

    private Token selectedToken;
    public static GameManager Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartDeterminer.OnStartPlayerDecided.AddListener(HandleStartPlayer);

        foreach (Node node in allNodes)
        {
            string connectedNames = string.Join(", ", node.connectedNodes.Select(n => n.name));
            Debug.Log($"{node.name} connected to: {connectedNames}");
        }

        UpdateTurnText();
        HighlightValidPlacementNodes();
        StartCoroutine(DelayedStart());

     
    }

    IEnumerator DelayedStart()
    {
    yield return new WaitForSeconds(0.1f);
    StartDeterminer.DetermineStarter();
    }



    void HandleStartPlayer(PlayerType starter)
    {
        currentPlayer = (starter == PlayerType.Human) ? 0 : aiPlayerID;

        if (starter == PlayerType.Human)
        {
            Debug.Log("Human starts the game.");
            UpdateTurnText();
        }
        else
        {
            Debug.Log("AI starts the game.");
            UpdateTurnText();
            StartCoroutine(aiManager.SafeExecuteAITurn());
        }
    }

    #region Core Game Logic

    public void PlaceToken(Node node)
    {
        if (node.isOccupied || gameOver || !IsPlacingPhase) return;

        GameObject tokenObj = Instantiate(tokenPrefab, node.transform.position, Quaternion.identity);
        Token token = tokenObj.GetComponent<Token>();
        token.ownerID = currentPlayer;

        SpriteRenderer sr = tokenObj.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = currentPlayer == 0 ? Color.red : Color.blue;

        token.transform.parent = node.transform;
        node.isOccupied = true;
        node.currentToken = token;

        tokensPlaced++;
        if (tokensPlaced >= 6)
        {
            IsPlacingPhase = false;
            ClearNodeHighlights();
               currencyManager.AddCurrency("DE", 100); // Example: Add 100 Gold at game start

            currentPlayer = 1 - currentPlayer;
            UpdateTurnText();

            if (!gameOver && isAIEnabled && currentPlayer == aiPlayerID)
            {
                StartCoroutine(aiManager.SafeExecuteAITurn());
                return;
            }

            return;
        }

        CheckWinCondition();
        ClearNodeHighlights();
        ClearTokenHighlights();

        currentPlayer = 1 - currentPlayer;
        UpdateTurnText();

        if (!gameOver)
        {
            if (IsPlacingPhase)
                HighlightValidPlacementNodes();

            if (isAIEnabled && currentPlayer == aiPlayerID)
                StartCoroutine(aiManager.SafeExecuteAITurn());
        }
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

        token.SetSelectedHighlight(false);
        token.Deselect();
        selectedToken = null;

        CheckWinCondition();
        ClearNodeHighlights();
        ClearTokenHighlights();

        currentPlayer = 1 - currentPlayer;
        UpdateTurnText();

        if (!gameOver)
        {
            if (IsPlacingPhase)
                HighlightValidPlacementNodes();

            if (isAIEnabled && currentPlayer == aiPlayerID)
                StartCoroutine(aiManager.SafeExecuteAITurn());
        }
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
                    ClearTokenHighlights();
                    selectedToken = node.currentToken;
                    selectedToken.Select();
                    selectedToken.SetSelectedHighlight(true);
                    HighlightValidMovementTargets(selectedToken);
                }
            }
            else
            {
                if (!node.isOccupied && IsConnected(selectedToken, node))
                {
                    MoveToken(selectedToken, node);
                }
                else
                {
                    selectedToken.SetSelectedHighlight(false);
                    selectedToken.Deselect();
                    selectedToken = null;
                    ClearNodeHighlights();
                    ClearTokenHighlights();
                }
            }
        }
    }

    public bool IsConnected(Token token, Node targetNode)
    {
        if (token == null || token.transform.parent == null) return false;
        Node currentNode = token.transform.parent.GetComponent<Node>();
        return currentNode != null && currentNode.connectedNodes.Contains(targetNode);
    }

    public void EndGame(int winnerID)
    {
        gameOver = true;
        lastWinnerID = winnerID;
        ClearNodeHighlights();
        ClearTokenHighlights();
        UpdateTurnText();
        Debug.Log($"Player {winnerID + 1} wins!");
    }

    #endregion

    #region Win Condition & AI Helpers

    public void CheckWinCondition()
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

    public Node FindWinningPlacement(int playerID)
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

    public Node FindBlockingPlacement(int opponentID)
    {
        return FindWinningPlacement(opponentID);
    }

    public Node FindRandomEmptyNode()
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

    public bool IsWinningLine(int playerID)
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

    public List<int[]> GetWinningLines()
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

    #endregion

    #region UI and Highlighting

    public void HighlightValidPlacementNodes()
    {
        if (!nodeHighlightEnabled) return;

        foreach (Node node in allNodes)
            node.Highlight(!node.isOccupied && IsPlacingPhase);
    }

    public void HighlightValidMovementTargets(Token token)
    {
        if (!nodeHighlightEnabled || token == null || token.transform.parent == null) return;

        Node currentNode = token.transform.parent.GetComponent<Node>();
        foreach (Node node in allNodes)
            node.Highlight(currentNode.connectedNodes.Contains(node) && !node.isOccupied);
    }


   public void HighlightMovableTokens()
    {
        foreach (Node node in allNodes)
        {
            Token token = node.currentToken;
            if (token != null && token.ownerID == currentPlayer)
            {
                bool canMove = node.connectedNodes.Any(n => !n.isOccupied);
                token.SetSelectedHighlight(canMove);
            }
        }
    }

   public void ClearNodeHighlights()
    {
        foreach (Node node in allNodes)
            node.Highlight(false);
    }

    public void ToggleNodeHighlight(bool state)
    {
        Debug.Log($"Toggling node highlight: {state}");
        Debug.Log($"Node highlight enabled: {nodeHighlightEnabled}");

        nodeHighlightEnabled = state;
        ClearNodeHighlights();
        if (state)
        {
            if (IsPlacingPhase)
                HighlightValidPlacementNodes();
            else if (selectedToken != null)
                HighlightValidMovementTargets(selectedToken);
        }

    }

   public void ClearTokenHighlights()
    {
        foreach (Node node in allNodes)
        {
            Token token = node.currentToken;
            if (token != null)
                token.SetSelectedHighlight(false); // resets to owner color
        }
    }

    public void UpdateTurnText()
    {
        if (gameOver)
        {
            if (lastWinnerID.HasValue)
            {
                turnText.text = $"Player {lastWinnerID.Value + 1} wins!";
                Debug.Log($"Turn text updated to: Player {lastWinnerID.Value + 1} wins!");
            }
            else
            {
                turnText.text = "Game Over";
                Debug.LogWarning("Game over triggered but winnerID was not set.");
            }

            return;
        }

        turnText.text = $"Player {(currentPlayer == 0 ? "I" : "II")}'s Turn";
    }


    #endregion
   
}



