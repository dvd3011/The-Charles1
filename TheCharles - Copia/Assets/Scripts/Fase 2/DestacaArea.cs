using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(LineRenderer))]
public class DestacaArea : MonoBehaviour
{
    void Start()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        LineRenderer line = GetComponent<LineRenderer>();

        line.useWorldSpace = false;
        line.loop = true;
        line.positionCount = 4;

        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.yellow;
        line.endColor = Color.yellow;

        // calcula vértices do box
        Vector2 s = box.size * 0.5f;
        Vector2 o = box.offset;
        Vector3[] points =
        {
            new Vector3(o.x - s.x, o.y - s.y, 0),
            new Vector3(o.x - s.x, o.y + s.y, 0),
            new Vector3(o.x + s.x, o.y + s.y, 0),
            new Vector3(o.x + s.x, o.y - s.y, 0)
        };
        line.SetPositions(points);
    }
}