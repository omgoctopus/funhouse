using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonEnemyScript : MonoBehaviour
{
    public bool cannonisfiring, needtoresetposition;
    private Vector2 startingposition;
    private int facing = -1;
    public float cannontimer;
    private float cannoncountdown;
    private GameObject cannonball;

    // Start is called before the first frame update
    void Start()
    {
        startingposition = transform.position;
        cannoncountdown = cannontimer;
        cannonball = transform.parent.gameObject.transform.GetChild(2).gameObject;
        cannonball.gameObject.SetActive(false);
        if (transform.localScale.x < 0)
        {
            facing = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (needtoresetposition == true)
        {
            transform.position = startingposition;
            needtoresetposition = false;
        }

        if (cannonisfiring == false)
        {
            cannoncountdown = cannontimer;
        }

        if (cannonisfiring == true)
        {
            cannoncountdown -= Time.deltaTime;
            if (cannoncountdown <= 0)
                fireprojectile();
        }
    }
    void fireprojectile()
    {
        cannonball.transform.position = transform.position + new Vector3(4*facing, 4);
        cannonball.gameObject.SetActive(true);
        //if cannnon is facing right, tell cannonball which direction to go
        if (transform.localScale.x < 0)
        {
            CannonBallScript cannonballscript = cannonball.GetComponent<CannonBallScript>();
            cannonballscript.firedirection = 1;
        }
        cannoncountdown = cannontimer;
    }

}
