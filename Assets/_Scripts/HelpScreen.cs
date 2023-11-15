using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpScreen : MonoBehaviour
{
    public Image[] images;
    private Image currentTutorial;
    private int index;
    private AudioSource audio;
    public AudioClip click;
    
    void Start()
    {
        index = 0;
        audio = GetComponent<AudioSource>();
        currentTutorial = transform.Find("Image").GetComponent<Image>();
        currentTutorial = images[index];
    }

    public void NextImage()
    {
        if (index < images.Length)
        {
            index++;
        }
        
        audio.PlayOneShot(click);
        StartCoroutine(transitionTutorial());
    }

    public void PrevImage()
    {
        if (index > 0)
        {
            index--;
        }
        
        audio.PlayOneShot(click);
        StartCoroutine(transitionTutorial());
    }

    public void Return()
    {
        audio.PlayOneShot(click);
    }

    private IEnumerator transitionTutorial()
    {
        LeanTween.alpha(currentTutorial.gameObject, 0f, 0.5f);
        yield return new WaitForSeconds(0.6f);
        currentTutorial = images[index];
        LeanTween.alpha(currentTutorial.gameObject, 1, 0.5f);
    }
}
