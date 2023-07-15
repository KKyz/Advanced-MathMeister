using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIFunctions : MonoBehaviour
{
    private AudioSource uiAudio;
    public AudioClip equateSound, shuffleSound;
    public GameObject Spawner, SparkleParticle, ExplosionParticle;
    public Text PlayerCounter;
    public Button shuffleButton, EquateButton, TrashButton;
    public Text TrashCountText;
    public static bool equationResetFlag;
    private bool canShuffle;
    private ColorBlock colorBlock, equateBlock;
    private GameObject sparkleParticle, explosion;
    [HideInInspector] public int playerNumber;
    public static int equationCounter, trashCounter;

    void Start()
    {
        uiAudio = gameObject.GetComponent<AudioSource>();
        equationCounter = 0;
        trashCounter = 0;
        colorBlock = shuffleButton.GetComponent<Button>().colors;
        equateBlock = EquateButton.GetComponent<Button>().colors;
        equationResetFlag = false;
        canShuffle = true;
    }

    public void ShuffleBlocks(float ShuffleTime)
    {
        if (canShuffle == true)
        {
            equationResetFlag = true;
            SpawnBlocks.RemoveBlock(1, false);

            Spawner.GetComponent<SpawnBlocks>().SetUp(5, 4);
            StartCoroutine(RefreshShuffle(ShuffleTime));

            uiAudio.PlayOneShot(shuffleSound, 0.7f);
        }
    }

    IEnumerator RefreshShuffle(float Secs)
    {
        canShuffle = false;
        colorBlock.selectedColor = new Color(0.68f, 0.68f, 0.68f, 1);
        colorBlock.pressedColor = new Color(0.68f, 0.68f, 0.68f, 1);
        colorBlock.normalColor = new Color(0.68f, 0.68f, 0.68f, 1);
        shuffleButton.GetComponent<Button>().colors = colorBlock;
        shuffleButton.GetComponent<Animator>().Play("ShuffleSpinOut");
        yield return new WaitForSeconds(Secs);
        colorBlock.selectedColor = new Color(1, 0, 0, 1);
        colorBlock.pressedColor = new Color(1, 0, 0, 1);
        colorBlock.normalColor = new Color(1, 0, 0, 1);
        shuffleButton.GetComponent<Button>().colors = colorBlock;
        shuffleButton.GetComponent<Animator>().Play("ShuffleSpinIn");
        canShuffle = true;
    }

    public void EquateCurrent()
    {
        if (EquateScore.output != 0 && EquateScore.calcs.Count != 1)
        {
            if (SwitchSubtractionAddition.AdditionMode){playerNumber += EquateScore.output;}
            if (!SwitchSubtractionAddition.AdditionMode){playerNumber -= EquateScore.output;}

            SpawnBlocks.RemoveBlock(1, true);

            equationCounter += 1;
            equationResetFlag = true;

            uiAudio.PlayOneShot(equateSound, 0.7f);
            SwitchSubtractionAddition.haveGasped = false;
            sparkleParticle = Instantiate(SparkleParticle, PlayerCounter.gameObject.transform.position, Quaternion.identity);
            Destroy(sparkleParticle, 2f);
        }
    }

    public void ThrowTrash()
    {
        if (trashCounter != 0)
        {
            GameObject.Find("CPUCounter").GetComponent<CPUCounter>().CPUInt += trashCounter;

            explosion = Instantiate(ExplosionParticle, GameObject.Find("CPUCounter").transform.position, Quaternion.identity);
            TrashButton.GetComponent<Animator>().Play("TrashSpinIn");
            Destroy(explosion, 2f);
            trashCounter = 0;
        }
    }

    public void Update()
    {
        PlayerCounter.text = playerNumber.ToString();

        if (SceneManager.GetActiveScene().name == "Level4")
        {
            if (trashCounter != 0)
            {
                TrashButton.GetComponent<Animator>().Play("TrashSpinIn");
                TrashCountText.text = trashCounter.ToString();
            }

            if (TrashButton.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("TrashSpinIn") && trashCounter == 0)
            {
                TrashButton.GetComponent<Animator>().Play("TrashSpinOut");
                TrashCountText.text = "0";
            }
        }

        if (EquateScore.output != 0)
        {
            if (EquateScore.calcs.Count == 3 || EquateScore.calcs.Count == 5 || EquateScore.calcs.Count == 7)
            {
                equateBlock.selectedColor = new Color(0, 1, 0, 1);
                equateBlock.pressedColor = new Color(0, 0.32f, 0.078f, 1);
                equateBlock.normalColor = new Color(0, 1, 0, 1); 
                EquateButton.enabled = true;
                EquateButton.GetComponent<Animator>().Play("EquateSpinIn");
            }
            
        }
        else
        {
            equateBlock.selectedColor = new Color(0.68f, 0.68f, 0.68f, 1);
            equateBlock.pressedColor = new Color(0.68f, 0.68f, 0.68f, 1);
            equateBlock.normalColor = new Color(0.68f, 0.68f, 0.68f, 1);
            EquateButton.enabled = false;

            if (EquateButton.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("EquateSpinIn"))
            {EquateButton.GetComponent<Animator>().Play("EquateSpinOut");}
        }

        if (SelectTargetNumber.scoreAddFlag)
        {
            equationCounter = 0;
        }
    }
}
