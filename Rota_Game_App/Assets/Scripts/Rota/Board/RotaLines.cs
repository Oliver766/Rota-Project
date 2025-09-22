using UnityEngine;

public class RotaLines : MonoBehaviour
{
   public float radius = 5f;

    void Start()
    {
        DrawLine(Vector3.up * radius, Vector3.down * radius);     // Vertical
        DrawLine(Vector3.left * radius, Vector3.right * radius);  // Horizontal
        DrawLine(new Vector3(-radius, -radius), new Vector3(radius, radius)); // Diagonal \
        DrawLine(new Vector3(-radius, radius), new Vector3(radius, -radius)); // Diagonal /
    }

    void DrawLine(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("Line");
        lineObj.transform.parent = transform;

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.black;
        lr.endColor = Color.black;
        lr.useWorldSpace = false;
    }




}
