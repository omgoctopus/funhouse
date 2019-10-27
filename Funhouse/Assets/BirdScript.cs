using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdScript : MonoBehaviour
{
    //raycastdistance, movespeed and flightdistance should be input as positive numbers. 
    //The facingright bool will 'aim' them in the correct direction
    //set raycastdistance to width of the room if camera x-axis if fixed
    //set it to half the screen-width if the camera scrolls so you don't aggro bird while off-screen
    //set flightdistance to the width of the room
    public float triggercountdown, launchdelay, movespeed, raycastdistance, flightdistance;
    public bool facingright;
    private float timetillaunch, finalxcoordinate;
    private bool birdhasvisionofplayer, birdhaslaunched, playerfound;
    private int raycastdirection;

    // Start is called before the first frame update
    void Start()
    {
        timetillaunch = triggercountdown;
        if (facingright == false)
        {
            raycastdirection = -1;
            movespeed = -movespeed;
            finalxcoordinate = transform.position.x - flightdistance;
        }
        if (facingright == true)
        {
            raycastdirection = 1;
            finalxcoordinate = transform.position.x + flightdistance;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(birdhaslaunched!=true)
        playerdetection();

        if (birdhasvisionofplayer == true)
        {
            timetillaunch -= Time.deltaTime;
        }
        if (timetillaunch <= 0)
        { Invoke("launch", launchdelay); ; }

        //destroy bird once it's within a pixel or two of final position
        if(transform.position.x<=finalxcoordinate+2 && transform.position.x >= finalxcoordinate - 2)
        {
            Debug.Log("Bird destroyed");
            Destroy(this.gameObject);
        }

    }

    void launch()
    {
        birdhaslaunched = true;
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(movespeed, 0);
    }

    void playerdetection()
    {
        RaycastHit2D[] hitB;
        hitB = Physics2D.RaycastAll(transform.position, new Vector2(raycastdirection, 0), raycastdistance);
        // you can increase RaycastLength and adjust direction for your case
        foreach (var hitedB in hitB)
        {
            if (hitedB.collider.gameObject.tag != "Player")
            {
                playerfound = false;
                continue;
            }
            if (hitedB.collider.gameObject.tag == "Player")
            {
                Debug.Log("birdhasvisionofplayer");
                playerfound = true;
                birdhasvisionofplayer = true;
                break;
            }
        }
        if (playerfound == false)
        {
            birdhasvisionofplayer = false;
            timetillaunch = triggercountdown;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Hazard" && birdhaslaunched!=true)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Hazard" && birdhaslaunched!=true)
        {
            Destroy(this.gameObject);
        }
    }

}
