using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombPlayerScript : MonoBehaviour
{
    private GameObject bombspawner;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerExit2D(Collider2D trig)
    {

        //bomb should be reaady to spawn again if you leave the area
        if (trig.name == "BombSpawnReset")
        {           
            bombspawner = trig.transform.parent.gameObject;
            BombSpawnerScript spawnerscript = bombspawner.GetComponent<BombSpawnerScript>();
            spawnerscript.readytospawn = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D trig)
    {
        Debug.Log("entered bomb spawn area");
        if (trig.name == "BombSpawnTrigger")
        {
            bombspawner = trig.transform.parent.gameObject;
            BombSpawnerScript spawnerscript = bombspawner.GetComponent<BombSpawnerScript>();
            spawnerscript.needtospawn = true;
            //Debug.Log("bomb spawned at" bombspawner.transform.parent.gameObject);

        }
    }
}
