using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spearman : MonoBehaviour
{
    public float movespeed;
    private int movedirection;
    private bool isexploding = false, needtoflipL, needtoflipR, readytoflip = true, grounded = true, facingright, beingpushed;

    // Start is called before the first frame update
    void Start()
    {
        movedirection = 1;
        facingright = true;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(movedirection * movespeed, gameObject.GetComponent<Rigidbody2D>().velocity.y);

        //check for ledge (but don't check for ledge while being pushed by the player)
        if (beingpushed == false)
        { ledgecheck(); }

    }
    void ledgecheck()
    {

        if (Physics2D.Raycast(transform.position + new Vector3(-7, -16.5f, 0), Vector2.down, 1.0f))
        {
            //Debug.Log("left hit");
            needtoflipL = false;
        }
        else
        {
            needtoflipL = true;
            //Debug.Log("left miss");
        }


        if (Physics2D.Raycast(transform.position + new Vector3(7, -16.5f, 0), Vector2.down, 1.0f)) { needtoflipR = false; }
        else { needtoflipR = true; }

        //flip when one raycast detects collider and the other doesn't
        if (needtoflipL == true && needtoflipR == false)
        {
            grounded = true;
            if (readytoflip == true)
                flip();
        }
        if (needtoflipL == false && needtoflipR == true)
        {
            grounded = true;
            if (readytoflip == true)
                flip();
        }
        if (needtoflipL == false && needtoflipR == false)
        {
            grounded = true;
            readytoflip = true;
        }
        //if neither raycast detects a surface, spearman isn't grounded
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
        facingright = !facingright;
        Vector2 localscale = gameObject.transform.localScale;
        localscale.x *= -1;
        transform.localScale = localscale;
        if (movedirection > 0) { movedirection = -1; }
        else { movedirection = 1; }
        readytoflip = false;
    }

    void turnaroundatwall()
    {
        if (movedirection > 0) { movedirection = -1; }
        else { movedirection = 1; }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Hazard")
        {
            Destroy(this.gameObject);
        }
        //keep track of whether or not spearman is being pushed by the player
        if (collision.gameObject.tag == "Player")
        {
            beingpushed = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            beingpushed = false;
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
