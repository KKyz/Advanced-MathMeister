using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayClickEffect : MonoBehaviour
{
    public GameObject ClickEffect;

    public void PlayCircleEffect()
    {
        var canvas = GameObject.Find("Canvas").transform;
        var thisClickEffect = Instantiate(ClickEffect, transform.position, Quaternion.identity);
        thisClickEffect.transform.SetParent(canvas.transform);
        thisClickEffect.transform.SetSiblingIndex(canvas.childCount - 2);

        LeanTween.move(gameObject, new Vector2(transform.position.x + 2000, transform.position.y), 1f).setEasePunch();
    }
}
