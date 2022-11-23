using UnityEngine;

public class ConfinerCreator : MonoBehaviour
{
    private void Awake() 
    {
        EdgeCollider2D edge = gameObject.AddComponent<EdgeCollider2D>();
        edge.points = GetComponent<PolygonCollider2D>().points;
        Vector2[] points = new Vector2[edge.pointCount+1];
        System.Array.Copy(edge.points, points, edge.pointCount);
        points[edge.pointCount] = points[0];
        edge.points = points;
    }
}
