using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CPUGoal : MonoBehaviour
{
    public static int CPGoal;

    public int minGoal, maxGoal;
    public GameObject SparksPrefab;
    private GameObject sparks;
    private Text goalText;
    [SerializeField]private bool awakenedUpdate = false;

    void Start()
    {
        goalText = gameObject.GetComponent<Text>();

        if (awakenedUpdate)
        {
            UpdateCPUGoal(minGoal, maxGoal);
            awakenedUpdate = false;
            Destroy(sparks);
        }
    }

    // Update is called once per frame
    public void UpdateCPUGoal(int minGoal, int maxGoal)
    {
        CPGoal = Random.Range(minGoal, maxGoal);
        goalText.text = CPGoal.ToString();

        sparks = Instantiate(SparksPrefab, transform.position, Quaternion.identity);
        Destroy(sparks, 2f);
    }
}
