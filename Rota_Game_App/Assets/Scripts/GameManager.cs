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
    [Header("Game Settings")]
    public GameObject tokenPrefab;
    public List<Node> allNodes;
    public AIDifficulty aiDifficulty = AIDifficulty.Medium;

    [Header("Managers")]
    public RotaAIManager aiManager;
    public StartDeterminer startDeterminer;
    public CurrencyManager currencyManager;
    public AchievementManager achievementManager;

    [Header("State Flags")]
    public bool isAIEnabled = true;
    public bool isPlacingPhase = true;
    public bool gameOver = false;
    public bool nodeHighlightEnabled = true;
    public bool aiControlsPlayerStart = true; // AI plays first move for Player 0


    [Header("Game State")]
    public int currentPlayer = 0;
    public int aiPlayerID = 1;
    public int tokensPlaced = 0;
    public int winnerID = -1;
    private int? lastWinnerID = null;

    [Header("UI")]
    public TextMeshProUGUI turnText;

    private Token selectedToken;
    public static GameManager Instance;

    void Awake() => Instance = this;

    void Start()
    {
        startDeterminer.OnStartPlayerDecided.AddListener(HandleStartPlayer);
        UpdateTurnText();
        HighlightValidPlacementNodes();
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.1f);
        startDeterminer.DetermineStarter();
    }

    void HandleStartPlayer(PlayerType starter)
    {
        currentPlayer = (starter == PlayerType.Human) ? 0 : aiPlayerID;
        UpdateTurnText();

        if (starter == PlayerType.AI || aiControlsPlayerStart)
        {
            StartCoroutine(aiManager.SafeExecuteAITurn());
        }
    }



    public void PlaceToken(Node node)
    {
        if (node.isOccupied || gameOver || !isPlacingPhase) return;

        GameObject tokenObj = Instantiate(tokenPrefab, node.transform.position, Quaternion.identity);
        Token token = tokenObj.GetComponent<Token>();
        token.ownerID = currentPlayer;

        tokenObj.GetComponent<SpriteRenderer>().color = currentPlayer == 0 ? Color.red : Color.blue;
        token.transform.parent = node.transform;
        node.isOccupied = true;
        node.currentToken = token;

        tokensPlaced++;
        if (currentPlayer == 0 && IsStrategicPlacement(node))
        {
            currencyManager.AddCurrency("DE", 25);
            achievementManager.RegisterProgress("perfect_turn");
        }

        if (tokensPlaced >= 6)
        {
            isPlacingPhase = false;

        }

        CheckWinCondition();
        ClearNodeHighlights();
        ClearTokenHighlights();

        currentPlayer = 1 - currentPlayer;

        if (aiControlsPlayerStart && currentPlayer == aiPlayerID)
            aiControlsPlayerStart = false;



        UpdateTurnText();

        if (!gameOver && isAIEnabled && currentPlayer == aiPlayerID)
            StartCoroutine(aiManager.SafeExecuteAITurn());
        else if (!gameOver && isPlacingPhase)
            HighlightValidPlacementNodes();
    }

    public void MoveToken(Token token, Node newNode)
    {
        if (token == null || newNode == null || newNode.isOccupied || gameOver || !IsConnected(token, newNode)) return;

        Node oldNode = token.transform.parent.GetComponent<Node>();
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

        // Reset AI override after first move
        if (aiControlsPlayerStart && currentPlayer == aiPlayerID)
            aiControlsPlayerStart = false;

        UpdateTurnText();

        if (!gameOver && isAIEnabled && currentPlayer == aiPlayerID)
            StartCoroutine(aiManager.SafeExecuteAITurn());
        else if (!gameOver && isPlacingPhase)
            HighlightValidPlacementNodes();
    }

    public void HandleNodeClick(Node node)
    {
        if (gameOver) return;

        if (isPlacingPhase)
        {
            if (currentPlayer == aiPlayerID) return;
            PlaceToken(node);
        }
        else
        {
            if (selectedToken == null && node.isOccupied && node.currentToken.ownerID == currentPlayer)
            {
                selectedToken = node.currentToken;
                selectedToken.Select();
                selectedToken.SetSelectedHighlight(true);
                HighlightValidMovementTargets(selectedToken);
            }
            else if (selectedToken != null && !node.isOccupied && IsConnected(selectedToken, node))
            {
                MoveToken(selectedToken, node);
            }
            else
            {
                selectedToken?.Deselect();
                selectedToken = null;
                ClearNodeHighlights();
                ClearTokenHighlights();
            }
        }
    }

    public void EndGame(int winnerID)
    {
        gameOver = true;
        lastWinnerID = winnerID;
        ClearNodeHighlights();
        ClearTokenHighlights();
        UpdateTurnText();

        if (winnerID == 0)
        {
            currencyManager.AddCurrency("DE", 100);
            achievementManager.RegisterProgress("win_game");
        }
    }

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

    public bool IsConnected(Token token, Node targetNode)
    {
        Node currentNode = token.transform.parent.GetComponent<Node>();
        return currentNode != null && currentNode.connectedNodes.Contains(targetNode);
    }

    public bool IsStrategicPlacement(Node node)
    {
        return FindWinningPlacement(currentPlayer) == node || FindBlockingPlacement(1 - currentPlayer) == node;
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
                if (node.isOccupied && node.currentToken.ownerID == playerID)
                    count++;
                else if (!node.isOccupied)
                    emptyNode = node;
            }

            if (count == 2 && emptyNode != null)
                return emptyNode;
        }

        return null;
    }

    public Node FindBlockingPlacement(int opponentID) => FindWinningPlacement(opponentID);

    public Node FindRandomEmptyNode()
    {
        var emptyNodes = allNodes.Where(n => !n.isOccupied).ToList();
        return emptyNodes.Count > 0 ? emptyNodes[Random.Range(0, emptyNodes.Count)] : null;
    }

    public bool IsWinningLine(int playerID)
    {
        return GetWinningLines().Any(line =>
        {
            Token t0 = allNodes[line[0]].currentToken;
            Token t1 = allNodes[line[1]].currentToken;
            Token t2 = allNodes[line[2]].currentToken;
            return t0 != null && t1 != null && t2 != null &&
                   t0.ownerID == playerID && t1.ownerID == playerID && t2.ownerID == playerID;
        });
    }

    public List<int[]> GetWinningLines() => new()
    {
        new[] {0, 1, 2}, new[] {3, 4, 5}, new[] {6, 7, 8},
        new[] {0, 3, 6}, new[] {1, 4, 7}, new[] {2, 5, 8},
        new[] {0, 4, 8}, new[] {2, 4, 6}
    };

    public void HighlightValidPlacementNodes()
    {
        if (!nodeHighlightEnabled) return;
        foreach (Node node in allNodes)
            node.Highlight(!node.isOccupied && isPlacingPhase);
    }

    public void HighlightValidMovementTargets(Token token)
    {
        if (!nodeHighlightEnabled || token == null || token.transform.parent == null) return;
        Node currentNode = token.transform.parent.GetComponent<Node>();
        foreach (Node node in allNodes)
            node.Highlight(currentNode.connectedNodes.Contains(node) && !node.isOccupied);
    }

    public void ClearNodeHighlights() => allNodes.ForEach(n => n.Highlight(false));
    public void ClearTokenHighlights() => allNodes.ForEach(n => n.currentToken?.SetSelectedHighlight(false));

    public void UpdateTurnText()
    {
        turnText.text = gameOver
            ? lastWinnerID.HasValue ? $"Player {lastWinnerID.Value + 1} wins!" : "Game Over"
            : $"Player {(currentPlayer == 0 ? "I" : "II")}'s Turn";
    }
}






