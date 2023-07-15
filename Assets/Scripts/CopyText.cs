using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CopyText : MonoBehaviour
{
    public GameObject copyObject;
    private Text selfText; 
    private string copyText;
    // Start is called before the first frame update
    void Start()
    {
        selfText = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        copyText = copyObject.GetComponent<Text>().text;
        selfText.text = copyText.ToString();
    }
}
