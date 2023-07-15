using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class CanvasSoundEffects : MonoBehaviour
{
    public AudioMixerGroup BGMMixer, SFXMixer;
    private AudioSource CanvasAudio;

    void Start()
    {
        CanvasAudio = gameObject.GetComponent<AudioSource>();
        CanvasAudio.outputAudioMixerGroup = BGMMixer;
    }
    public void PlayCanvasSFX(AudioClip SFX)
    {
        CanvasAudio.outputAudioMixerGroup = SFXMixer;
        CanvasAudio.loop = false;
        CanvasAudio.clip = SFX;
        CanvasAudio.Play();
    }
}
