using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using B83.LogicExpressionParser;

public class EquateScore : MonoBehaviour
{
    private Text playerCounter;
    private Text playerUncalculatedCounter;
    public List<string> calcs = new();
    public int output;
    public string calcsString;
    private Parser parser = new();
    private NumberExpression exp;

    void Start()
    {
        //Initialise output and calc string to be 0
        output = 0;
        calcsString = "0+0";
        playerCounter = GameObject.Find("LabelCanvas").transform.Find("CalculatedNumberCounter").GetComponent<Text>();
        playerUncalculatedCounter = GameObject.Find("LabelCanvas").transform.Find("UncalculatedNumberCounter").GetComponent<Text>();
        
        for (int i = 0; i <= calcs.Count - 1; i++){calcs.RemoveAt(i);}
    }

    void Update()
    {
            
        for(int i = 0; i < calcs.Count; i++)
        {
            calcsString = string.Join("", calcs.ToArray());
        }

        if (calcs.Count == 1 || calcs.Count == 3 || calcs.Count == 5 || calcs.Count == 7)
        {
            exp = parser.ParseNumber(calcsString);
            output = Convert.ToInt32(exp.GetNumber());
            playerCounter.text = output.ToString();
        }

        else
        {
            output = 0;
            playerCounter.text = "0";
        }

        if (calcsString != "0+0")
        {
            playerUncalculatedCounter.text = calcsString;
        }
        else
        {
            playerUncalculatedCounter.text = "0";
        }

        if (calcs.Count >= 1)
        {
            if (calcs[0] == "+" || calcs[0] == "-" || calcs[0] == "*" || calcs[0] == "/")
            {
                calcs.Remove(calcs[0]);
            }
        }

        if (calcs.Count <= 0)
        {
            calcsString = "0+0";
        }
    }
}
