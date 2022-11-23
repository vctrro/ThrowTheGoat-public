using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxConfiner : MonoBehaviour
{
    private void Awake()
    {
        var box = GetComponent<BoxCollider2D>();
        Vector3[] vertexes = {box.bounds.min, new Vector3(box.bounds.min.x, box.bounds.max.y), box.bounds.max, new Vector3(box.bounds.max.x, box.bounds.min.y)};
        GetComponent<LineRenderer>().SetPositions(vertexes);
        var edge = gameObject.AddComponent<EdgeCollider2D>();
        Vector2[] points = {box.bounds.min, new Vector2(box.bounds.min.x, box.bounds.max.y), box.bounds.max, new Vector2(box.bounds.max.x, box.bounds.min.y)};
        edge.points = points;
        box.enabled = false;
    }
}
