using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpScreen : MonoBehaviour
{
    public Sprite[] images;
    private Image currentTutorial;
    private int index;
    private bool clickable;
    private AudioSource audio;
    public AudioClip click;
    private Button next, prev;
    
    void Start()
    {
        index = 0;
        audio = GetComponent<AudioSource>();
        currentTutorial = transform.Find("Image").GetComponent<Image>();
        next = transform.Find("Next").GetComponent<Button>();
        prev = transform.Find("Back").GetComponent<Button>();
        StartCoroutine(transitionTutorial());
        clickable = true;
    }

    public void NextImage()
    {
        if (index < images.Length && clickable)
        {
            index++;
            
            //audio.PlayOneShot(click);
            StartCoroutine(transitionTutorial());
            clickable = false;
        }
    }

    public void PrevImage()
    {
        if (index > 0 && clickable)
        {
            index--;
            //audio.PlayOneShot(click);
            StartCoroutine(transitionTutorial());
            clickable = false;
        }
    }

    public void Return()
    {
        //audio.PlayOneShot(click);
    }

    private IEnumerator transitionTutorial()
    {
        Debug.Log("Transitioning Scenes");
        LeanTween.alpha(currentTutorial.rectTransform, 0f, 0.5f);
        yield return new WaitForSeconds(0.6f);
        currentTutorial.sprite = images[index];
        LeanTween.alpha(currentTutorial.rectTransform, 1, 0.5f);
        
        var nextCond = index < images.Length - 1 ? next.interactable = true : next.interactable = false;
        var prevCond = index > 0 ? prev.interactable = true : prev.interactable = false;
        clickable = true;
    }
}
