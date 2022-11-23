using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudsController : MonoBehaviour
{
    [SerializeField] private float speed = 1;
    [SerializeField] private float leftBound = -10;
    [SerializeField] private float rightBound = 63;

    private void Update()
    {
        foreach (Transform child in transform)
        {
            child.position += (Vector3)Vector2.left * speed * Time.deltaTime;
            if (child.position.x < leftBound)
            {
                child.position = new Vector3(rightBound, child.position.y/*  + Random.Range(-1f, 1f) */, child.position.z);
            }
        }
    }
}
