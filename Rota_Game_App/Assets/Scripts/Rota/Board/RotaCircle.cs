using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RotaCircle : MonoBehaviour
{
   [Header("Board Settings")]
    public float radius = 5f;
    public int circleSegments = 100;
    public float lineWidth = 0.05f;
    public Material lineMaterial;

    void Start()
    {
        DrawCircle();
        DrawRadialLines();
    }

    void DrawCircle()
    {
        GameObject circleObj = new GameObject("RotaCircle");
        circleObj.transform.parent = transform;

        LineRenderer lr = circleObj.AddComponent<LineRenderer>();
        lr.positionCount = circleSegments + 1;
        lr.loop = true;
        lr.useWorldSpace = false;
        lr.material = lineMaterial;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;

        for (int i = 0; i <= circleSegments; i++)
        {
            float angle = i * Mathf.Deg2Rad * (360f / circleSegments);
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            lr.SetPosition(i, new Vector3(x, y, 0));
        }
    }

    void DrawRadialLines()
    {
        // Define pairs of angles for opposite points
        float[] anglePairs = { 0f, 180f, 90f, 270f, 45f, 225f, 135f, 315f };

        for (int i = 0; i < anglePairs.Length; i += 2)
        {
            Vector3 start = AngleToVector(anglePairs[i]) * radius;
            Vector3 end = AngleToVector(anglePairs[i + 1]) * radius;
            CreateLine($"Line_{i / 2}", start, end);
        }
    }

    Vector3 AngleToVector(float angleDeg)
    {
        float rad = Mathf.Deg2Rad * angleDeg;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);
    }

    void CreateLine(string name, Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject(name);
        lineObj.transform.parent = transform;

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.useWorldSpace = false;
        lr.material = lineMaterial;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

}


