using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotaAIManager : MonoBehaviour
{
    [Header("References")]
    public GameManager gameManager;

    public IEnumerator SafeExecuteAITurn()
    {
        yield return new WaitForSeconds(Random.Range(1.5f, 3.5f)); // ⏳ AI "thinking" delay

        if (gameManager.gameOver) yield break;

        try
        {
            if (gameManager.isPlacingPhase)
                ExecutePlacement();
            else
                ExecuteMovement();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"AI execution failed: {ex.Message}\n{ex.StackTrace}");
        }

        gameManager.UpdateTurnText();
    }

    private void ExecutePlacement()
    {
        Node bestNode = gameManager.aiDifficulty switch
        {
            AIDifficulty.Hard => FindWinningPlacement(gameManager.aiPlayerID)
                              ?? FindBlockingPlacement(1 - gameManager.aiPlayerID)
                              ?? FindRandomEmptyNode(),

            AIDifficulty.Medium => FindBlockingPlacement(1 - gameManager.aiPlayerID)
                                ?? FindRandomEmptyNode(),

            _ => FindRandomEmptyNode()
        };

        if (bestNode != null)
        {
            bestNode.Highlight(true); // Optional visual cue
            gameManager.PlaceToken(bestNode);
        }
        else
        {
            Debug.LogWarning("AI could not find a valid placement.");
        }
    }

    private void ExecuteMovement()
    {
        switch (gameManager.aiDifficulty)
        {
            case AIDifficulty.Hard:
                TryWinOrBlockOrMove();
                break;
            case AIDifficulty.Medium:
                TryBlockOrMoveRandom();
                break;
            default:
                TryEasyMove();
                break;
        }
    }

    private void TryEasyMove()
    {
        foreach (Node node in gameManager.allNodes)
        {
            if (node.isOccupied && node.currentToken.ownerID == gameManager.aiPlayerID)
            {
                foreach (Node target in node.connectedNodes)
                {
                    if (!target.isOccupied)
                    {
                        gameManager.MoveToken(node.currentToken, target);
                        return;
                    }
                }
            }
        }

        Debug.Log("AI could not find a valid move.");
    }

    private void TryBlockOrMoveRandom()
    {
        foreach (Node node in gameManager.allNodes)
        {
            if (node.isOccupied && node.currentToken.ownerID == gameManager.aiPlayerID)
            {
                Token token = node.currentToken;
                foreach (Node target in node.connectedNodes)
                {
                    if (!target.isOccupied)
                    {
                        Node originalNode = token.transform.parent.GetComponent<Node>();
                        SimulateMove(token, target);

                        if (gameManager.IsWinningLine(1 - gameManager.aiPlayerID))
                        {
                            UndoSimulatedMove(token, originalNode, target);
                            gameManager.MoveToken(token, target);
                            return;
                        }

                        UndoSimulatedMove(token, originalNode, target);
                    }
                }
            }
        }

        TryEasyMove();
    }

    private void TryWinOrBlockOrMove()
    {
        foreach (Node node in gameManager.allNodes)
        {
            if (node.isOccupied && node.currentToken.ownerID == gameManager.aiPlayerID)
            {
                Token token = node.currentToken;
                foreach (Node target in node.connectedNodes)
                {
                    if (!target.isOccupied)
                    {
                        Node originalNode = token.transform.parent.GetComponent<Node>();
                        SimulateMove(token, target);

                        if (gameManager.IsWinningLine(gameManager.aiPlayerID))
                        {
                            UndoSimulatedMove(token, originalNode, target);
                            gameManager.MoveToken(token, target);
                            return;
                        }

                        UndoSimulatedMove(token, originalNode, target);
                    }
                }
            }
        }

        TryBlockOrMoveRandom();
    }

    private void SimulateMove(Token token, Node target)
    {
        Node original = token.transform.parent.GetComponent<Node>();
        original.isOccupied = false;
        original.currentToken = null;

        target.isOccupied = true;
        target.currentToken = token;
    }

    private void UndoSimulatedMove(Token token, Node original, Node target)
    {
        target.isOccupied = false;
        target.currentToken = null;

        original.isOccupied = true;
        original.currentToken = token;
    }

    private Node FindWinningPlacement(int playerID) => gameManager.FindWinningPlacement(playerID);
    private Node FindBlockingPlacement(int opponentID) => gameManager.FindBlockingPlacement(opponentID);
    private Node FindRandomEmptyNode() => gameManager.FindRandomEmptyNode();
}
