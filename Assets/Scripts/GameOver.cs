using UnityEngine;

public class GameOver : MonoBehaviour
{
    private SceneController sceneController;
    private bool gameOver = false;

    private void Start()
    {
        sceneController = GameObject.Find("SceneController").GetComponent<SceneController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameOver) return;
        Vector2 target = other.transform.position;
        sceneController.OnGoatIsDead.Invoke(target);
        gameOver = true;
    }
}
