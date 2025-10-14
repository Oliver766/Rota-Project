using UnityEngine;
using UnityEngine.UI;

public class ShineEffect : MonoBehaviour
{
    public RectTransform shineOverlay;
    public float speed = 200f;
    private Vector2 startPos;
    private Vector2 endPos;

    void Start()
    {
        startPos = new Vector2(-shineOverlay.rect.width, 0);
        endPos = new Vector2(shineOverlay.rect.width, 0);
        shineOverlay.anchoredPosition = startPos;
    }

    void Update()
    {
        shineOverlay.anchoredPosition = Vector2.MoveTowards(shineOverlay.anchoredPosition, endPos, speed * Time.deltaTime);

        if (shineOverlay.anchoredPosition == endPos)
            shineOverlay.anchoredPosition = startPos;
    }
}