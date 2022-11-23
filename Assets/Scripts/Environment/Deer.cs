using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deer : MonoBehaviour
{
    [SerializeField] float min, max, delay;
    private void Start()
    {
        InvokeRepeating("DeerAppears", 3f, delay);
    }

    private void DeerAppears()
    {
        transform.localPosition = new Vector3(Random.Range(min, max), 0, 0);
        GetComponentInChildren<Animator>().Play("Deer", 0);
        // Debug.Log(GetComponentInChildren<Animator>().gameObject);
    }
}
