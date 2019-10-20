using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombSpawnerScript : MonoBehaviour
{
    public bool readytospawn, needtospawn;
    public GameObject bomb;

    // Start is called before the first frame update
    void Start()
    {
        readytospawn = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("readytospawnbomb=" + readytospawn);
        if (needtospawn == true && readytospawn == true)
        {
            spawnbomb();
        }
    }

    void spawnbomb()
    {
        Instantiate(bomb, transform.position, Quaternion.identity);
        Debug.Log("spawning bomb at" + transform.position);
        readytospawn = false;
        needtospawn = false;
    }
}
