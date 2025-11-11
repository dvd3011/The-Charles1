using UnityEngine;
using UnityEngine.UI;

public class BarMovement : MonoBehaviour
{
    [Header("UI References (assigned in inspector)")]
    public Image pointer;              // The pointer image moving left-right
    public float pointerSpeed;  // Increased speed for a more dynamic feel

    private bool movingRight = true;   // Direction of the pointer movement
    private float barWidth;             // Width of the bar

    // Expanded movement boundaries for pointer to go beyond bar edges
    private float leftLimit;
    private float rightLimit;

    public void Initialize(float barWidth)
    {
        pointerSpeed = 200;
        this.barWidth = barWidth;
        leftLimit = -barWidth * 2.9f;    // 10% beyond left edge
        rightLimit = barWidth * 2.9f;    // 10% beyond right edge
        ResetPointer();
    }
    public void SpeedSapo()
    {
        pointerSpeed = 200;
    }
    public void SpeedBeija()
    {
        pointerSpeed = 400;
    }

    public void MovePointer()
    {
        float delta = pointerSpeed * Time.deltaTime * (movingRight ? 1 : -1);
        Vector2 pos = pointer.rectTransform.anchoredPosition;
        pos.x += delta;

        if (pos.x > rightLimit)
        {
            pos.x = rightLimit;
            movingRight = false;
        }
        else if (pos.x < leftLimit)
        {
            pos.x = leftLimit;
            movingRight = true;
        }

        pointer.rectTransform.anchoredPosition = pos;
    }

    public void ResetPointer()
    {
        Vector2 pos = pointer.rectTransform.anchoredPosition;
        pos.x = 0;
        pointer.rectTransform.anchoredPosition = pos;
        movingRight = true;
    }
}

