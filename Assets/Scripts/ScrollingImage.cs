using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingImage : MonoBehaviour
{
    private Renderer quadRenderer;

    public float scrollSpeed = 0.3f;

    void Start()
    {
        quadRenderer = gameObject.GetComponent<Renderer>();
    }

    void Update()
    {
        Vector2 textureOffset = new Vector2(Time.time*scrollSpeed,0);
        quadRenderer.material.mainTextureOffset = textureOffset;
    }
}
