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
    }

    public void ClickedAnimation(string AnimationToPlay)
    {
        gameObject.GetComponent<Animator>().Play(AnimationToPlay);
    }
}
