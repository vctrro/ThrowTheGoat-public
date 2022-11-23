using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLaunch : MonoBehaviour
{
    [SerializeField] private int startOrder = 1;
    private SceneController sceneController;

    private void Start()
    {
        sceneController = GameObject.Find("SceneController").GetComponent<SceneController>();
        sceneController.OnThrowGoat.AddListener(Launch);
    }

    private void Launch(int order)
    {
        if (order == startOrder)
        {
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
    }
}
