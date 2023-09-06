using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeBlocks : MonoBehaviour,  IPointerEnterHandler, IPointerExitHandler
{
    private AudioSource audioData;
    private float pitchValue = 1.0f;
    public AudioClip selectAudio, removeAudio;
    private string output;
    private SpawnBlocks spawnBlocks;
    private EquateScore equateScore;
    [HideInInspector]
    public string myString;

    public GameObject Sphere;
    public GameObject blockClickPrefab;

    [HideInInspector]
    public GameObject selectSphere;

    private GameObject blockParticle;

    public List<GameObject> SelectedBlocks = new();

    public bool selected;
    public bool lockBlock = false;
    public bool firstSelect;
    private bool isBeingSelected, isBeingRemoved;
    public string animationState;


    public int myID;
    public int myTrash;
    public int IDCounter;

    private Animator blockAnim;
    private SpriteRenderer spriteRenderer;

    public Color myColor;
    
    void Start()
    {
        audioData = gameObject.GetComponent<AudioSource>();
        audioData.pitch = pitchValue;
        IDCounter = 0;

        selected = false;
        firstSelect = true;
        isBeingSelected = false;
        isBeingRemoved = false;
        animationState = "Idle";

        var spawner = transform.parent;
        equateScore = spawner.GetComponent<EquateScore>();
        spawnBlocks = spawner.GetComponent<SpawnBlocks>();
        
        blockAnim = GetComponent<Animator>();
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
                    StartCoroutine(LerpPosition(new Vector2(transform.position.x, transform.position.y - 1), 0.21f));
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
                audioData.PlayOneShot(removeAudio);
                isBeingRemoved = true;
            }
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        StartCoroutine(RefreshSelection(0.1f));
    }

    void AddBlock()
    {
        firstSelect = false;
        selected = true;
        
        if (firstSelect)
        {
            pitchValue = 1.0f;
            audioData.pitch = pitchValue;
        }
        else
        {
            pitchValue = 1 + (SelectedBlocks.Count * 0.03f);
            audioData.pitch = pitchValue;
        }

        audioData.PlayOneShot(selectAudio);

        

        IDCounter += 1;
        myID = IDCounter;

        SelectedBlocks.Add(gameObject);
        equateScore.calcs.Add(myString);

        selectSphere = Instantiate(Sphere, gameObject.transform.position, Quaternion.identity);
        selectSphere.transform.SetParent(gameObject.transform);

        blockParticle = Instantiate(blockClickPrefab, gameObject.transform.position, Quaternion.identity);
        blockParticle.transform.SetParent(gameObject.transform);
        

        spriteRenderer.material.SetColor("_Color", Color.yellow);
    }

    IEnumerator LerpPosition(Vector2 targetPosition, float duration)
    {
        float time = 0;
        Vector2 startPosition = transform.position;

        while (time < duration)
        {
            transform.position = Vector2.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }

    IEnumerator RefreshSelection(float time)
    {
        isBeingSelected = true;
        isBeingRemoved = true;

        yield return new WaitForSeconds(time);

        isBeingSelected = false;
        isBeingRemoved = false;
    }

}
