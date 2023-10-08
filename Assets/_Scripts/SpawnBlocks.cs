using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class SpawnBlocks : MonoBehaviour
{
    public int width, height;
    public GameObject tilePrefab, linePrefab, destroyParticle;
    public GameObject[] Blocks;
    public GameObject[,] allBlocks;
    public List<GameObject> selectedBlocks = new();
    public bool canBeSelected;
    
    private Vector2 tempPosition;
    private bool canStaticSetUp;
    //private SwipeBlocks swipeBlocks;
    private GameManager gameManager;
    
    void Start()
    {
        gameManager = GameObject.Find("GameInterface").GetComponent<GameManager>();
        canStaticSetUp = false;
        canBeSelected = false;
        allBlocks = new GameObject[width,height];
        SetUp(width, height);
    }

    public void SetUp(int width, int height)
    {
        //Initial setup w/o any special rules, used in start and when shuffling
        canStaticSetUp = false;
        canBeSelected = true;
        
        //Clears out all contents in game object
        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }

        //Creating grid of blocks
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                
                tempPosition = new Vector2(i, j);
                
                //First, it creates all of the background tiles
                var backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity);
                backgroundTile.transform.SetParent(gameObject.transform);
                backgroundTile.name = "(" + i + "," + j + ")B";
                
                /*Secondly, the block to be placed on the background tile position is decided. This is done through
                 two random variables. randBlock1 is an arbitrary probability counter 
                 that settles on a tier of blocks (e.g. most powerful blocks have a 15% chance
                 of appearing). randBlock2 decides which one from that tier block (e.g. choosing *, 8, 9, etc.).*/
                var randBlock1 = Random.Range(0, 100);
                var randBlock2 = 0;
                
                if (randBlock1 < 45)
                {
                    randBlock2 = Random.Range(0, 5);
                }
                else if (randBlock1 >= 45 && randBlock1 < 90)
                {
                    randBlock2 = Random.Range(5, 9);
                }
                else if (randBlock1 >= 90)
                {
                    randBlock2 = Random.Range(9, 13);
                }
                
                //Creates and places these blocks into the grid
                var currentBlock = Instantiate(Blocks[randBlock2], tempPosition, Quaternion.identity);
                //currentBlock.GetComponent<Animator>().Play("GrowBlock");
                currentBlock.transform.localScale = new Vector3(0, 0, 0);
                LeanTween.scale(currentBlock.gameObject, new Vector3(1.25f, 1.25f, 1.25f), 1f).setEaseOutBounce();
                currentBlock.transform.SetParent(gameObject.transform);
                currentBlock.name = "(" + i + "," + j + ")";
                //swipeBlocks = currentBlock.GetComponent<SwipeBlocks>();

                //The block is tracked by placing it into an array called allBlocks
                allBlocks[i, j] = currentBlock;
            }
        }

        canStaticSetUp = true;
    }

    public IEnumerator StaticSetUp(int width, int height)
    {
        canStaticSetUp = false;
        canBeSelected = true;
        tempPosition = new Vector2(width, height);

        /*Very similar to doing SetUp(). The difference is that staticSetUp prevents blocks from moving when new ones are
         being made*/
         var randBlock1 = Random.Range(0, 100);
         var randBlock2 = 0;

        if (randBlock1 < 45)
        {
            randBlock2 = Random.Range(0, 5);
        }

        else if (randBlock1 < 75 && randBlock1 >= 45)
        {
            randBlock2 = Random.Range(5, 9);
        }

        else if (randBlock1 >= 75)
        {
            randBlock2 = Random.Range(9, 13);
        }

        var currentBlock = Instantiate(Blocks[randBlock2], tempPosition, Quaternion.identity);
        allBlocks[width, height] = currentBlock;
        currentBlock.name = "(" + width + "," + height + ")";
        currentBlock.transform.SetParent(gameObject.transform);
        //currentBlock.GetComponent<Animator>().Play("GrowBlock");
        currentBlock.transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(currentBlock.gameObject, new Vector3(1.25f, 1.25f, 1.25f), 1f).setEaseOutBounce();

        var swipeBlock = currentBlock.GetComponent<SwipeBlocks>();
        swipeBlock.lockBlock = true;
        yield return new WaitForSeconds(0.23f);
        swipeBlock.lockBlock = false;
        canStaticSetUp = true;
    }

    public void RemoveBlock(int index, bool isDelete)
    {

        if (gameManager.calcs.Count > 0)
        {
            if (isDelete)
            {
                for (int f = gameManager.calcs.Count - 1; f >= 0; f--)
                {
                    gameManager.calcs.Remove(gameManager.calcs[f]);
                }

                gameManager.calcsString = "0+0";
            }

            else
            {
                for (int i = gameManager.calcs.Count - 1; i >= index - 1; i--)
                {
                    gameManager.calcs.RemoveAt(i);
                }
            }
        }

        if (selectedBlocks.Count > 0)
        {
            for (int i = selectedBlocks.Count - 1; i >= index - 1; i--)
            {
                for (int j = selectedBlocks[i].transform.childCount - 1; j >= 0; j--)
                {
                    Destroy(selectedBlocks[i].transform.GetChild(j).gameObject);
                }

                var swipeBlock = selectedBlocks[i].GetComponent<SwipeBlocks>();
                var blockColor = swipeBlock.myColor;
                selectedBlocks[i].GetComponent<SpriteRenderer>().material.SetColor("_Color", blockColor);
                selectedBlocks[i].GetComponent<SwipeBlocks>().selected = false;
                swipeBlock.IDCounter -= 1;
            }

            if (isDelete)
            {
                {
                    canBeSelected = false;
                    for (int j = selectedBlocks.Count - 1; j >= index - 1; j--)
                    {
                        //var destroyParticleInst = Instantiate(destroyParticle, selectedBlocks[j].gameObject.transform.position, Quaternion.identity);
                        //destroyParticleInst.transform.SetParent(selectedBlocks[j].gameObject.transform);

                        Destroy(selectedBlocks[j].gameObject, 1);
                        //selectedBlocks[j].GetComponent<Animator>().Play("ShrinkBlock");
                        LeanTween.scale(selectedBlocks[j].gameObject, new Vector2(0, 0), 1f);
                        var swipeBlock = selectedBlocks[j].GetComponent<SwipeBlocks>();
                        swipeBlock.IDCounter = 0;
                    }
                }
            }

            else
            {
                for (int i = selectedBlocks.Count - 1; i >= index - 1; i--)
                {selectedBlocks.RemoveAt(i);}
            }
        }
    }

    void Update()
    {
        //If 
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

        //Cleanup selectedBlocks if null items are present
        for (int j = 0; j <= selectedBlocks.Count - 1; j++)
        {
            if (selectedBlocks[j] == null)
            {
                selectedBlocks.Remove(selectedBlocks[j]);
            }
        }

        if (selectedBlocks.Count >= 2 && selectedBlocks[selectedBlocks.Count - 1] != null && selectedBlocks[selectedBlocks.Count - 1].GetComponent<SwipeBlocks>().myID > 1 && selectedBlocks[selectedBlocks.Count - 1].transform.childCount < 3)
        {
            if(!selectedBlocks[selectedBlocks.Count - 1].GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("ShrinkBlock"))
            {
                var currentLine = Instantiate(linePrefab, selectedBlocks[selectedBlocks.Count - 1].transform.position, Quaternion.identity);
                currentLine.transform.SetParent(selectedBlocks[selectedBlocks.Count - 1].transform);
                DrawLine currentLinePoints = currentLine.GetComponent<DrawLine>();
                currentLinePoints.origin = selectedBlocks[selectedBlocks.Count - 1].gameObject.transform.transform;
                currentLinePoints.destination = selectedBlocks[selectedBlocks.Count - 2].gameObject.transform.transform;
            }
        }

    }
}
