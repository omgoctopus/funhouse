using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sneekenemyscript : MonoBehaviour
{
    // ATTACH THIS TO  SNEAKYSNEEKBODY


    private Vector2 startingposition;
    public float sneekmovespeed, sneekslowspeed;
    private float movespeed;
    public bool playerisinsneekrange = false, sneekisaggro = false, needtoresetposition=false, sneekseesplayer;
    private bool playerseessneek, ray1detectground, ray2detectground, ray3detectground, ray4detectground;
    private GameObject player;

    //stuff for vector math
    private Vector2 playercorner1, playercorner2, playercorner3, playercorner4, sneekcorner1, sneekcorner2, sneekcorner3, sneekcorner4;
    private Vector2 movedirection;
    //rays run from a point on the sneek to a point on the player 
    //ray 1 is topright, 2 is top left, 3 is bottomleft, 4 is bottomright

    // Start is called before the first frame update
    void Start()
    {
        startingposition = transform.position;
        movespeed = sneekmovespeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (needtoresetposition == true)
        { transform.position = startingposition;
            needtoresetposition = false;
        }

        if (sneekisaggro == true)
        {
            Debug.Log("you passed the sneek");
            checkforplayervision();
        }

        if (sneekseesplayer == true)
        {
            //check if player is facing sneek and adjust speed accordingly
            player = GameObject.FindGameObjectWithTag("Player");
            player_move playerscript = player.GetComponent<player_move>();
            if(playerscript.facingright==true && player.transform.position.x<transform.position.x)
            { movespeed = sneekslowspeed; }
            if (playerscript.facingright == false && player.transform.position.x > transform.position.x)
            { movespeed = sneekslowspeed; }
            if (playerscript.facingright == true && player.transform.position.x >transform.position.x)
            { movespeed = sneekmovespeed; }
            if (playerscript.facingright == false && player.transform.position.x < transform.position.x)
            { movespeed = sneekmovespeed; }

            //move sneek
            gameObject.GetComponent<Rigidbody2D>().velocity = movedirection.normalized * movespeed;
        }

        if (sneekseesplayer == false)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        }


    }

    void checkforplayervision()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        ray1detectground = false;
        ray2detectground = false;
        ray3detectground = false;
        ray4detectground = false;


        //define corner points of player
        playercorner1 = player.transform.position + new Vector3(8, 16,0);
        playercorner2 = player.transform.position + new Vector3(-8, 16, 0);
        playercorner3 = player.transform.position + new Vector3(-8, -15, 0);
        playercorner4 = player.transform.position + new Vector3(8, -15, 0);

        //define corner points of sneek
        sneekcorner1 = transform.position + new Vector3(8, 8, 0);
        sneekcorner2 = transform.position + new Vector3(-8, 8, 0);
        sneekcorner3 = transform.position + new Vector3(-8, -8, 0);
        sneekcorner4 = transform.position + new Vector3(8, -8, 0);

        //hitTL is raycast from topright of sneek to topright of player
        RaycastHit2D[] hitTR;
        hitTR = Physics2D.LinecastAll(sneekcorner1, playercorner1);
        foreach (var hitedTR in hitTR)
        {
            if (hitedTR.collider.gameObject.tag == "ground")
            {
                ray1detectground = true;
            }
            //else { ray1detectground = false; }

        }

        //hitTL is raycast from topleft of sneek to  topleft of player
        RaycastHit2D[] hitTL;
        hitTL = Physics2D.LinecastAll(sneekcorner2, playercorner2);
        foreach (var hitedTL in hitTL)
        {
            if (hitedTL.collider.gameObject.tag == "ground")
            {
                ray2detectground = true;
            }
            //else { ray2detectground = false; }
        }

        //hitBL is raycast from bottomleft of sneek to bottom left of player
        RaycastHit2D[] hitBL;
        hitBL = Physics2D.LinecastAll(sneekcorner3, playercorner3);
        foreach (var hitedBL in hitBL)
        {
            if (hitedBL.collider.gameObject.tag == "ground")
            {
                ray3detectground = true;
            }
            //else { ray3detectground = false; }
        }

        //hitBR is raycast from bottomright of sneek to  bottomright of player
        RaycastHit2D[] hitBR;
        hitBR = Physics2D.LinecastAll(sneekcorner4, playercorner4);
        foreach (var hitedBR in hitBR)
        {
            if (hitedBR.collider.gameObject.tag == "ground")
            {
                ray4detectground = true;
            }
            //else { ray4detectground = false; }
        }

        if(ray1detectground==false && ray2detectground==false && ray3detectground==false && ray4detectground == false)
        {
            sneekseesplayer = true;
            movedirection = player.transform.position - transform.position;
        }
        else { sneekseesplayer = false; }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Hazard")
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Hazard")
        {
            Destroy(this.gameObject);
        }
    }
}
