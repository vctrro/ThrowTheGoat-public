using System.Collections;
using UnityEngine;

public class Win : MonoBehaviour
{
    private SceneController sceneController;
    private bool win = false;

    private void Start()
    {
        sceneController = GameObject.Find("SceneController").GetComponent<SceneController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (win) return;
        win = true;
                
        sceneController.OnWin.Invoke();
    }
}
