using UnityEngine;

public class Token : MonoBehaviour
{
     public int ownerID;
    public bool isSelected = false;

      public void Select()
      {
            isSelected = true;
                if (transform.parent == null)
            {
                  Debug.LogWarning("Token has no parent—cannot highlight connected nodes.");
                  return;
            }

            

            // Optional: change color or scale for feedback
            Node currentNode = transform.parent.GetComponent<Node>();
            if (currentNode != null)
            {
                  Debug.LogWarning("Token's parent does not have a Node component.");
                  return;
            }


                  foreach (Node connected in currentNode.connectedNodes)
            {
                        if (!connected.isOccupied)
                              connected.SetHighlight(true);
                        Debug.Log($"Highlighting Node: {connected.name}");

            }
      }

      

      public void Deselect()
      {
            isSelected = false;
            // Reset visual feedback
            Node currentNode = transform.parent.GetComponent<Node>();
            if (currentNode != null)
            {
                  foreach (Node connected in currentNode.connectedNodes)
                  {
                        connected.SetHighlight(false);
            Debug.Log($"Highlighting Node: {connected.name}");

            }
      }

    }


}
