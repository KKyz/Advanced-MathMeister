using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    private static Image fadeColor;
    private static CanvasGroup fadeBlock;


    public void Awake()
    {
        GameObject fadeObj = transform.Find("Fade").gameObject;
        fadeObj.SetActive(true);
        fadeColor = fadeObj.GetComponent<Image>();
        fadeBlock = fadeObj.GetComponent<CanvasGroup>();

        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        
        if (buildIndex == 0 || buildIndex == 2 || buildIndex == 3 || buildIndex == 4 || buildIndex == 5 ||
            buildIndex == 10)
        {
            StartCoroutine(FadeOut());  
        }
        else
        {
            StartCoroutine(SemiFadeIn());  
        }
    }

    public static IEnumerator FadeIn()
    {
        LeanTween.alpha(fadeColor.rectTransform, 1f, 1f); 
        fadeBlock.blocksRaycasts = true;
        yield return 0;
    }

    public static IEnumerator FadeOut()
    {
        LeanTween.alpha(fadeColor.rectTransform, 0f, 1f).setEase(LeanTweenType.linear);

        fadeBlock.blocksRaycasts = true;
        yield return new WaitForSeconds(0.75f);
        fadeBlock.blocksRaycasts = false;
        yield return 0;
    }

    public static IEnumerator SemiFadeIn()
    {
        LeanTween.alpha(fadeColor.rectTransform, 0.3f, 1f).setEase(LeanTweenType.linear);
        fadeBlock.blocksRaycasts = true;
        yield return 0;
    }

    public static IEnumerator SemiFadeOut()
    {
        LeanTween.alpha(fadeColor.rectTransform, 0f, 0.5f).setEase(LeanTweenType.linear);

        fadeBlock.blocksRaycasts = false;
        yield return 0;
    }
}
