using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnBlocks : MonoBehaviour
{
    public int width, height;
    private int RandBlock1, RandBlock2, RandTrash;
    public GameObject tilePrefab, destroyParticlePrefab, LinePrefab;
    public static GameObject destroyParticlePrefabS;
    public GameObject[] Blocks;
    private BackgroundTile[,] allTiles;
    public static GameObject[,] allBlocks;
    private GameObject currentBlock, currentLine;
    private Vector2 tempPosition;
    private GameObject backgroundTile;
    private static GameObject destroyParticle;
    public static bool canStaticSetUp;
    private Color red, blue;
    
    [HideInInspector]
    public DrawLine lineDrawer;
    
    [HideInInspector]
    public SwipeBlocks swipeBlocks;
    
    
    void Start()
    {
        lineDrawer = GetComponent<DrawLine>();
        swipeBlocks = GetComponent<SwipeBlocks>();
        destroyParticlePrefabS = destroyParticlePrefab;
        canStaticSetUp = false;
        allTiles = new BackgroundTile[width,height];
        allBlocks = new GameObject[width,height];
        SetUp(width, height);
    }

    public void SetUp(int width, int height)
    {
        canStaticSetUp = false;
        SwipeBlocks.canBeSelected = true;
        foreach (Transform child in gameObject.transform)
        {Destroy(child.gameObject);}

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
        SwipeBlocks.canBeSelected = true;
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

    public static void RemoveBlock(int index, bool isDelete)
    {

        if (EquateScore.calcs.Count > 0)
        {
            if (isDelete)
            {
                for (int f = EquateScore.calcs.Count - 1; f >= 0; f--)
                {EquateScore.calcs.Remove(EquateScore.calcs[f]);}

                EquateScore.calcsString = "0+0";
            }

            else
            {
                for (int i = EquateScore.calcs.Count - 1; i >= index - 1; i--)
                {EquateScore.calcs.RemoveAt(i);}
            }
        }

        if (SwipeBlocks.SelectedBlocks.Count > 0)
        {
            for (int i = SwipeBlocks.SelectedBlocks.Count - 1; i >= index - 1; i--)
            { 
                for (int f = SwipeBlocks.SelectedBlocks[i].transform.childCount - 1; f >= 0; f--)
                {Destroy(SwipeBlocks.SelectedBlocks[i].transform.GetChild(f).gameObject);}

                var blockColor = SwipeBlocks.SelectedBlocks[i].GetComponent<SwipeBlocks>().myColor;
                SwipeBlocks.SelectedBlocks[i].GetComponent<SpriteRenderer>().material.SetColor("_Color", blockColor);
                SwipeBlocks.SelectedBlocks[i].GetComponent<SwipeBlocks>().selected = false;
                SwipeBlocks.IDCounter -= 1;
            }

            if (isDelete)
            {
                {
                    SwipeBlocks.canBeSelected = false;
                    for (int j = SwipeBlocks.SelectedBlocks.Count - 1; j >= index - 1; j--)
                    {
                        destroyParticle = Instantiate(destroyParticlePrefabS, SwipeBlocks.SelectedBlocks[j].gameObject.transform.position, Quaternion.identity);
                        destroyParticle.transform.SetParent(SwipeBlocks.SelectedBlocks[j].gameObject.transform);

                        if (SwipeBlocks.SelectedBlocks[j].GetComponent<SwipeBlocks>().myColor != Color.white)
                        {UIFunctions.trashCounter += SwipeBlocks.SelectedBlocks[j].GetComponent<SwipeBlocks>().myTrash;}
        
                        Destroy(SwipeBlocks.SelectedBlocks[j].gameObject, 1);
                        SwipeBlocks.SelectedBlocks[j].GetComponent<Animator>().Play("ShrinkBlock");
                        SwipeBlocks.IDCounter = 0;
                    }
                }
            }

            else
            {
                for (int i = SwipeBlocks.SelectedBlocks.Count - 1; i >= index - 1; i--)
                {SwipeBlocks.SelectedBlocks.RemoveAt(i);}
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allBlocks[i, 3] == null && canStaticSetUp == true && transform.childCount < 40)
                {
                    StartCoroutine(StaticSetUp(i, 3));
                }
            }
        }

        for (int j = 0; j <= SwipeBlocks.SelectedBlocks.Count - 1; j++)
        {
            if (SwipeBlocks.SelectedBlocks[j] == null){SwipeBlocks.SelectedBlocks.Remove(SwipeBlocks.SelectedBlocks[j]);}
        }

        if (SwipeBlocks.SelectedBlocks.Count >= 2 && SwipeBlocks.SelectedBlocks[SwipeBlocks.SelectedBlocks.Count - 1] != null && SwipeBlocks.SelectedBlocks[SwipeBlocks.SelectedBlocks.Count - 1].GetComponent<SwipeBlocks>().myID > 1 && SwipeBlocks.SelectedBlocks[SwipeBlocks.SelectedBlocks.Count - 1].transform.childCount < 3)
        {
            if(!SwipeBlocks.SelectedBlocks[SwipeBlocks.SelectedBlocks.Count - 1].GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("ShrinkBlock"))
            {
                currentLine = Instantiate(LinePrefab, SwipeBlocks.SelectedBlocks[SwipeBlocks.SelectedBlocks.Count - 1].transform.position, Quaternion.identity);
                currentLine.transform.SetParent(SwipeBlocks.SelectedBlocks[SwipeBlocks.SelectedBlocks.Count - 1].transform);
                currentLine.GetComponent<DrawLine>().origin = SwipeBlocks.SelectedBlocks[SwipeBlocks.SelectedBlocks.Count - 1].gameObject.transform.transform;
                currentLine.GetComponent<DrawLine>().destination = SwipeBlocks.SelectedBlocks[SwipeBlocks.SelectedBlocks.Count - 2].gameObject.transform.transform;
            }
        }

    }
}
