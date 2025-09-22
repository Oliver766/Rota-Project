using UnityEngine;

public class RotaNodes : MonoBehaviour
{
[Header("Node Settings")]
    public GameObject nodePrefab;       // Assign in Inspector
    public float radius = 5f;
    public Material labelMaterial;      // Optional

    void Start()
    {
        if (nodePrefab == null)
        {
            Debug.LogError("Node prefab not assigned!");
            return;
        }

        // Outer nodes at 45° intervals
        float[] nodeAngles = { 0f, 45f, 90f, 135f, 180f, 225f, 270f, 315f };

        for (int i = 0; i < nodeAngles.Length; i++)
        {
            Vector3 pos = AngleToVector(nodeAngles[i]) * radius;
            CreateNode(pos, i);
        }

        // Center node
        CreateNode(Vector3.zero, 8);
    }

    Vector3 AngleToVector(float angleDeg)
    {
        float rad = Mathf.Deg2Rad * angleDeg;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);
    }

    void CreateNode(Vector3 position, int index)
    {
        GameObject node = Instantiate(nodePrefab, position, Quaternion.identity, transform);
        node.name = $"Node_{index}";

        // Optional label
       // TextMesh label = node.AddComponent<TextMesh>();
      //  label.text = index.ToString();
       // label.characterSize = 0.3f;
       // label.anchor = TextAnchor.MiddleCenter;
       // label.alignment = TextAlignment.Center;
       // label.color = Color.black;
       // label.fontSize = 32;
        //label.transform.localPosition = Vector3.zero;

       // if (labelMaterial != null)
           // label.GetComponent<MeshRenderer>().material = labelMaterial;
    }



}
