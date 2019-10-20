using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class livingstatue_enemyscript : MonoBehaviour
{
    public bool statueisalive = false;
    public int EnemySpeed;
    private bool statuemovementhasstarted=false;
    private int XmoveDirection;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        //gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (statueisalive == true && statuemovementhasstarted == false)
            statuestartup();

        if (statuemovementhasstarted == true)
        {
            
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(XmoveDirection * EnemySpeed, gameObject.GetComponent<Rigidbody2D>().velocity.y);
            wallcheck();
        }

    }

    void wallcheck()
    {
        //hitT is raycast that checks for collision in top half of statue body
        RaycastHit2D[] hitT;
        hitT = Physics2D.RaycastAll(transform.position + new Vector3(8*XmoveDirection, 7, 0), new Vector2(XmoveDirection,0), 1.0f);
        foreach (var hitedT in hitT)
        {
            if (hitedT.collider.gameObject.tag == "ground")
            { 
                flip();
            }

        }
        //hitB is raycast that checks for collision in bottom half of statue body
        RaycastHit2D[] hitB;
        hitB = Physics2D.RaycastAll(transform.position + new Vector3(8 * XmoveDirection, -7, 0), new Vector2(XmoveDirection, 0), 1.0f);
        // you can increase RaycastLength and adjust direction for your case
        foreach (var hitedB in hitB)
        {
            if (hitedB.collider.gameObject.tag == "ground")
            {
                flip();
            }

        }

    }

    void flip()
    {
        if (XmoveDirection > 0) { XmoveDirection = -1; }
        else { XmoveDirection = 1; }
    }

    //this is where we locate player and establish initial move direction
    //also where we enable statue's collider
    void statuestartup()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player.transform.position.x > transform.position.x)
        { XmoveDirection = 1; }
        else { XmoveDirection = -1; }

        Debug.Log("playerx=" + player.transform.position.x + " statuex="+transform.position.x);

        //just change layers instead of turning on box collider.  this way bomb can kill statue before it awakes
        //gameObject.GetComponent<BoxCollider2D>().enabled = true;
        //gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
        gameObject.layer = 0;

        statuemovementhasstarted = true;
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
        if (collision.gameObject.tag == "Hazard")
        {
            Destroy(this.gameObject);
        }
    }
}
