using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CPUCounter : MonoBehaviour
{
    public GameObject levelText, Explosion;
    private GameObject exxplosion;
    [HideInInspector]
    public int maxRand, minRand, CPUInt, CPUCalcInt, RespondInt, CPULevel;

    public int respondPassInt;

    private Text Counter, currentLevelText;
    public static bool canAct;

    void Start()
    {
        canAct = false;
        Counter = gameObject.GetComponent<Text>();
        currentLevelText = levelText.GetComponent<Text>();
        CPUInt = 0;
        CPUCalcInt = 0;
        CPULevel = 1;
        currentLevelText.text = "Level " + CPULevel.ToString();
        
        minRand = 1;
        maxRand = 4;
        
        if (SceneManager.GetActiveScene().name == "Level3")
        {respondPassInt = 1070;}

        if (SceneManager.GetActiveScene().name == "Level4")
        {respondPassInt = 1170;}
    }

    void Update()
    {
        Counter.text = CPUInt.ToString();
        
        if (canAct)
        {
            RespondInt = Random.Range(1, respondPassInt + 1);

            if (RespondInt >= respondPassInt)
            {
                CPUCalcInt = Random.Range(minRand, maxRand + 1);

                if (CPUInt < CPUGoal.CPGoal && CPUInt != CPUGoal.CPGoal - 1)
                {
                    CPUInt += CPUCalcInt + 1;

                    if (SceneManager.GetActiveScene().name == "Level4")
                    {
                        var TrashInt = Random.Range(1, 20);

                        if (TrashInt >= 19)
                        {
                            var TrashToThrow = Random.Range(1, 3);
                            if (TrashToThrow == 1)
                            {
                                GameObject.Find("ButtonCanvas").GetComponent<UIFunctions>().playerNumber += 5;
                            }
                            else if (TrashToThrow != 1)
                            {
                                GameObject.Find("ButtonCanvas").GetComponent<UIFunctions>().playerNumber -= 5;
                            }
                            exxplosion = Instantiate(Explosion, GameObject.Find("YourNumberCounter").transform.position, Quaternion.identity);
                            Destroy(exxplosion, 2f);
                        }
                    }
                }

                if (CPUInt > CPUGoal.CPGoal)
                {
                    CPUInt -= CPUCalcInt - 1;

                    if (SceneManager.GetActiveScene().name == "Level4")
                    {
                        var TrashInt = Random.Range(1, 20);

                        if (TrashInt >= 19)
                        {
                            var TrashToThrow = Random.Range(1, 3);
                            if (TrashToThrow == 1)
                            {
                                GameObject.Find("ButtonCanvas").GetComponent<UIFunctions>().playerNumber += 5;
                            }
                            else if (TrashToThrow != 1)
                            {
                                GameObject.Find("ButtonCanvas").GetComponent<UIFunctions>().playerNumber -= 5;
                            }
                            exxplosion = Instantiate(Explosion, GameObject.Find("YourNumberCounter").transform.position, Quaternion.identity);
                            Destroy(exxplosion, 2f);
                        }
                    }
                }

                else if (CPUInt == CPUGoal.CPGoal - 1 || CPUInt == CPUGoal.CPGoal + 1)
                {CPUInt = CPUGoal.CPGoal;}
            }
        }
    }

    public void CPULevelUp()
    {
        CPULevel += 1;
        currentLevelText.text = "Level " + CPULevel.ToString();

        if (CPULevel % 2 == 0)
        {respondPassInt -= 10;}

        if (CPULevel % 5 == 0)
        {maxRand += 2;}
    }
}
