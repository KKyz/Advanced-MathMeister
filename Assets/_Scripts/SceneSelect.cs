using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelect : MonoBehaviour
{
    
    private int SceneInt;

    public void LoadScene(int SceneNumber)
    {
        SceneInt = SceneNumber;
        StartCoroutine(Fade.FadeIn());
        StartCoroutine(TransitionToScene());

    }

    IEnumerator TransitionToScene()
    {
        yield return new WaitForSeconds(1.1f);
        SceneManager.LoadScene(SceneInt);
    }
}
