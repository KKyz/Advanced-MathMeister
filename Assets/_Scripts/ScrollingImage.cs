using UnityEngine;

public class ScrollingImage : MonoBehaviour
{
    //Literally just scrolls the BG on the title screen.
    //Nothing more to see here.
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
