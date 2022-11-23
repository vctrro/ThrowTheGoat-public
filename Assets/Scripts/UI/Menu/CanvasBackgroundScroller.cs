using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasBackgroundScroller : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 0.04f;
    private RawImage background;
    Vector2 offset = Vector2.zero;

    private void Start()
    {
        background = GetComponent<RawImage>();
    }

    private void Update()
    {
        offset += new Vector2(Time.fixedDeltaTime * scrollSpeed, 0f);

        background.materialForRendering.mainTextureOffset = offset;
        // background.materialForRendering.SetTextureOffset("_MainTex", offset);
    }
}
