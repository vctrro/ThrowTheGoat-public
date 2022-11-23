using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSound : MonoBehaviour
{
    private AudioSource hitSound, goatHitSound;
    private bool pause = true;
    private bool hit = false, goatHit = false;
    private float pauseTime = .6f;
    private SceneController sceneController;

    private void Start()
    {
        sceneController = GameObject.Find("SceneController").GetComponent<SceneController>();
        hitSound = GetComponent<AudioSource>();
        goatHitSound = AudioManager.Instance.GoatHit;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (pause)
        {
            pauseTime = .6f;
            return;
        }

        if (other.relativeVelocity.sqrMagnitude < 80) return;
        
        pauseTime = .6f;
        hit = true;

        if (other.gameObject.tag == "Player")
        {
            goatHit = true;
            Debug.Log($"{other.gameObject} with {other.otherCollider.gameObject} by {other.relativeVelocity.sqrMagnitude}");
        }
    }

    private void FixedUpdate()
    {
        if (pause)
        if (pauseTime > 0)
        {
            pauseTime -= Time.fixedDeltaTime;
        }
        else
        {
            pause = false;
        }
    }

    private void Update()
    {
        if (pause) return;
        if (hit)
        {
            hitSound.Play();
            hit = false;
        }

        if (goatHit)
        {
            sceneController.OnHitGoat.Invoke();
            goatHitSound.Play();
            goatHit = false;
            Debug.Log("GoatHit");
        }
        pause = true;
    }
}
