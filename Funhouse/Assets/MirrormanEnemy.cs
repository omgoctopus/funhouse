using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrormanEnemy : MonoBehaviour
{
    //notetoself: make sure mirrorman doesn't move while game is paused
    public float playerspeed, jumptime;
    public bool playerisinrange=false, needtoresetposition=false;
    private Vector2 newposition, startingposition, externalforce = new Vector2(0, 0), platformforce = new Vector2(0, 0), slopenormal, playermovedirection;
    public bool facingright = true;
    public float timeuntildismount = 0.12f, ladderX, moveX, moveY, accelerationtime, sensitivity, decelerationtime, accelerationrate, decelerationrate;
    public bool isGrounded, ladderaccess, onladder, readytodismount, readyfordoor, onslope, needtostop;
    private bool playerjumped = false;
    private GameObject laddertop;
    public int playersustainedjumppower = 1250, playerstartingjumppower;


    private float jumptimecounter, leaveladdertimer = 0.0f, accelerationtimer = 0.0f, decelerationtimer = 0.0f, speedfactor, startingspeed, movespeedtimer, initialmovespeed;

    // Start is called before the first frame update
    void Start()
    {
        startingposition = transform.position;
    }

    private void FixedUpdate()
    {
        //this came from void playermove
        if (Input.GetKey(GameManager.GM.jump) && playerjumped == true)
        {
            if (jumptimecounter > 0)
            {
                GetComponent<Rigidbody2D>().AddForce(Vector2.up * playersustainedjumppower);
                //Debug.Log(jumptimecounter);
                jumptimecounter -= Time.deltaTime;
            }
            else
            {
                playerjumped = false;
            }
        }

        if (Input.GetKeyUp(GameManager.GM.jump))
        {
            playerjumped = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("mirrormanisgrounded=" + isGrounded);

        if (needtoresetposition == true)
        {
            transform.position = startingposition;
            needtoresetposition = false;
        }

        if (playerisinrange == true)
        {
            GroundedUpdater();
            mirrormanmovement(); }

        //turn off gravity while on slopes so mirrorman doesn't slide down
        if (onslope == true)
        {
            GetComponent<Rigidbody2D>().gravityScale = 0.0f;
        }
        if (onslope != true && onladder != true)
        {
            GetComponent<Rigidbody2D>().gravityScale = 1.0f;
        }
    }

    void mirrormanmovement()
    {
        // COPIED FROM PLAYER_MOVE
        //Debug.Log("needtostop=" + needtostop + " movespeed=" + speedfactor+" deceltimer="+decelerationtimer+" acceltimer="+accelerationtimer+" facingright="+facingright);

        //left-right controls
        //enable joystick:
        moveX = -Input.GetAxis(GameManager.GM.Horizontal);

        // can only jump if grounded
        if (Input.GetKeyDown(GameManager.GM.jump) && isGrounded == true) { jump(); }



        // used to like idea of jumping off of ladder. now I don't.
        //if (Input.GetKeyDown(GameManager.GM.jump) && onladder == true && moveX !=0) { jump(); }

        //these lines allow player to use keys or buttons if they prefer
        if (Input.GetKey(GameManager.GM.right))
        { moveX = -1; }

        if (Input.GetKey(GameManager.GM.left))
        {
            moveX = 1;
        }

        if (moveX > sensitivity)
            moveX = 1;

        if (moveX < -1 * sensitivity)
            moveX = -1;

        if (moveY > sensitivity)
            moveY = 1;

        if (moveY < -1 * sensitivity)
            moveY = -1;

        //if inputs don't meet sensitivity threshold, set moveX to 0 and reset accelerationtimer
        if (!Input.GetKey(GameManager.GM.right) && !Input.GetKey(GameManager.GM.left) && Input.GetAxisRaw(GameManager.GM.Horizontal) < sensitivity && Input.GetAxisRaw(GameManager.GM.Horizontal) > -1 * sensitivity)
        {
            moveX = 0;
            accelerationtimer = 0;
        }

        //moveX acceleration
        if (moveX > 0 && gameObject.GetComponent<Rigidbody2D>().velocity.x >= 0)
        {
            needtostop = false;
            decelerationtimer = 0;
            accelerationtimer += Time.deltaTime;
            if (accelerationtimer > accelerationtime)
            {
                speedfactor = 1;
            }
            else
            {
                speedfactor = 0.5f + accelerationtimer / accelerationtime / 2;
            }
        }

        if (moveX < 0 && gameObject.GetComponent<Rigidbody2D>().velocity.x <= 0)
        {
            needtostop = false;
            decelerationtimer = 0;
            accelerationtimer += Time.deltaTime;
            if (accelerationtimer > accelerationtime)
            {
                speedfactor = -1;
            }
            else
            {
                speedfactor = -0.5f - accelerationtimer / accelerationtime / 2;
            }
        }

        //moveX deceleration
        if (speedfactor != 0 && moveX == 0 && needtostop != true)
        {
            startingspeed = speedfactor;
            needtostop = true;
        }

        if (moveX > 0 && gameObject.GetComponent<Rigidbody2D>().velocity.x < 0)
        {
            accelerationtimer = 0;
            startingspeed = speedfactor;
            needtostop = true;
        }

        if (moveX < 0 && gameObject.GetComponent<Rigidbody2D>().velocity.x > 0)
        {
            accelerationtimer = 0;
            startingspeed = speedfactor;
            needtostop = true;
        }

        if (needtostop == true)
        {
            decelerationtimer += Time.deltaTime;
            if (decelerationtimer > decelerationtime)
            {
                speedfactor = 0;
                decelerationtimer = 0;
                needtostop = false;
            }
            else
            {
                speedfactor = startingspeed * (1 - decelerationtimer / decelerationtime);
            }
        }

        if (onladder != true && gameObject.GetComponent<Rigidbody2D>().velocity.x == 0)
        {
            needtostop = false;
        }

        //up-down controls
        moveY = Input.GetAxis(GameManager.GM.Vertical);

        if (Input.GetKey(GameManager.GM.up))
        { moveY = 1; }

        if (Input.GetKey(GameManager.GM.down))
        {
            moveY = -1;
        }

        if (!Input.GetKey(GameManager.GM.up) && !Input.GetKey(GameManager.GM.down) && Input.GetAxisRaw(GameManager.GM.Vertical) < sensitivity && Input.GetAxisRaw(GameManager.GM.Vertical) > -1 * sensitivity)
        { moveY = 0; }

        //if you have just used a door,
        //Y-axis has to be reset to zero before you can enter a door again
        if (moveY == 0)
            readyfordoor = true;


        //animations
        //player direction
        if (moveX < -0.02 && facingright == true)
        {
            FlipPlayer();
        }
        else if (moveX > 0.02 && facingright == false)
        {
            FlipPlayer();
        }
        //left-right movement

        //old left-right movement
        //if(onladder!=true)
        //gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(moveX * playerspeed, gameObject.GetComponent<Rigidbody2D>().velocity.y);

        // this code lets you jump normally and fall normally once you're off the ground 
        //NOTE CHANGED moveX TO speedfactor FOR THE NEXT TWO SECTIONS
        if (onladder != true)
        {
            //if you recently jumped or are not on a slope, use 'normal' movemennt
            if (onslope != true || playerjumped == true && moveX != 0)
                gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(speedfactor * playerspeed, gameObject.GetComponent<Rigidbody2D>().velocity.y) + externalforce;

            if (moveX == 0)
                gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.GetComponentInParent<Rigidbody2D>().velocity.x, gameObject.GetComponent<Rigidbody2D>().velocity.y) + externalforce + platformforce;
        }

        //this code attempts to correct velocity path on slopes
        if (onladder != true && onslope == true && playerjumped == false)
        {
            playermovedirection = -Vector2.Perpendicular(slopenormal);
            gameObject.GetComponent<Rigidbody2D>().velocity = speedfactor * playerspeed * playermovedirection;
        }

        //add idle movement to wakeup physics engine for ontriggerstay
        if (onladder != true && gameObject.GetComponent<Rigidbody2D>().velocity == new Vector2(0, 0))
        {
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * .01f);
        }

        //ladder movement
        if (moveY != 0 && ladderaccess == true)
        {
            onladder = true;
        }

        //if (onladder == true &&  isGrounded!=true)
        if (onladder == true)
        {
            Debug.Log("on ladder, moveY =" + moveY + " moveX =" + moveX);

            //alloow vertical movement
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, moveY * playerspeed * 0.5f);

            //if you're on the ladder, snap xposition to center of ladder
            if (transform.position.x > ladderX || transform.position.x < ladderX)
            {
                newposition = new Vector2(ladderX, transform.position.y);
                transform.position = newposition;
            }
            //if you're on a ladder, not affected by gravity
            GetComponent<Rigidbody2D>().gravityScale = 0.0f;

            //if you're on a ladder move to a layer that can pass through the top of the ladder
            //gameObject.layer = 8;
        }




        //this lets you get off the ladder when you're at the bottom
        if (onladder == true && isGrounded == true && moveY == 0 && moveX != 0)
            onladder = false;

        // let's you get off ladder if you hold left or right for more than 0.1 second
        if (onladder == true && moveX != 0)
        {
            leaveladdertimer += Time.deltaTime;
            if (leaveladdertimer > timeuntildismount)
            {
                onladder = false;
            }
        }

        // resets 0.1 second countdown if you stop holding left or right
        if (onladder == true && moveX == 0)
        {
            leaveladdertimer = 0;
        }

        if (onladder == false)
        {
            //if you're not climbing the ladder, gravity should act normally
            GetComponent<Rigidbody2D>().gravityScale = 1.0f;
            //if you're not climbing the ladder, you should be able to stand on the top of it
            if (laddertop != null)
                Physics2D.IgnoreCollision(laddertop.GetComponent<Collider2D>(), GetComponent<Collider2D>(), false);
        }
    }

    void GroundedUpdater()
    {
        isGrounded = false; //Set to false every frame by default
        onslope = false;
        RaycastHit2D[] hit;
        hit = Physics2D.RaycastAll(transform.position + new Vector3(0, -16, 0), Vector2.down, 1.0f);
        // you can increase RaycastLength and adjust direction for your case
        foreach (var hited in hit)
        {
            if (hited.collider.gameObject == gameObject) //Ignore my character
                continue;
            // Don't forget to add tag to your ground
            if (hited.collider.gameObject.tag == "ground")
            { //Change it to match ground tag
                isGrounded = true;
            }

            //if you are not currently climbing ladder, the ladder tops should 'ground' the character
            if (hited.collider.gameObject.tag == "laddertop" && onladder != true)
            { //Change it to match ground tag
                isGrounded = true;
            }
        }
        RaycastHit2D[] hitL;
        hitL = Physics2D.RaycastAll(transform.position + new Vector3(-7, -16, 0), Vector2.down, 1.0f);
        // you can increase RaycastLength and adjust direction for your case
        foreach (var hitedL in hitL)
        {
            if (hitedL.collider.gameObject == gameObject) //Ignore my character
                continue;
            // Don't forget to add tag to your ground
            if (hitedL.collider.gameObject.tag == "ground")
            { //Change it to match ground tag
                isGrounded = true;
            }
            //if you are not currently climbing ladder, the ladder tops should 'ground' the character
            if (hitedL.collider.gameObject.tag == "laddertop" && onladder != true)
            { //Change it to match ground tag
                isGrounded = true;
            }
            //if raycast detects a slope, add a vertical force doesn't fall down slope
            if (hitedL.collider.gameObject.layer == 8 && moveX == 0)
            {
                //GetComponent<Rigidbody2D>().AddForce(Vector2.up * 1000);
                onslope = true;
            }

        }
        RaycastHit2D[] hitR;
        hitR = Physics2D.RaycastAll(transform.position + new Vector3(7, -16, 0), Vector2.down, 1.0f);
        // you can increase RaycastLength and adjust direction for your case
        foreach (var hitedR in hitR)
        {
            if (hitedR.collider.gameObject == gameObject) //Ignore my character
                continue;
            // Don't forget to add tag to your ground
            if (hitedR.collider.gameObject.tag == "ground")
            { //Change it to match ground tag
                isGrounded = true;
            }
            //if you are not currently climbing ladder, the ladder tops should 'ground' the character
            if (hitedR.collider.gameObject.tag == "laddertop" && onladder != true)
            { //Change it to match ground tag
                isGrounded = true;
            }
            //if raycast detects a slope, add a vertical force doesn't fall down slope
            if (hitedR.collider.gameObject.layer == 8 && moveX == 0)
            {
                //GetComponent<Rigidbody2D>().AddForce(Vector2.up * 1000);
                onslope = true;
            }
        }

    }
    void jump()
    {
        //note jump power was originally 20550

        //playerjumped bool is needed so jump physics work when you jump from a slope
        playerjumped = true;

        jumptimecounter = jumptime;

        //set vertical velocity to zero before applying jumping power so player can't jump extra high from slopes
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.GetComponent<Rigidbody2D>().velocity.x, 0);

        //jumpingcode
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * playerstartingjumppower);
    }
    void FlipPlayer()
    {
        facingright = !facingright;
        Vector2 localscale = gameObject.transform.localScale;
        localscale.x *= -1;
        transform.localScale = localscale;

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
