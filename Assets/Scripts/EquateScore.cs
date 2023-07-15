using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using B83.LogicExpressionParser;

public class EquateScore : MonoBehaviour
{
    public Text playerCounter;
    public Text playerUncalculatedCounter;
    public static List<string> calcs = new List<string>();
    public static int output;
    public static string calcsString;
    private Parser parser = new Parser();
    private NumberExpression exp;

    void Start()
    {
        output = 0;
        calcsString = "0+0";

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

        if(calcsString != "0+0"){playerUncalculatedCounter.text = calcsString;}
        else{playerUncalculatedCounter.text = "0";}

        if (calcs.Count >= 1)
        {
            if (calcs[0] == "+" || calcs[0] == "-" || calcs[0] == "*" || calcs[0] == "/")
            {
                calcs.Remove(calcs[0]);
            }
        }
        
        if (calcs.Count <= 0)
        {calcsString = "0+0";}
    }
}
