using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTrigger : MonoBehaviour
{
    [SerializeField] private ParticleSystem blobs;
    private void OnTriggerEnter2D(Collider2D other)
    {
        GetComponent<AudioSource>().Play();
        Instantiate(blobs, new Vector3(other.transform.position.x, 1f, 0f), blobs.transform.rotation);
    }
}