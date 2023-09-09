using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public Transform Spawner;

    void Start()
    {
        Spawner = transform.parent;
    }
    void Update()
    {
        for (int i = Spawner.transform.childCount - 1; i >= 0; i--)
        {
            if (Spawner.transform.GetChild(i).name == "(" +  transform.position.x + "," + transform.position.y + ")")
            {
                SpawnBlocks.allBlocks[(int)transform.position.x, (int)transform.position.y] = Spawner.transform.GetChild(i).gameObject;
            }
            else
            {
                SpawnBlocks.allBlocks[(int)transform.position.x, (int)transform.position.y] = null;
            }
        }
    }
}
