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

    private Token selectedToken;

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

    [Header("Timer")]
    public float moveTimeLimit = 10f; // seconds
    private float moveTimer = 0f;
    private bool isMoveTimerActive = false;



    [Header("Game Texts")]
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI moveTimerText; // ← for numeric countdown

    public static GameManager Instance;

    public StartDeterminer StartDeterminer;
    void Awake()
    {
        Instance = this;
        // Subscribe to the event
        StartDeterminer.OnStartPlayerDecided.AddListener(HandleStartPlayer);
        
    } 
     


    void Start()
    {
        // Confirm node connections
        foreach (Node node in allNodes)
        {
            string connectedNames = string.Join(", ", node.connectedNodes.Select(n => n.name));
            Debug.Log($"{node.name} connected to: {connectedNames}");
        }
        UpdateTurnText();
        HighlightValidPlacementNodes();
        StartDeterminer.DetermineStarter();

    }


    void HandleStartPlayer(PlayerType starter)
    {
        // ✅ Fix 1: Set currentPlayer based on starter
        currentPlayer = (starter == PlayerType.Human) ? 0 : aiPlayerID;


        if (starter == PlayerType.Human)
        {
            Debug.Log("Human starts the game.");
            UpdateTurnText();

        }
        else
        {
            Debug.Log("AI starts the game.");
            // Trigger AI turn logic
            StartCoroutine(SafeExecuteAITurn());
        }
    }


    void Update()
    {
               if (isMoveTimerActive)
        {
            moveTimer -= Time.deltaTime;

            if (moveTimerText != null)
                moveTimerText.text = $"⏱ {Mathf.CeilToInt(moveTimer)}s";

            if (moveTimer <= 0f)
                CancelMoveDueToTimeout();
        }
        else
        {
            if (moveTimerText != null)
                moveTimerText.text = "";
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
            sr.color = (currentPlayer == 0 ? Color.red : Color.blue);

        token.transform.parent = node.transform;
        node.isOccupied = true;
        node.currentToken = token;

        tokensPlaced++;
        if (tokensPlaced >= 6)
            IsPlacingPhase = false;

        CheckWinCondition();
        ClearNodeHighlights();
        ClearTokenHighlights();

        currentPlayer = 1 - currentPlayer;
        UpdateTurnText();

        if (!gameOver)
        {
            if (IsPlacingPhase)
                HighlightValidPlacementNodes();

            if (currentPlayer != aiPlayerID)
            {
                moveTimer = moveTimeLimit;
                isMoveTimerActive = true;
            }

            if (isAIEnabled && currentPlayer == aiPlayerID)
                StartCoroutine(SafeExecuteAITurn());
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

        isMoveTimerActive = false; // ✅ stop timer
        if (moveTimerText != null)
            moveTimerText.text = "";

        CheckWinCondition();
        ClearNodeHighlights();
        ClearTokenHighlights();

        currentPlayer = 1 - currentPlayer;
        UpdateTurnText();

        if (!gameOver)
        {
            if (IsPlacingPhase)
                HighlightValidPlacementNodes();

            if (currentPlayer != aiPlayerID)
            {
                moveTimer = moveTimeLimit;
                isMoveTimerActive = true;
            }

            if (isAIEnabled && currentPlayer == aiPlayerID)
                StartCoroutine(SafeExecuteAITurn());
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

                    moveTimer = moveTimeLimit;
                    isMoveTimerActive = true;
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



    bool IsConnected(Token token, Node targetNode)
    {
        if (token == null || token.transform.parent == null) return false;
        Node currentNode = token.transform.parent.GetComponent<Node>();
        return currentNode != null && currentNode.connectedNodes.Contains(targetNode);
    }


    void EndGame(int winnerID)
    {
       gameOver = true;
        lastWinnerID = winnerID;
        ClearNodeHighlights();
        ClearTokenHighlights();
        isMoveTimerActive = false;
        if (moveTimerText != null)
            moveTimerText.text = "";
        UpdateTurnText();
        Debug.Log($"Player {winnerID + 1} wins!");

    }
    #endregion



    #region Win Condition & AI Helpers

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
# endregion

    #region UI  and Highlighting


    /// <summary>
    /// UI Related code for highlighting nodes during placement and movement phases.
    /// </summary>
    

    void HighlightValidPlacementNodes()
    {
        if (!nodeHighlightEnabled) return;

        foreach (Node node in allNodes)
            node.Highlight(!node.isOccupied && IsPlacingPhase);
    }

    void HighlightValidMovementTargets(Token token)
    {
        if (!nodeHighlightEnabled || token == null || token.transform.parent == null) return;

        Node currentNode = token.transform.parent.GetComponent<Node>();
        foreach (Node node in allNodes)
            node.Highlight(currentNode.connectedNodes.Contains(node) && !node.isOccupied);
    }

    void HighlightMovableTokens()
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

    void ClearNodeHighlights()
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

    void ClearTokenHighlights()
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


    void CancelMoveDueToTimeout()
{
    isMoveTimerActive = false;

    if (selectedToken != null)
    {
        selectedToken.SetSelectedHighlight(false);
        selectedToken.Deselect();
        selectedToken = null;
    }

    ClearNodeHighlights();
    ClearTokenHighlights();

    if (moveTimerText != null)
        moveTimerText.text = "";

    Debug.Log($"Player {currentPlayer + 1} timed out.");

    // ✅ Advance turn after timeout
    currentPlayer = 1 - currentPlayer;
    UpdateTurnText();

    if (!gameOver)
    {
        if (IsPlacingPhase)
            HighlightValidPlacementNodes();

        if (currentPlayer == aiPlayerID && isAIEnabled)
            StartCoroutine(SafeExecuteAITurn());
        else
        {
            moveTimer = moveTimeLimit;
            isMoveTimerActive = true;
        }
    }
    }
    #endregion
    #region AI Logic
   IEnumerator ExecuteAITurn()
{
    float thinkTime = Random.Range(1.5f, 3.5f); // ⏳ AI "thinking" delay
    yield return new WaitForSeconds(thinkTime);

    if (gameOver) yield break;

    try
    {
        if (IsPlacingPhase)
            AITokenPlacement();
        else
            AITokenMovement();
    }
    catch (System.Exception ex)
    {
        Debug.LogError($"AI execution failed: {ex.Message}\n{ex.StackTrace}");
        yield break;
    }

    UpdateTurnText();
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
    #endregion


}



