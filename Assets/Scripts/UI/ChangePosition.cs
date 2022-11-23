using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePosition : MonoBehaviour
{
    [SerializeField] private GameObject boxConfiner;
    private LineRenderer vRay, hRay;
    private LayerMask vRayMask, hRayMask;
    private Collider2D ballCollider;
    private Vector2 minPosition, maxPosition;
    private Vector3 offset, deltaPos;
    private float distanceToCamera;


    private void Start()
    {
        hRay = boxConfiner.transform.Find("HRay").GetComponent<LineRenderer>();
        vRay = boxConfiner.transform.Find("VRay").GetComponent<LineRenderer>();
        vRayMask = LayerMask.GetMask("Rigidbody", "Ground", "Goat", "Default", "GameOver");
        hRayMask = LayerMask.GetMask("RigidbodyConfiners");
        ballCollider = GetComponent<CircleCollider2D>();

        minPosition = boxConfiner.GetComponent<EdgeCollider2D>().bounds.min + ballCollider.bounds.size/2;
        maxPosition = boxConfiner.GetComponent<EdgeCollider2D>().bounds.max - ballCollider.bounds.size/2;
        deltaPos = transform.position;
        distanceToCamera = Camera.main.WorldToScreenPoint(transform.position).z;
    }

    private void Update()
    {
        /* if (boxConfiner.activeSelf)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (ballCollider.OverlapPoint(Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, distanceToCamera))))
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        startPos = touch.position;
                        Debug.Log($"Start {touch.position}");
                        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, distanceToCamera));
                        Debug.Log($"Offset {offset}");
                        DrawRays();
                        hRay.enabled = true;
                        vRay.enabled = true;
                        break;
                    case TouchPhase.Moved:
                        deltaPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, distanceToCamera)) + offset;
                        Debug.Log($"Delta {deltaPos}");
                        DrawRays();
                        break;
                    case TouchPhase.Ended:
                        hRay.enabled = false;
                        vRay.enabled = false;
                        break;
                }

            }
            else
            {
                hRay.enabled = false;
                vRay.enabled = false;
            }
            transform.position = new Vector3(Mathf.Clamp(deltaPos.x, minPosition.x, maxPosition.x),
                                        Mathf.Clamp(deltaPos.y, minPosition.y, maxPosition.y), 0);
        } */
        
    }

    private void OnMouseDown()
    {
        distanceToCamera = Camera.main.WorldToScreenPoint(transform.position).z;
        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceToCamera));
        DrawRays();
        hRay.enabled = true;
        vRay.enabled = true;
    }

    private void OnMouseDrag()
    {
        deltaPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceToCamera)) + offset;
        transform.position = new Vector3(Mathf.Clamp(deltaPos.x, minPosition.x, maxPosition.x),
                                        Mathf.Clamp(deltaPos.y, minPosition.y, maxPosition.y));
        DrawRays();
    }

    private void OnMouseUp()
    {
        hRay.enabled = false;
        vRay.enabled = false;
    }

    private void DrawRays()
    {
        RaycastHit2D[] hit = new RaycastHit2D[1];
        ballCollider.Raycast(Vector2.down, hit, 50f, vRayMask);
        vRay.SetPosition(0, transform.position);
        vRay.SetPosition(1, hit[0].point + Vector2.down*0.1f);

        ballCollider.Raycast(Vector2.left, hit, 30f, hRayMask);
        hRay.SetPosition(0, hit[0].point);

        ballCollider.Raycast(Vector2.right, hit, 30f, hRayMask);
        hRay.SetPosition(1, hit[0].point);        
    }
}
