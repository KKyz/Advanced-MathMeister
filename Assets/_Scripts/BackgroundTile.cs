using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public SpawnBlocks spawner;

    void Start()
    {
        spawner = transform.parent.GetComponent<SpawnBlocks>();
    }
    void Update()
    {
        for (int i = spawner.transform.childCount - 1; i >= 0; i--)
        {
            if (spawner.transform.GetChild(i).name == "(" +  transform.position.x + "," + transform.position.y + ")")
            {
                spawner.allBlocks[(int)transform.position.x, (int)transform.position.y] = spawner.transform.GetChild(i).gameObject;
            }
            else
            {
                spawner.allBlocks[(int)transform.position.x, (int)transform.position.y] = null;
            }
        }
    }
}
