using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeBlocks : MonoBehaviour,  IPointerEnterHandler, IPointerExitHandler
{
    [Header("String")]
    public string myString;
    
    [Header("Properties")]
    private AudioSource sfxSource;
    public AudioClip selectAudio, removeAudio;
    public GameObject sphere, blockClickPrefab;
    public bool selected, lockBlock, firstSelect;
    public string animationState;
    public int myID, IDCounter;
    public Color myColor;
    public Sprite[] breakSprites;

    private float pitchValue = 1.0f;
    private Animator blockAnim;
    private SpriteRenderer spriteRenderer;
    private GameManager gameManager;
    private bool isBeingSelected, isBeingRemoved, isSuperBlock; //literally used to prevent number being selected/unselected too quickly. So stupid, such a bad solution. i give it a D-
    [SerializeField]private int breakCounter;

    void Start()
    {
        gameManager = GameObject.Find("GameInterface").GetComponent<GameManager>();
        lockBlock = false;
        IDCounter = 0;
        breakCounter = 5;

        selected = false;
        firstSelect = true;
        isBeingSelected = false;
        isBeingRemoved = false;
        animationState = "Idle";

        blockAnim = GetComponent<Animator>();
        sfxSource = gameManager.GetComponent<AudioSource>();
        sfxSource.pitch = pitchValue;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        if (myString == "")
        {
            spriteRenderer.sprite = breakSprites[breakCounter - 1];
        }

        var randval = Random.Range(0, 5);
        
        {
            if (randval > 1)
            {
                myColor = Color.white;
                isSuperBlock = false;
            }

            else
            {
                myColor = new Color(1f, 165/255f, 0f);
                isSuperBlock = true;
            }
        }

        spriteRenderer.color = myColor;
    }

    void Update()
    {
        if (gameManager != null)
        {
            if (gameManager.selectedBlocks.Count <= 0)
            {
                //If no blocks are selected, this block can be the first to be selected
                IDCounter = 0;
                firstSelect = true;
            }

            if (!gameManager.equationResetFlag)
            {
                // transform.position.y >= 1: If this block isn't on the bottom row
                // gameManager.allBlocks[(int)transform.position.x, (int)transform.position.y - 1] == null: And there's nothing below you
                // !lockBlock: Not in a condition where block is in a fixed pos (e.g. when shuffling)
                if (transform.position.y >= 1 && gameManager.allBlocks[(int)transform.position.x, (int)transform.position.y - 1] == null && !lockBlock)
                {
                    LeanTween.move(gameObject, new Vector2(transform.position.x, (int)transform.position.y - 1), 0.21f);
                    
                    /*
                    for (int i = (int)transform.position.y; i >= 0; i--)
                    {
                        if (gameManager.allBlocks[(int)transform.position.x, i] == null)
                        {
                            
                        }
                    }*/
                }
                else
                {
                    //transform.position = new Vector2((int)transform.position.x, (int)transform.position.y);
                    gameManager.allBlocks[(int)transform.position.x, (int)transform.position.y] = this;
                    gameObject.name = "(" + transform.position.x + "," + transform.position.y + ")";
                }
            }
        }
    }

    //When block is clicked on
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (myString != "")
        {
            if (!selected && !isBeingRemoved)
            {
                if (gameManager.selectedBlocks.Count <= 6 && gameManager.canBeSelected)
                {
                    if (firstSelect && gameObject.tag != "Operation" && gameManager.selectedBlocks.Count < 1)
                    {
                        isBeingSelected = true; 
                        AddBlock();
                    }

                    else if (gameManager.selectedBlocks.Count >= 1) 
                    {
                        if(gameManager.selectedBlocks[gameManager.selectedBlocks.Count - 1].tag != gameObject.tag)
                        {
                            if ((int)gameObject.transform.position.x <= (int)gameManager.selectedBlocks[gameManager.selectedBlocks.Count - 1].transform.position.x + 1 && (int)gameObject.transform.position.x >= (int)gameManager.selectedBlocks[gameManager.selectedBlocks.Count - 1].transform.position.x - 1)
                            {
                                if ((int)gameObject.transform.position.y <= (int)gameManager
                                        .selectedBlocks[gameManager.selectedBlocks.Count - 1].transform.position.y + 1 &&
                                    (int)gameObject.transform.position.y >= (int)gameManager
                                        .selectedBlocks[gameManager.selectedBlocks.Count - 1].transform.position.y - 1)
                                {
                                    isBeingSelected = true; 
                                    AddBlock();
                                }
                            }
                        }
                    }
                }
            }
            
            else if (selected && !isBeingSelected)
            {
                if (gameManager.selectedBlocks.Contains(this))
                {
                    gameManager.RemoveBlock(myID);
                    sfxSource.PlayOneShot(removeAudio);
                    isBeingRemoved = true;
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        StartCoroutine(RefreshSelection());
    }

    void AddBlock()
    {
        if (firstSelect)
        {
            pitchValue = 1.0f;
            sfxSource.pitch = pitchValue;
        }
        else
        {
            pitchValue = 1 + (gameManager.selectedBlocks.Count * 0.03f);
            sfxSource.pitch = pitchValue;
        }
        
        firstSelect = false;
        selected = true;

        sfxSource.PlayOneShot(selectAudio);
        
        IDCounter++;
        myID = IDCounter;

        gameManager.selectedBlocks.Add(this);
        gameManager.calcs.Add(myString);

        var selectSphere = Instantiate(sphere, gameObject.transform.position, Quaternion.identity);
        selectSphere.transform.SetParent(gameObject.transform);

        var blockParticle = Instantiate(blockClickPrefab, gameObject.transform.position, Quaternion.identity);
        blockParticle.transform.SetParent(gameObject.transform);
        Destroy(blockParticle, 2.5f);


        spriteRenderer.color = Color.yellow;
    }

    public void ReduceBreakCounter()
    {
        int randomInt = 2;
        //if a move has been made in level 4, reduce break counter until block breaks

        if (breakCounter > 1)
        {
            breakCounter--;
            spriteRenderer.sprite = breakSprites[breakCounter - 1];
        }
        
        else
        {
            while (randomInt == 2)
            {
                randomInt = Random.Range(0, 12);
                SwipeBlocks randomBlock = gameManager.Blocks[randomInt].GetComponent<SwipeBlocks>();
                myString = randomBlock.myString;
                spriteRenderer.sprite = randomBlock.GetComponent<SpriteRenderer>().sprite;
            }
        }
    }

    private IEnumerator RefreshSelection()
    {
        isBeingSelected = true;
        isBeingRemoved = true;

        yield return new WaitForSeconds(0.1f);

        isBeingSelected = false;
        isBeingRemoved = false;
    }
}
