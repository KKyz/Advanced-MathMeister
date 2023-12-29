using System;
using UnityEngine;
using UnityEngine.UI;

public class CopyText : MonoBehaviour
{
    public Text targetText;

    private Text myText;
    // Start is called before the first frame update
    void Start()
    {
        myText = GetComponent<Text>();
    }

    private void Update()
    {
        myText.text = targetText.text;
        gameObject.SetActive(targetText.gameObject.activeInHierarchy);
    }
}
