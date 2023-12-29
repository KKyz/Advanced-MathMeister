using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelect : MonoBehaviour
{
    
    private int SceneInt;

    public void Start()
    {
        if (SceneManager.GetActiveScene().name == "ExtraScreen")
        {
            StartCoroutine(SlideInGameButtons());
        }
    }

    private IEnumerator SlideInGameButtons()
    {
        var gameButtons = transform.Find("Games");
        var count = 0;
        List<float> pos = new();

        foreach (Transform button in gameButtons)
        {
            pos.Add(button.position.x);
            button.position = new Vector3(-600, button.position.y);
        }

        foreach (Transform button in gameButtons)
        {
            LeanTween.moveX(button.gameObject, pos[count], 1f).setEaseOutBounce();
            count++;
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    private IEnumerator SlideOutGameButtons()
    {
        var gameButtons = transform.Find("Games");

        foreach (Transform button in gameButtons)
        {
            var buttonTarget = -600;
            LeanTween.moveX(button.gameObject, buttonTarget, 1f).setEaseInBounce();
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void LoadScene(int SceneNumber)
    {
        SceneInt = SceneNumber;
        StartCoroutine(Fade.FadeIn());
        StartCoroutine(TransitionToScene());
        
        if (SceneManager.GetActiveScene().name == "ExtraScreen")
        {
            StartCoroutine(SlideOutGameButtons());
        }
    }

    public void HideResults()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        } 
    }

    IEnumerator TransitionToScene()
    {
        yield return new WaitForSeconds(1.1f);
        SceneManager.LoadScene(SceneInt);
    }
}
