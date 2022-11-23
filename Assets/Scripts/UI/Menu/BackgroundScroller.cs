using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 0.04f;
    private Renderer background;

    private void Start()
    {
        background = GetComponent<Renderer>();
    }

    private void Update()
    {
        Vector2 offset = new Vector2(Time.time * scrollSpeed, 0f);

        background.material.mainTextureOffset = offset;
    }
}
