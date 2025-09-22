using UnityEngine;
using System.Collections.Generic;

public class RotaNodeConnector : MonoBehaviour
{
    public GameManager gameManager;


    void Start()
    {
          StartCoroutine(SetupNodes());

    }
    System.Collections.IEnumerator SetupNodes()
    {
        // Wait until all nodes are present
        while (GameObject.Find("Node_0") == null)
            yield return null;

        Node[] nodes = new Node[9];
        for (int i = 0; i < 9; i++)
        {
            GameObject nodeObj = GameObject.Find($"Node_{i}");
            if (nodeObj != null)
                nodes[i] = nodeObj.GetComponent<Node>();
            else
                Debug.LogWarning($"Node_{i} not found");
        }

        // Connect nodes
        Connect(nodes[0], 4, 5, 8);
        Connect(nodes[1], 4, 6, 8);
        Connect(nodes[2], 6, 7, 8);
        Connect(nodes[3], 5, 7, 8);
        Connect(nodes[4], 0, 1, 8);
        Connect(nodes[5], 0, 3, 8);
        Connect(nodes[6], 1, 2, 8);
        Connect(nodes[7], 2, 3, 8);
        Connect(nodes[8], 0, 1, 2, 3, 4, 5, 6, 7);


        // Populate GameManager's allNodes list
        if (gameManager != null)
        {
            gameManager.allNodes = new List<Node>(nodes);
            Debug.Log("Nodes registered with GameManager");
        }
        else
        {
            Debug.LogWarning("GameManager reference not set in RotaNodeConnector.");
        }


      
    }


    void Connect(Node source, params int[] targets)
    {
        foreach (int i in targets)
        {
            if (source != null && i >= 0 && i < 9)
                source.connectedNodes.Add(GameObject.Find($"Node_{i}").GetComponent<Node>());
        }
    }

}
