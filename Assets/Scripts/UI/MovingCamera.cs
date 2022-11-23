using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
// using Cinemachine;

public class MovingCamera : MonoBehaviour
{
    private Vector3 force, mousePoint1, mousePoint2;
    // private CinemachineBrain vCam;
    private Rigidbody2D rb2D;

    private void Start()
    {
        rb2D = Camera.main.GetComponent<Rigidbody2D>();
        // vCam = Camera.main.GetComponent<CinemachineBrain>();
    }

    private void OnMouseDown()
    {
        mousePoint1 = Input.mousePosition;
    }

    private void OnMouseDrag()
    {
        // if (EventSystem.current.IsPointerOverGameObject()) return;
        // vCam.enabled = false;
        mousePoint2 = Input.mousePosition;
        force = mousePoint1 - mousePoint2;
        mousePoint1 = mousePoint2;
    }

    private void FixedUpdate()
    {
        rb2D.velocity = force;
    }
}
