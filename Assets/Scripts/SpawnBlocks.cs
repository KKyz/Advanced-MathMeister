using System.Collections;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnBlocks : MonoBehaviour
{
    public int width, height;
    public GameObject tilePrefab, destroyParticlePrefab, LinePrefab;
    public GameObject[] Blocks;
    public static GameObject[,] allBlocks;
    public bool canBeSelected;

    private int RandBlock1, RandBlock2, RandTrash;
    private EquateScore equateScore;
    private GameObject currentBlock, currentLine;
    private Vector2 tempPosition;
    private static GameObject destroyParticlePrefabS;
    private GameObject backgroundTile;
    private static GameObject destroyParticle;
    private static bool canStaticSetUp;
    private Color red, blue;
    private DrawLine lineDrawer;
    private SwipeBlocks swipeBlocks;
    private GameManager gameManager;
    
    void Start()
    {
        gameManager = GameObject.Find("GameInterface").GetComponent<GameManager>();
        equateScore = GetComponent<EquateScore>();
        lineDrawer = GetComponent<DrawLine>();
        destroyParticlePrefabS = destroyParticlePrefab;
        canStaticSetUp = false;
        canBeSelected = false;
        allBlocks = new GameObject[width,height];
        SetUp(width, height);
    }

    public void SetUp(int width, int height)
    {
        canStaticSetUp = false;
        canBeSelected = true;
        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                tempPosition = new Vector2(i, j);
                backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity);
                backgroundTile.transform.SetParent(gameObject.transform);
                backgroundTile.name = "(" + i + "," + j + ")B";
                
                RandBlock1 = Random.Range(0, 100);
                if (RandBlock1 < 45){RandBlock2 = Random.Range(0, 5);}
                else if (RandBlock1 >= 45 && RandBlock1 < 90){RandBlock2 = Random.Range(5, 9);}
                else if (RandBlock1 >= 90){RandBlock2 = Random.Range(9, 13);}

                

                currentBlock = Instantiate(Blocks[RandBlock2], tempPosition, Quaternion.identity);
                currentBlock.GetComponent<Animator>().Play("GrowBlock");
                currentBlock.transform.SetParent(gameObject.transform);
                currentBlock.name = "(" + i + "," + j + ")";
                swipeBlocks = currentBlock.GetComponent<SwipeBlocks>();

                if (SceneManager.GetActiveScene().name == "Level4")
                {
                    red = new Color(1f, 0.49f, 0.49f, 1f);
                    blue = new Color(0.38f, 0.61f, 1f, 1f);

                    RandTrash = Random.Range(0, 100);

                    if (RandTrash <= 5)
                    {
                        currentBlock.GetComponent<SwipeBlocks>().myColor = red;
                        currentBlock.GetComponent<SwipeBlocks>().myTrash = -5;
                    }

                    else if (RandTrash > 5 && RandTrash <= 10)
                    {
                        currentBlock.GetComponent<SwipeBlocks>().myColor = blue;
                        currentBlock.GetComponent<SwipeBlocks>().myTrash = 5;
                    }

                    else
                    {
                        currentBlock.GetComponent<SwipeBlocks>().myColor = Color.white;
                        currentBlock.GetComponent<SwipeBlocks>().myTrash = 0;
                    }
                }

                allBlocks[i, j] = currentBlock;
            }
        }

        canStaticSetUp = true;
    }

    IEnumerator StaticSetUp(int width, int height)
    {
        canStaticSetUp = false;
        canBeSelected = true;
        tempPosition = new Vector2(width, height);

        RandBlock1 = Random.Range(0, 100);

        if (RandBlock1 < 45)
        {RandBlock2 = Random.Range(0, 5);}

        else if (RandBlock1 >= 45 && RandBlock1 < 75)
        {RandBlock2 = Random.Range(5, 9);}

        else if (RandBlock1 >= 75)
        {RandBlock2 = Random.Range(9, 13);}

        currentBlock = Instantiate(Blocks[RandBlock2], tempPosition, Quaternion.identity);
        allBlocks[width, height] = currentBlock;
        currentBlock.name = "(" + width + "," + height + ")";
        currentBlock.transform.SetParent(gameObject.transform);
        currentBlock.GetComponent<Animator>().Play("GrowBlock");
        if (SceneManager.GetActiveScene().name == "Level4")
        {
            red = new Color(1f, 0.49f, 0.49f, 1f);
            blue = new Color(0.38f, 0.61f, 1f, 1f);
            RandTrash = Random.Range(0, 100);

            if (RandTrash <= 5)
            {
                currentBlock.GetComponent<SwipeBlocks>().myColor = red;
                currentBlock.GetComponent<SwipeBlocks>().myTrash = -5;
            }

            else if (RandTrash > 5 && RandTrash <= 10)
            {
                currentBlock.GetComponent<SwipeBlocks>().myColor = blue;
                currentBlock.GetComponent<SwipeBlocks>().myTrash = 5;
            }

            else
            {
                currentBlock.GetComponent<SwipeBlocks>().myColor = Color.white;
                currentBlock.GetComponent<SwipeBlocks>().myTrash = 0;
            }
        }
        currentBlock.GetComponent<SwipeBlocks>().lockBlock = true;
        yield return new WaitForSeconds(0.23f);
        currentBlock.GetComponent<SwipeBlocks>().lockBlock = false;
        canStaticSetUp = true;
    }

    public void RemoveBlock(int index, bool isDelete)
    {

        if (equateScore.calcs.Count > 0)
        {
            if (isDelete)
            {
                for (int f = equateScore.calcs.Count - 1; f >= 0; f--)
                {
                    equateScore.calcs.Remove(equateScore.calcs[f]);
                }

                equateScore.calcsString = "0+0";
            }

            else
            {
                for (int i = equateScore.calcs.Count - 1; i >= index - 1; i--)
                {
                    equateScore.calcs.RemoveAt(i);
                }
            }
        }

        if (swipeBlocks.SelectedBlocks.Count > 0)
        {
            for (int i = swipeBlocks.SelectedBlocks.Count - 1; i >= index - 1; i--)
            { 
                for (int f = swipeBlocks.SelectedBlocks[i].transform.childCount - 1; f >= 0; f--)
                {Destroy(swipeBlocks.SelectedBlocks[i].transform.GetChild(f).gameObject);}

                var blockColor = swipeBlocks.SelectedBlocks[i].GetComponent<SwipeBlocks>().myColor;
                swipeBlocks.SelectedBlocks[i].GetComponent<SpriteRenderer>().material.SetColor("_Color", blockColor);
                swipeBlocks.SelectedBlocks[i].GetComponent<SwipeBlocks>().selected = false;
                swipeBlocks.IDCounter -= 1;
            }

            if (isDelete)
            {
                {
                    canBeSelected = false;
                    for (int j = swipeBlocks.SelectedBlocks.Count - 1; j >= index - 1; j--)
                    {
                        destroyParticle = Instantiate(destroyParticlePrefabS, swipeBlocks.SelectedBlocks[j].gameObject.transform.position, Quaternion.identity);
                        destroyParticle.transform.SetParent(swipeBlocks.SelectedBlocks[j].gameObject.transform);

                        if (swipeBlocks.SelectedBlocks[j].GetComponent<SwipeBlocks>().myColor != Color.white)
                        {
                            gameManager.trashCounter += swipeBlocks.SelectedBlocks[j].GetComponent<SwipeBlocks>().myTrash;
                        }
        
                        Destroy(swipeBlocks.SelectedBlocks[j].gameObject, 1);
                        swipeBlocks.SelectedBlocks[j].GetComponent<Animator>().Play("ShrinkBlock");
                        swipeBlocks.IDCounter = 0;
                    }
                }
            }

            else
            {
                for (int i = swipeBlocks.SelectedBlocks.Count - 1; i >= index - 1; i--)
                {swipeBlocks.SelectedBlocks.RemoveAt(i);}
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allBlocks[i, 3] == null && canStaticSetUp && transform.childCount < 40)
                {
                    StartCoroutine(StaticSetUp(i, 3));
                }
            }
        }

        for (int j = 0; j <= swipeBlocks.SelectedBlocks.Count - 1; j++)
        {
            if (swipeBlocks.SelectedBlocks[j] == null){swipeBlocks.SelectedBlocks.Remove(swipeBlocks.SelectedBlocks[j]);}
        }

        if (swipeBlocks.SelectedBlocks.Count >= 2 && swipeBlocks.SelectedBlocks[swipeBlocks.SelectedBlocks.Count - 1] != null && swipeBlocks.SelectedBlocks[swipeBlocks.SelectedBlocks.Count - 1].GetComponent<SwipeBlocks>().myID > 1 && swipeBlocks.SelectedBlocks[swipeBlocks.SelectedBlocks.Count - 1].transform.childCount < 3)
        {
            if(!swipeBlocks.SelectedBlocks[swipeBlocks.SelectedBlocks.Count - 1].GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("ShrinkBlock"))
            {
                currentLine = Instantiate(LinePrefab, swipeBlocks.SelectedBlocks[swipeBlocks.SelectedBlocks.Count - 1].transform.position, Quaternion.identity);
                currentLine.transform.SetParent(swipeBlocks.SelectedBlocks[swipeBlocks.SelectedBlocks.Count - 1].transform);
                DrawLine currentLinePoints = currentLine.GetComponent<DrawLine>();
                currentLinePoints.origin = swipeBlocks.SelectedBlocks[swipeBlocks.SelectedBlocks.Count - 1].gameObject.transform.transform;
                currentLinePoints.destination = swipeBlocks.SelectedBlocks[swipeBlocks.SelectedBlocks.Count - 2].gameObject.transform.transform;
            }
        }

    }
}
