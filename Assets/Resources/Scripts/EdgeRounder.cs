using UnityEngine;

public class EdgeRounder : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EdgeCollider2D edgeCollider = GetComponent<EdgeCollider2D>();
        Vector2[] points = edgeCollider.points;
        for (int i = 0; i < points.Length; i++)
        {
            Vector2 point = points[i];
            point.x = Mathf.Round(point.x);
            point.y = Mathf.Round(point.y);
            points[i] = point;
        }
        edgeCollider.points = points;
    }

}
