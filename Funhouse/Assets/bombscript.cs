using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bombscript : MonoBehaviour
{
    public float explosiontimer, changedirectiontimer, movespeed;
    private float explodetimeremaining, timetildirectionchange;
    private int movecheck, movedirection;
    private bool isexploding = false, needtoflipL, needtoflipR, readytoflip=true, grounded=true;
    private GameObject explosion, body;

    // Start is called before the first frame update
    void Start()
    {
        explodetimeremaining = explosiontimer;
        timetildirectionchange = changedirectiontimer;
        explosion= transform.GetChild(0).gameObject;
        body= transform.GetChild(1).gameObject;
        explosion.gameObject.SetActive(false);
        body.gameObject.SetActive(true);
        movecheck=Random.Range(0, 2);
        if (movecheck == 0)
            movedirection = -1;
        if (movecheck == 1)
            movedirection = 1;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("bombgrounded=" + grounded+",vel="+gameObject.GetComponent<Rigidbody2D>().velocity.x);

        //timers
        explodetimeremaining -= Time.deltaTime;
        timetildirectionchange -= Time.deltaTime;
        if (explodetimeremaining <= 0)
            explode();
        if (timetildirectionchange <= 0)
            randomdirectionchange();

        //movement
        if (isexploding != true && grounded == true)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(movedirection * movespeed, gameObject.GetComponent<Rigidbody2D>().velocity.y);
            Debug.Log("bomb is moving");
        }

        //check for ledge
        ledgecheck();
    }

    void explode()
    {
        isexploding = true;
        gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
        transform.gameObject.tag = "Hazard";
        explosion.gameObject.SetActive(true);
        body.gameObject.SetActive(false);
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
        Destroy(this.gameObject,0.5f);
    }

    void ledgecheck()
    {
        //old script mostly works but bounces of inanimate statue
        //RaycastHit2D[] hitL;
        //hitL = Physics2D.RaycastAll(transform.position + new Vector3(-7, -8, 0), Vector2.down, 1.0f);

        if (Physics2D.Raycast(transform.position + new Vector3(-7, -8.5f, 0), Vector2.down, 1.0f)) {
            //Debug.Log("left hit");
            needtoflipL = false; }
        else { needtoflipL = true;
            //Debug.Log("left miss");
        }

        //old script: 
        //mostly works, but bounces off inanimate statue
        // you can increase RaycastLength and adjust direction for your case
        //foreach (var hitedL in hitL)
        //{
        //    if (hitedL.collider.gameObject.tag == "ground")
        //    { //Change it to match ground tag
        //        needtoflipL = false;
        //    }
        //    else { needtoflipL = true; }
        //}

        //old script. mostly works, but bounces off inanimate statue
        //RaycastHit2D[] hitR;
        //hitR = Physics2D.RaycastAll(transform.position + new Vector3(7, -8, 0), Vector2.down, 1.0f);

        if (Physics2D.Raycast(transform.position + new Vector3(7, -8.5f, 0), Vector2.down, 1.0f)){ needtoflipR = false; }
        else { needtoflipR = true; }

        //old script. mostly works, but bounces of inanimate statue
        // you can increase RaycastLength and adjust direction for your case
        //foreach (var hitedR in hitR)
        //{
        //    if (hitedR.collider.gameObject.tag == "ground")
        //    { //Change it to match ground tag
        //        needtoflipR = false;
        //    }
        //    else { needtoflipR = true; }
        //    
        //}

        //flip when one raycast detects collider and the other doesn't
        if (needtoflipL == true && needtoflipR == false)
        {
            grounded = true;
            if(readytoflip==true)
                flip();
        }
        if (needtoflipL == false && needtoflipR == true)
        {
            grounded = true;
            if (readytoflip == true)
                flip();
        }
        if(needtoflipL==false && needtoflipR == false)
        {
            grounded = true;
            readytoflip = true;
        }
        //if neither raycast detects a surface, bomb isn't grounded
        if (needtoflipL == true && needtoflipR == true)
        {
            grounded = false;
        }

        //hitB is raycast that checks for collision in bottom half of statue body
        RaycastHit2D[] hitB;
        hitB = Physics2D.RaycastAll(transform.position + new Vector3(8 * movedirection, -6, 0), new Vector2(movedirection, 0), 1.0f);
        // you can increase RaycastLength and adjust direction for your case
        foreach (var hitedB in hitB)
        {
            if (hitedB.collider.gameObject.tag == "ground")
            {
                turnaroundatwall();
            }

        }

    }

    void flip()
    {
        Debug.Log("flip was called");
        timetildirectionchange = changedirectiontimer;
        if (movedirection > 0) { movedirection = -1; }
        else { movedirection = 1; }
        readytoflip = false;
    }

    void turnaroundatwall()
    {
        Debug.Log("turnaroundatwall was called");
        timetildirectionchange = changedirectiontimer;
        if (movedirection > 0) { movedirection = -1; }
        else { movedirection = 1; }
    }


    void randomdirectionchange()
    {
        Debug.Log("changing direction");
        movecheck = Random.Range(0, 2);
        if (movecheck == 0)
            movedirection = -1;
        if (movecheck == 1)
            movedirection = 1;
        timetildirectionchange = changedirectiontimer;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Hazard" & isexploding!=true)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Hazard" & isexploding!=true)
        {
            Destroy(this.gameObject);
        }
    }
}
