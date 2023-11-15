using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayClickEffect : MonoBehaviour
{ 
    public void PlayCircleEffect()
    {
        var canvas = GameObject.Find("Canvas").transform;
        LeanTween.move(gameObject, new Vector2(transform.position.x + 2000, transform.position.y), 1f).setEasePunch();
    }
}
