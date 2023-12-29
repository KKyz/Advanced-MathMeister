using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public GameManager gameManager;

    void Start()
    {
        gameManager = transform.parent.parent.GetComponent<GameManager>();
    }
    
    void Update()
    {
        if (gameManager.spawner != null)
        {
            for (int i = gameManager.spawner.childCount - 1; i >= 0; i--)
            {
                if (gameManager.spawner.GetChild(i).name == "(" +  transform.position.x + "," + transform.position.y + ")")
                {
                    gameManager.allBlocks[(int)transform.position.x, (int)transform.position.y] = gameManager.spawner.GetChild(i).GetComponent<SwipeBlocks>();
                }
                else
                {
                    gameManager.allBlocks[(int)transform.position.x, (int)transform.position.y] = null;
                }
            }
        }
    }
}