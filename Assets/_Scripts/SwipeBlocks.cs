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
    private float pitchValue = 1.0f;
    private SpawnBlocks spawner;
    public GameObject sphere, blockClickPrefab;
    public bool selected, lockBlock, firstSelect;
    public string animationState;
    public int myID, IDCounter;
    public Color myColor;


    private Animator blockAnim;
    private SpriteRenderer spriteRenderer;
    private GameManager gameManager;
    private bool isBeingSelected, isBeingRemoved; //what??
    

    void Start()
    {
        spawner = transform.parent.GetComponent<SpawnBlocks>();
        
        gameManager = GameObject.Find("GameInterface").GetComponent<GameManager>();
        lockBlock = false;
        IDCounter = 0;

        selected = false;
        firstSelect = true;
        isBeingSelected = false;
        isBeingRemoved = false;
        animationState = "Idle";

        blockAnim = GetComponent<Animator>();
        sfxSource = gameManager.GetComponent<AudioSource>();
        sfxSource.pitch = pitchValue;
        //blockAnim.Play("Idle");
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.material.SetColor("_Color", myColor);
    }

    void Update()
    {
        if (spawner != null)
        {
            if (spawner.selectedBlocks.Count <= 0)
            {
                IDCounter = 0;
                firstSelect = true;
            }

            if (transform.position.y >= 1 && spawner.allBlocks[(int)transform.position.x, (int)transform.position.y - 1] == null && lockBlock == false)
            {
                for (int i = (int)transform.position.y; i >= 0; i--)
                {
                    if (spawner.allBlocks[(int)transform.position.x, i] == null)
                    {
                        LeanTween.move(gameObject, new Vector2(transform.position.x, transform.position.y - 1), 0.21f).setEasePunch();
                    }
                }
            }
            else
            {
                transform.position = new Vector2((int)transform.position.x, (int)transform.position.y);
                spawner.allBlocks[(int)transform.position.x, (int)transform.position.y] = gameObject;
                gameObject.name = "(" + transform.position.x.ToString() + "," + transform.position.y.ToString() + ")";
            }
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (!selected && !isBeingRemoved)
        {
            if (spawner.selectedBlocks.Count <= 6 && spawner.canBeSelected)
            {
                if (firstSelect && gameObject.tag != "Operation" && spawner.selectedBlocks.Count < 1)
                {isBeingSelected = true; AddBlock();}

                else if (spawner.selectedBlocks.Count >= 1) 
                {
                    if(spawner.selectedBlocks[spawner.selectedBlocks.Count - 1].tag != gameObject.tag)
                    {
                        if ((int)gameObject.transform.position.x <= (int)spawner.selectedBlocks[spawner.selectedBlocks.Count - 1].transform.position.x + 1 && (int)gameObject.transform.position.x >= (int)spawner.selectedBlocks[spawner.selectedBlocks.Count - 1].transform.position.x - 1)
                        {
                            if ((int)gameObject.transform.position.y <= (int)spawner.selectedBlocks[spawner.selectedBlocks.Count - 1].transform.position.y + 1 && (int)gameObject.transform.position.y >= (int)spawner.selectedBlocks[spawner.selectedBlocks.Count - 1].transform.position.y - 1)
                            {isBeingSelected = true; AddBlock();}
                        }
                    }
                }
            }
        }

        else if (selected && !isBeingSelected)
        {
            if (spawner.selectedBlocks.Contains(gameObject))
            {
                spawner.RemoveBlock(myID, false);
                sfxSource.PlayOneShot(removeAudio);
                isBeingRemoved = true;
            }
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        StartCoroutine(RefreshSelection());
    }

    void AddBlock()
    {
        firstSelect = false;
        selected = true;
        
        if (firstSelect)
        {
            pitchValue = 1.0f;
            sfxSource.pitch = pitchValue;
        }
        else
        {
            pitchValue = 1 + (spawner.selectedBlocks.Count * 0.03f);
            sfxSource.pitch = pitchValue;
        }

        sfxSource.PlayOneShot(selectAudio);
        
        IDCounter += 1;
        myID = IDCounter;

        spawner.selectedBlocks.Add(gameObject);
        gameManager.calcs.Add(myString);

        var selectSphere = Instantiate(sphere, gameObject.transform.position, Quaternion.identity);
        selectSphere.transform.SetParent(gameObject.transform);

        var blockParticle = Instantiate(blockClickPrefab, gameObject.transform.position, Quaternion.identity);
        blockParticle.transform.SetParent(gameObject.transform);
        

        spriteRenderer.material.SetColor("_Color", Color.yellow);
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
