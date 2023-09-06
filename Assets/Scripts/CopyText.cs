using UnityEngine;
using UnityEngine.UI;

public class CopyText : MonoBehaviour
{
    public Text targetText;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Text>().text = targetText.text;
    }
}
