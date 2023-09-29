using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeBlocks : MonoBehaviour,  IPointerEnterHandler, IPointerExitHandler
{
    private AudioSource sfxSource;
    public AudioClip selectAudio, removeAudio;
    
    private float pitchValue = 1.0f;
    private SpawnBlocks spawnBlocks;
    
    [HideInInspector]
    public string myString;

    public GameObject sphere;
    public GameObject blockClickPrefab;

    public List<GameObject> SelectedBlocks = new();

    public bool selected, lockBlock, firstSelect;
    private bool isBeingSelected, isBeingRemoved; //what??
    public string animationState;


    public int myID, IDCounter;

    private Animator blockAnim;
    private SpriteRenderer spriteRenderer;
    private GameManager gameManager;

    public Color myColor;
    
    void Start()
    {
        gameManager = GameObject.Find("GameInterface").GetComponent<GameManager>();
        lockBlock = false;
        sfxSource.pitch = pitchValue;
        IDCounter = 0;

        selected = false;
        firstSelect = true;
        isBeingSelected = false;
        isBeingRemoved = false;
        animationState = "Idle";
        
        spawnBlocks = transform.parent.GetComponent<SpawnBlocks>();
        
        blockAnim = GetComponent<Animator>();
        sfxSource = gameManager.GetComponent<AudioSource>();
        myString = gameObject.GetComponent<AssignString>().blockStr;
        blockAnim.Play("Idle");
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.material.SetColor("_Color", myColor);
    }

    void Update()
    {
        if (SelectedBlocks.Count <= 0)
        {
            IDCounter = 0;
            firstSelect = true;
        }

        if (transform.position.y >= 1 && SpawnBlocks.allBlocks[(int)transform.position.x, (int)transform.position.y - 1] == null && lockBlock == false)
        {
            for (int i = (int)transform.position.y; i >= 0; i--)
            {
                if (SpawnBlocks.allBlocks[(int)transform.position.x, i] == null)
                {
                    LeanTween.move(gameObject, new Vector2(transform.position.x, transform.position.y - 1), 0.21f);
                }
            }
        }
        else
        {
            transform.position = new Vector2((int)transform.position.x, (int)transform.position.y);
            SpawnBlocks.allBlocks[(int)transform.position.x, (int)transform.position.y] = gameObject;
            gameObject.name = "(" + transform.position.x.ToString() + "," + transform.position.y.ToString() + ")";
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (!selected && !isBeingRemoved)
        {
            if (SelectedBlocks.Count <= 6 && spawnBlocks.canBeSelected)
            {
                if (firstSelect && gameObject.tag != "Operation" && SelectedBlocks.Count < 1)
                {isBeingSelected = true; AddBlock();}

                else if (SelectedBlocks.Count >= 1) 
                {
                    if(SelectedBlocks[SelectedBlocks.Count - 1].tag != gameObject.tag)
                    {
                        if ((int)gameObject.transform.position.x <= (int)SelectedBlocks[SelectedBlocks.Count - 1].transform.position.x + 1 && (int)gameObject.transform.position.x >= (int)SelectedBlocks[SelectedBlocks.Count - 1].transform.position.x - 1)
                        {
                            if ((int)gameObject.transform.position.y <= (int)SelectedBlocks[SelectedBlocks.Count - 1].transform.position.y + 1 && (int)gameObject.transform.position.y >= (int)SelectedBlocks[SelectedBlocks.Count - 1].transform.position.y - 1)
                            {isBeingSelected = true; AddBlock();}
                        }
                    }
                }
            }
        }

        else if (selected && !isBeingSelected)
        {
            if (SelectedBlocks.Contains(gameObject))
            {
                spawnBlocks.RemoveBlock(myID, false);
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
            pitchValue = 1 + (SelectedBlocks.Count * 0.03f);
            sfxSource.pitch = pitchValue;
        }

        sfxSource.PlayOneShot(selectAudio);
        
        IDCounter += 1;
        myID = IDCounter;

        SelectedBlocks.Add(gameObject);
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
