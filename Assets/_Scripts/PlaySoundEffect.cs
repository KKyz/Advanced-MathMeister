using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSFX : MonoBehaviour
{
    private AudioSource CanvasAudio;

    void PlayCanvasSFX(AudioClip SFX)
    {
        CanvasAudio = gameObject.GetComponent<AudioSource>();
        CanvasAudio.loop = false;
        CanvasAudio.clip = SFX;
        CanvasAudio.Play();
    }
}
