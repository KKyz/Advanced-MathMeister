using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelect : MonoBehaviour
{

    public GameObject whiteFade;
    private int SceneInt;

    public void LoadScene(int SceneNumber)
    {
        SceneInt = SceneNumber;
        StartCoroutine(TransitionToScene());

    }

    IEnumerator TransitionToScene()
    {
        whiteFade.GetComponent<Animator>().Play("FadeOut");
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene(SceneInt);
    }
}
