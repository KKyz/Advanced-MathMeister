using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBG : MonoBehaviour
{
    private GameObject Planet, Sky, Fader;
    public Material[] Planets, Skies;
    private MeshRenderer currentPlanet, currentSky;
    private Animator fadeAnim;
    private int BGInt, objCounter;
    bool isCoroutineStarted = false, nextBG = false;
    private int Threshold;

    void Start()
    {
        isCoroutineStarted = false;
        nextBG = false;
        Sky = transform.GetChild(0).gameObject;
        Planet = transform.GetChild(1).gameObject;
        Fader = transform.GetChild(2).gameObject;

        currentSky = Sky.GetComponent<MeshRenderer>();
        currentPlanet = Planet.GetComponent<MeshRenderer>();
        fadeAnim = Fader.GetComponent<Animator>();

        BGInt = 0;
        Threshold = 1000;
        currentSky.material = Skies[0];
        currentPlanet.material = Planets[0];

        objCounter = 0;
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeInHierarchy)
            {objCounter += 1;}
        }
    }

    void Update()
    {
        if (objCounter == 5)
        {
            if (ShowScore.playerScore >= Threshold && ShowScore.playerScore > 0 && BGInt <= BGInt + 1)
            {
                if (!nextBG)
                {
                    BGInt += 1;
                    nextBG = true;
                }
                if (!isCoroutineStarted && BGInt <= 3)
                {
                    StartCoroutine(UpdateBG(BGInt));
                }
            }

            else
            {
                isCoroutineStarted = false;
                nextBG = false;
            }
        }
    }

    IEnumerator UpdateBG(int NextTex)
    {
        fadeAnim.Play("SquareFadeIn");

        yield return new WaitForSeconds(0.5f);

        currentSky.material = Skies[NextTex - 1];
        currentPlanet.material = Planets[NextTex - 1];

        yield return new WaitForSeconds(0.5f);

        fadeAnim.Play("SquareFadeOut");
        Threshold += 1000;
        isCoroutineStarted = true;
    }
}
