using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayClickEffect : MonoBehaviour
{
    public GameObject ClickEffect, canvas;
    private GameObject clickEffect;
    private bool hasPlayed = false;

    public void PlayCircleEffect()
    {
        if (hasPlayed == false)
        {
            clickEffect = Instantiate(ClickEffect, transform.position, Quaternion.identity);
            clickEffect.transform.SetParent(canvas.transform);
            hasPlayed = true;
        }
    }

    public void ClickedAnimation(string AnimationToPlay)
    {
        gameObject.GetComponent<Animator>().Play(AnimationToPlay);
    }
}
