using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class player_move : MonoBehaviour
{
    // basic movements/actions
    public float playerspeed = 100;
    private bool facingright = true, readyforyellowcircle = false, readyforpinkcircle=false, keyused = false, door3locked=true, door7locked=true;
    private bool readyforbluetriangle = false, readyforredsquare=false, readyforyellowsquare=false, readyfororangesquare=false;
    private bool roomisfinishedrotating=true, redplaced = false, orangeplaced = false, yellowplaced = false, playerjumped=false, dialogueremaining=false, playerhasupgrade=false;
    public int playerjumppower = 1250;
    public float moveX, moveY, ladderX, menumoveX, menumoveY, accelerationtime, decelerationtime, accelerationrate, decelerationrate;
    public bool isGrounded, ladderaccess, onladder, readytodismount, readyfordoor, onslope, needtostop;
    public float timeuntildismount = 0.12f, sensitivity;
    private float leaveladdertimer = 0.0f, accelerationtimer=0.0f, decelerationtimer=0.0f, speedfactor, startingspeed, movespeedtimer, initialmovespeed;
    Vector2 newposition, slopenormal, playermovedirection, spawningpoint, externalforce=new Vector2 (0,0);
    private GameObject laddertop;
    public GameObject dialoguebox, door3spritelocked, door3spriteunlocked, door7spritelocked, door7spriteunlocked;
    public GameObject typewriterbox;
    private GameObject menuicon;
    public bool waitforcommand = false, readyforcursormovement = false, readytotalk=true; //readytotalk is used to prevent the player from accidentally re-initiating conversation with an NPC they just talked to
    public bool waitfortext = false, upgradeison=false;
    bool isPaused = false;
    GameObject[] pauseObjects;
    public string[] inventory = new string[8];
    public Vector2[] slotlocation = { new Vector2(-56, 32), new Vector2(-24, 32), new Vector2(8, 32), new Vector2(40, 32), new Vector2(-56, 0), new Vector2(-24, 0), new Vector2(8, 0), new Vector2(40, 0), new Vector2 (-400, 0) };
    // slot locations 0-7 correspond to inventory 0-7. Slot Location 8 is for cursor or items when not in use
    private string itemname;
    private List<int> cursorlocation = new List<int>();
    private int currentposition, j, dialoguepagenumber, strangegravityeffects=0, roomstate=1;

    // level item menu icons
    public GameObject inventorycursor;
    public GameObject yellowcircleicon, key2icon;
    public GameObject pinkcircleicon, orangesquareicon, yellowsquareicon, redsquareicon, bluetriangleicon;
    public GameObject key1, bluelocktoppiece, missingred, missingorange, missingyellow;

    // gravity rooms
    public GameObject gravityroom1;


    void Start()
    {
        readyfordoor = true;
        dialoguebox.gameObject.SetActive(false);
        typewriterbox.gameObject.SetActive(false);
        door3spritelocked.gameObject.SetActive(true);
        door3spriteunlocked.gameObject.SetActive(false);
        door7spritelocked.gameObject.SetActive(true);
        door7spriteunlocked.gameObject.SetActive(false);
        bluelocktoppiece.gameObject.SetActive(false);
        missingred.gameObject.SetActive(false);
        missingorange.gameObject.SetActive(false);
        missingyellow.gameObject.SetActive(false);
        initialmovespeed = playerspeed;

        pauseObjects = GameObject.FindGameObjectsWithTag("pause");
        hidePaused();

        StartCoroutine(initializeinventory());

        slopenormal = new Vector2(0, 1);

        //temporarily giving upgrade at launch for debugging. REMOVE THIS LINE LATER
        playerhasupgrade = true;
    }

    private void Awake()
    {
        GameObject check = GameObject.Find("APP");
        if (check == null)
        { UnityEngine.SceneManagement.SceneManager.LoadScene("preloadscene"); }

    }


    // Update is called once per frame
    void Update()
    {
        
        Debug.DrawLine(Vector3.zero, 32*playermovedirection, Color.red);

        if (Input.GetKeyDown(GameManager.GM.check) && waitfortext == true && readytotalk==true)
            {
            TypeWriterEffect Typescript = typewriterbox.GetComponent<TypeWriterEffect>();
            Debug.Log(Typescript.fullText);
            //Typescript.i = Typescript.fullText.Length;
            dialoguebox.gameObject.SetActive(true);
            dialoguebox.gameObject.GetComponent<Text>().text = Typescript.fullText;
            typewriterbox.gameObject.SetActive(false);
            Invoke("ready", 0.4f);
            Debug.Log("ready invoked");
        }

            if (waitforcommand != true && waitfortext != true && isPaused  == false)
        PlayerMove();

        GroundedUpdater();

        //turn off gravity while on slopes so player doesn't slide down
        if (onslope == true)
        {
            GetComponent<Rigidbody2D>().gravityScale = 0.0f;
        }
        if (onslope != true && onladder != true)
        {
            GetComponent<Rigidbody2D>().gravityScale = 1.0f;
        }

        //pause
        // added if waitforcaommnd != true, waitfortext !=true to try and remove issue  where player can pause during dialogue
        if (waitforcommand != true && waitfortext != true && Input.GetKeyDown(GameManager.GM.pause))
        {
            pausegame();
        }

        //toggle upgrade
        if (waitforcommand != true && waitfortext != true && isPaused!=true && playerhasupgrade==true && Input.GetKeyDown(GameManager.GM.upgrade))
        {
            toggleupgrade();
        }

        //cursor movement while paused
        if (isPaused)
        {
            cursormovement();
        }

            // this exits the text screen when you pick up an item or finish talking
            if (waitforcommand == true)
        {
            Waitingforinput();
        }

            // if you are in the middle of multipage dialgue, the first page will start in ontriggerstay, then it will resume here in the update section
        if (waitforcommand == true && Input.GetKeyDown(GameManager.GM.check) && dialoguepagenumber == 2)
        {
            Debug.Log("you got the upgrade");
            Time.timeScale = 1;
            dialoguebox.gameObject.SetActive(false);
            typewriterbox.gameObject.SetActive(false);
            playerhasupgrade = true;
            dialoguebox.gameObject.SetActive(true);
            dialoguebox.gameObject.GetComponent<Text>().text = "You got the upgrade.";
            //Time.timeScale = 0;
            dialoguepagenumber = 3;
            //waitforcommand = true; let's try having this false
            readytotalk = false;
            Invoke("talkcooldown", 0.4f);
        }

        if (waitforcommand == true && Input.GetKeyDown(GameManager.GM.check) && dialoguepagenumber == 3 && readytotalk==true)
        {
            Time.timeScale = 1;
            dialoguebox.gameObject.SetActive(false);
            typewriterbox.gameObject.SetActive(false);
            typewriterbox.gameObject.SetActive(true);
            TypeWriterEffect typeWriterEffect = typewriterbox.GetComponent<TypeWriterEffect>();
            typeWriterEffect.fullText = "This item will let you resist the strange effects found in certain rooms";
            waitfortext = true;
            waitforcommand = false;
            readytotalk = false;
            dialogueremaining = false;
            dialoguepagenumber = 0;
            Invoke("talkcooldown", 0.4f);
        }


    }

    //pause game
    void pausegame()
    {
        if (isPaused)
        {
            Time.timeScale = 1;
            isPaused = false;
            hidePaused();
            Debug.Log("inventory closed");
        }
        else
        {
            Debug.Log("inventory opened");
            Time.timeScale = 0;
            isPaused = true;
            showPaused();
            Debug.Log("cursor start at"+ currentposition);
        }
    }

    //toggle upgrade THIS SHOULD WORK FOR MOST DUNGEONS
    void toggleupgrade()
    {
        if (strangegravityeffects == 0)
        {
            if (upgradeison)
            {
                upgradeison = false;
                Debug.Log("upgrade turned off");

                //for color-coded dungeon or tractor beams, move player back to default layer
                gameObject.layer = 0;
            }
            else
            {
                upgradeison = true;
                Debug.Log("upgrade turned on");

                //for colorcoded dungeon, move player to new layer that interacts with intangible layer
                //in tractor beam dungeon, this is a layer that does NOT interact with tractor beams
                gameObject.layer = 10;

                //return player movespeed to original speed
                playerspeed = initialmovespeed;
            }
        }
        if (strangegravityeffects != 0)
        {
            if (roomisfinishedrotating == true)
                rotatetheroom();
        }
    }

    void rotatetheroom()
    {
        if (strangegravityeffects == 1 && roomstate==1) //rotate room 14
        {
            roomisfinishedrotating = false;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
            StartCoroutine(rotateroom1clockwise());
        }
        if (strangegravityeffects == 1 && roomstate == 2) //rotate room 14
        {
            roomisfinishedrotating = false;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
            StartCoroutine(rotateroom1counterclockwise());
        }
    }

    IEnumerator rotateroom1clockwise()
    {
        for (int i = 0; i <= 90; i++)
        {
            gravityroom1.transform.eulerAngles = new Vector3(0, 0, -i);
            yield return new WaitForSeconds(0.0004f);
        }
        roomstate = 2;
        roomisfinishedrotating = true;
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        yield break;
    }

    IEnumerator rotateroom1counterclockwise()
    {
        for (int i = 0; i <= 90; i++)
        {
            gravityroom1.transform.eulerAngles = new Vector3(0, 0, i-90);
            yield return new WaitForSeconds(0.0004f);
        }
        roomstate = 1;
        roomisfinishedrotating = true;
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        yield break;
    }

    void cursormovement()
    {
        if (Input.GetKeyDown(GameManager.GM.check))
        {
            Invoke(inventory[j], 0.0f);
        }

        //note: GetAxisRaw is not affected by timescale. That's why it's used in pause menu
        menumoveX = Input.GetAxisRaw(GameManager.GM.Horizontal);
        //these lines allow player to use keys or buttons if they prefer
        if (Input.GetKey(GameManager.GM.right))
        { menumoveX = 1;
            Debug.Log("menumoveX =" +menumoveX);
            Debug.Log("ready for cursor movement =" + readyforcursormovement);
        }

        if (Input.GetKey(GameManager.GM.left))
        {
            menumoveX = -1;
        }

        //up-down controls
        menumoveY = Input.GetAxisRaw(GameManager.GM.Vertical);

        if (Input.GetKey(GameManager.GM.up))
        { menumoveY = 1; }

        if (Input.GetKey(GameManager.GM.down))
        {
            menumoveY = -1;
        }

        if (menumoveX <= sensitivity && menumoveX >= -sensitivity && menumoveY <= sensitivity && menumoveY >= -sensitivity)
            readyforcursormovement = true;

        if(readyforcursormovement==false)
        {
            Debug.Log("menumoveX=" + menumoveX + " menumoveY=" + menumoveY);
        }

        if (menumoveX > sensitivity && readyforcursormovement == true)
        {
            Debug.Log("right key registered");
            currentposition = currentposition + 1;
            if (currentposition == cursorlocation.Count)
            {
                Debug.Log(currentposition);
                currentposition = 0; }
            Debug.Log("current position = "+ currentposition);            
            Debug.Log("list.count = " + cursorlocation.Count);
            j = cursorlocation[currentposition];
            Debug.Log("j = " + j);
            inventorycursor.transform.localPosition = slotlocation[j];
            readyforcursormovement = false;
        }
        if (menumoveX < -1 * sensitivity && readyforcursormovement == true)
        {
            Debug.Log("left key registered");
            currentposition = currentposition - 1;
            if (currentposition < 0)
            {
                Debug.Log(currentposition);
                currentposition = cursorlocation.Count - 1; }
            Debug.Log("current position is " + currentposition);
            j = cursorlocation[currentposition];
            Debug.Log("j = " + j);
            inventorycursor.transform.localPosition = slotlocation[j];
            readyforcursormovement = false;
        }

    }


    void Waitingforinput()
    {
        if (Input.GetKeyDown(GameManager.GM.check)  && dialogueremaining==false)
        {
            //Debug.Log("left");
            Time.timeScale = 1;
            dialoguebox.gameObject.SetActive(false);
            typewriterbox.gameObject.SetActive(false);
            waitforcommand = false;
            readytotalk = false;
            Invoke("talkcooldown", 0.4f);
        }
    }

    //shows objects with ShowOnPause tag
    public void showPaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(true);
        }

        StartCoroutine(findinventorycursor());
    }

    //hides objects with ShowOnPause tag
    public void hidePaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(false);
        }
    }

    void ready()
    {
        //Debug.Log("ready");
        Time.timeScale = 0;
        waitfortext = false;
        waitforcommand = true;
    }

    void talkcooldown()
    {
        readytotalk = true;
    }

    void PlayerMove()
    {
        //Debug.Log("needtostop=" + needtostop + " movespeed=" + speedfactor+" deceltimer="+decelerationtimer+" acceltimer="+accelerationtimer+" facingright="+facingright);

        //left-right controls
        //enable joystick:
        moveX = Input.GetAxis(GameManager.GM.Horizontal);
        
        // can only jump if grounded
        if (Input.GetKeyDown(GameManager.GM.jump) && isGrounded == true) { jump(); }

        // used to like idea of jumping off of ladder. now I don't.
        //if (Input.GetKeyDown(GameManager.GM.jump) && onladder == true && moveX !=0) { jump(); }

        //these lines allow player to use keys or buttons if they prefer
        if (Input.GetKey(GameManager.GM.right))
        { moveX = 1; }

        if (Input.GetKey(GameManager.GM.left))
        { moveX = -1;
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
        { moveX = 0;
            accelerationtimer = 0;
        }

        //moveX acceleration
        if(moveX>0 && gameObject.GetComponent<Rigidbody2D>().velocity.x>=0)
        {
            needtostop = false;
            decelerationtimer = 0;
            accelerationtimer += Time.deltaTime;
            if (accelerationtimer > accelerationtime)
            {
                speedfactor=1;
            }
            else
            {
                speedfactor = 0.5f + accelerationtimer / accelerationtime / 2;
            }
        }

        if (moveX < 0 && gameObject.GetComponent<Rigidbody2D>().velocity.x<=0)
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
        if(speedfactor!=0 && moveX==0 && needtostop!=true)
        {
            startingspeed = speedfactor;
            needtostop = true;
        }
       
        if(moveX>0 && gameObject.GetComponent<Rigidbody2D>().velocity.x < 0)
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
            if(onslope!=true || playerjumped==true)
                gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(speedfactor * playerspeed, gameObject.GetComponent<Rigidbody2D>().velocity.y) + externalforce;
        }
        
        //this code attempts to correct velocity path on slopes
        if (onladder != true && onslope==true && playerjumped==false)
        {
            playermovedirection = -Vector2.Perpendicular(slopenormal);
            gameObject.GetComponent<Rigidbody2D>().velocity = speedfactor * playerspeed * playermovedirection;
                }

        //add idle movement to wakeup physics engine for ontriggerstay
        if (onladder!=true && gameObject.GetComponent<Rigidbody2D>().velocity==new Vector2(0,0))
        {
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * .01f);
        }

        //ladder movement
        if (moveY != 0 && ladderaccess == true)
        {
            onladder = true;
        }

        //if (onladder == true &&  isGrounded!=true)
        if(onladder == true)
        {
            Debug.Log("on ladder, moveY ="+moveY+" moveX ="+moveX);

            //alloow vertical movement
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, moveY * playerspeed * 0.5f);

            //if you're on the ladder, snap xposition to center of ladder
            if (transform.position.x > ladderX || transform.position.x < ladderX)
            {
                newposition = new Vector2 (ladderX, transform.position.y);
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
        if(onladder == true && moveX == 0)
        {
            leaveladdertimer = 0;
        }

        if (onladder == false)
        {
            //if you're not climbing the ladder, gravity should act normally
            GetComponent<Rigidbody2D>().gravityScale = 1.0f;
            //if you're not climbing the ladder, you should be able to stand on the top of it
            if(laddertop!=null)
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
            //will have to add this line in color-coded intangible dungeon
            if (hited.collider.gameObject.layer == 9 && upgradeison==true)
            { 
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
            if (hitedL.collider.gameObject.tag == "laddertop" && onladder!=true)
            { //Change it to match ground tag
                isGrounded = true;
            }
            //if raycast detects a slope, add a vertical force doesn't fall down slope
            if (hitedL.collider.gameObject.layer == 8 && moveX == 0)
            {
                //GetComponent<Rigidbody2D>().AddForce(Vector2.up * 1000);
                onslope = true;
            }

            //will have to add this line in color-coded intangible dungeon
            if (hitedL.collider.gameObject.layer == 9 && upgradeison == true)
            {
                isGrounded = true;
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

            //will have to add this line in color-coded intangible dungeon
            if (hitedR.collider.gameObject.layer == 9 && upgradeison == true)
            {
                isGrounded = true;
            }
        }

    }

    void OnTriggerEnter2D(Collider2D trig)
    {
        //updates spawnpoint as you play
        if(trig.tag == "spawn")
        {
            spawningpoint = trig.transform.position;
        }

        //if player enters ladder's trig zone
        if (trig.tag == "ladder")
        {
            //store X-position of the ladder
            ladderX = trig.transform.position.x;
            //note that player has access to a ladder
            ladderaccess = true;
        }

        if (trig.name == "NPC1")
        {
            readyforyellowcircle = true;
            //Debug.Log("readyforyellowcircle = " + readyforyellowcircle);
        }

        if (trig.name == "BlueLock")
        {
            readyforbluetriangle = true;
        }

        if (trig.name == "RedLock")
        {
            readyforredsquare = true;
        }

        if (trig.name == "OrangeLock")
        {
            readyfororangesquare = true;
        }

        if (trig.name == "YellowLock")
        {
            readyforyellowsquare = true;
        }


        if (trig.name == "NPC2")
        {
            readyforpinkcircle = true;
        }

    }

    void OnTriggerStay2D(Collider2D trig)
    {
        if (trig.name == "Room2" && upgradeison==false)
        {
            playerspeed = initialmovespeed / 2;
        }

        if (trig.name == "Room3" && upgradeison == false)
        {
            movespeedtimer += Time.deltaTime;
            playerspeed = initialmovespeed * (Mathf.Sin(movespeedtimer*2.5f)+1);
        }

        if (trig.name == "Room14")
        {
            strangegravityeffects = 1;
        }

        if (trig.name == "tractorbeamthatpullsleft")
        {
            externalforce = new Vector2 (-100, 0);
        }

        //if player is in door's trig zone
        if (trig.name == "Door1")
        {
            if (moveY > 0 && readyfordoor == true)
            {
                GameObject Door2 = GameObject.Find("Door2");
                transform.position = Door2.transform.position;
                //set readyfordoor to false so player doesn't accidentally re-enter
                readyfordoor = false;
            }
        }

        //if player is in door's trig zone
        if (trig.name == "Door2")
        {
            if (moveY > 0 && readyfordoor == true)
            {
                GameObject Door1 = GameObject.Find("Door1");
                transform.position = Door1.transform.position;
                readyfordoor = false;
            }
        }

        // locked door mechanic
        if (trig.name == "Door3")
        {
            //if locked, check for key and unlock if a key is found and used
            if (moveY > 0 && readyfordoor == true && door3locked==true)
            {
                StartCoroutine(checkforkey());
                if (keyused == true)
                {
                    GameObject Door4 = GameObject.Find("Door4");
                    transform.position = Door4.transform.position;
                    //set readyfordoor to false so player doesn't accidentally re-enter
                    keyused = false;
                    door3locked = false;
                    readyfordoor = false;
                    door3spritelocked.gameObject.SetActive(false);
                    door3spriteunlocked.gameObject.SetActive(true);
                }
            }
            //if already unlocked, function like any other door
            if (moveY > 0 && readyfordoor == true && door3locked == false)
            {
                GameObject Door4 = GameObject.Find("Door4");
                transform.position = Door4.transform.position;
                readyfordoor = false;
            }
        }

        if (trig.name == "Door4")
        {
            if (moveY > 0 && readyfordoor == true)
            {
                GameObject Door3 = GameObject.Find("Door3");
                transform.position = Door3.transform.position;
                readyfordoor = false;
            }
        }

        //if player is in door's trig zone
        if (trig.name == "Door5")
        {
            if (moveY > 0 && readyfordoor == true)
            {
                GameObject Door6 = GameObject.Find("Door6");
                transform.position = Door6.transform.position;
                //set readyfordoor to false so player doesn't accidentally re-enter
                readyfordoor = false;
            }
        }

        //if player is in door's trig zone
        if (trig.name == "Door6")
        {
            if (moveY > 0 && readyfordoor == true)
            {
                GameObject Door5 = GameObject.Find("Door5");
                transform.position = Door5.transform.position;
                readyfordoor = false;
            }
        }

        // locked door mechanic
        if (trig.name == "Door7")
        {
            //if locked, check for key and unlock if a key is found and used
            if (moveY > 0 && readyfordoor == true && door7locked == true)
            {
                StartCoroutine(checkforkey());
                if (keyused == true)
                {
                    GameObject Door8 = GameObject.Find("Door8");
                    transform.position = Door8.transform.position;
                    //set readyfordoor to false so player doesn't accidentally re-enter
                    keyused = false;
                    door7locked = false;
                    readyfordoor = false;
                    door7spritelocked.gameObject.SetActive(false);
                    door7spriteunlocked.gameObject.SetActive(true);
                }
            }
            //if already unlocked, function like any other door
            if (moveY > 0 && readyfordoor == true && door7locked == false)
            {
                GameObject Door8 = GameObject.Find("Door8");
                transform.position = Door8.transform.position;
                readyfordoor = false;
            }
        }

        //if player is in door's trig zone
        if (trig.name == "Door8")
        {
            if (moveY > 0 && readyfordoor == true)
            {
                GameObject Door7 = GameObject.Find("Door7");
                transform.position = Door7.transform.position;
                readyfordoor = false;
            }
        }


        if (trig.name == "Door9")
        {
            if (moveY > 0 && readyfordoor == true)
            {
                GameObject Door10 = GameObject.Find("Door10");
                transform.position = Door10.transform.position;
                //set readyfordoor to false so player doesn't accidentally re-enter
                readyfordoor = false;
            }
        }

        //if player is in door's trig zone
        if (trig.name == "Door10")
        {
            if (moveY > 0 && readyfordoor == true)
            {
                GameObject Door9 = GameObject.Find("Door9");
                transform.position = Door9.transform.position;
                readyfordoor = false;
            }
        }



        //if player is in yellow ball trig zone
        if (trig.name == "YellowCircle")
        {
            if (Input.GetKeyUp(GameManager.GM.check))
            {
                itemname = trig.name;
                menuicon = yellowcircleicon;
                Destroy(trig.gameObject);
                StartCoroutine(addtoinventory());
                dialoguebox.gameObject.SetActive(true);
                dialoguebox.gameObject.GetComponent<Text>().text = "You got the yellow ball.";
                Time.timeScale = 0;
                waitforcommand = true;
            }
        }

        //if player is in yellow square trig zone
        if (trig.name == "YellowSquare")
        {
            if (Input.GetKeyUp(GameManager.GM.check))
            {
                itemname = trig.name;
                menuicon = yellowsquareicon;
                Destroy(trig.gameObject);
                StartCoroutine(addtoinventory());
                dialoguebox.gameObject.SetActive(true);
                dialoguebox.gameObject.GetComponent<Text>().text = "You got the yellow square.";
                Time.timeScale = 0;
                waitforcommand = true;
            }
        }

        //if player is in item trig zone
        if (trig.name == "RedSquare")
        {
            if (Input.GetKeyUp(GameManager.GM.check))
            {
                itemname = trig.name;
                menuicon = redsquareicon;
                Destroy(trig.gameObject);
                StartCoroutine(addtoinventory());
                dialoguebox.gameObject.SetActive(true);
                dialoguebox.gameObject.GetComponent<Text>().text = "You got the red square.";
                Time.timeScale = 0;
                waitforcommand = true;
            }
        }

        //if player is in item trig zone
        if (trig.name == "BlueTriangle")
        {
            if (Input.GetKeyUp(GameManager.GM.check))
            {
                itemname = trig.name;
                menuicon = bluetriangleicon;
                Destroy(trig.gameObject);
                StartCoroutine(addtoinventory());
                dialoguebox.gameObject.SetActive(true);
                dialoguebox.gameObject.GetComponent<Text>().text = "You got the blue triangle.";
                Time.timeScale = 0;
                waitforcommand = true;
            }
        }

        //if player is in pink ball trig zone
        if (trig.name == "PinkCircle")
        {
            if (Input.GetKeyUp(GameManager.GM.check))
            {
                itemname = trig.name;
                menuicon = pinkcircleicon;
                Destroy(trig.gameObject);
                StartCoroutine(addtoinventory());
                dialoguebox.gameObject.SetActive(true);
                dialoguebox.gameObject.GetComponent<Text>().text = "You got the pink ball.";
                Time.timeScale = 0;
                waitforcommand = true;
            }
        }

        //if player is in item trig zone
        if (trig.name == "key")
        {
            if (Input.GetKeyUp(GameManager.GM.check))
            {
                itemname = trig.name;
                menuicon = key2icon;
                Destroy(trig.gameObject);
                StartCoroutine(addtoinventory());
                dialoguebox.gameObject.SetActive(true);
                dialoguebox.gameObject.GetComponent<Text>().text = "You got a key.";
                Time.timeScale = 0;
                waitforcommand = true;
            }
        }

        //if player is in NPC1 trig zone
        if (trig.name == "NPC1")
        {
            if (Input.GetKeyDown(GameManager.GM.check) && waitforcommand == false && waitfortext == false && isPaused == false && readytotalk==true)
            {
                readytotalk = false;
                //Debug.Log("start talking");
                typewriterbox.gameObject.SetActive(true);
                    TypeWriterEffect typeWriterEffect = typewriterbox.GetComponent<TypeWriterEffect>();
                    typeWriterEffect.fullText = "Bring me the yellow ball and I'll give you a key";
                    waitfortext = true;
                //Debug.Log("wait for text =" + waitfortext);
                Invoke("talkcooldown", 0.2f);
            }
        }

        if (trig.name == "NPC2")
        {
            if (Input.GetKeyDown(GameManager.GM.check) && waitforcommand == false && waitfortext == false && isPaused == false && readytotalk == true)
            {
                readytotalk = false;
                //Debug.Log("start talking");
                typewriterbox.gameObject.SetActive(true);
                TypeWriterEffect typeWriterEffect = typewriterbox.GetComponent<TypeWriterEffect>();
                typeWriterEffect.fullText = "Bring me the pink ball and I'll give you a key";
                waitfortext = true;
                //Debug.Log("wait for text =" + waitfortext);
                Invoke("talkcooldown", 0.2f);
            }
        }

        if (trig.name == "personthatprovidesupgrade" && playerhasupgrade==false)
        {
            if (Input.GetKeyDown(GameManager.GM.check) && waitforcommand == false && waitfortext == false && isPaused == false && readytotalk == true)
            {
                readytotalk = false;
                dialoguepagenumber = 2;
                //dialogue remaining is used to keep player there through multiple pages of dialogue.
                dialogueremaining = true;
                //Debug.Log("start talking");
                typewriterbox.gameObject.SetActive(true);
                TypeWriterEffect typeWriterEffect = typewriterbox.GetComponent<TypeWriterEffect>();
                typeWriterEffect.fullText = "You will need this to conquer the dungeon";
                waitfortext = true;
                //Debug.Log("wait for text =" + waitfortext);
                Invoke("talkcooldown", 0.2f);
            }
    }
        if (trig.name == "personthatprovidesupgrade" && playerhasupgrade == true)
        {
            if (Input.GetKeyDown(GameManager.GM.check) && waitforcommand == false && waitfortext == false && isPaused == false && readytotalk == true)
            {
                readytotalk = false;
                //Debug.Log("start talking");
                typewriterbox.gameObject.SetActive(true);
                TypeWriterEffect typeWriterEffect = typewriterbox.GetComponent<TypeWriterEffect>();
                typeWriterEffect.fullText = "This item will let you resist the strange effects found in certain rooms";
                waitfortext = true;
                //Debug.Log("wait for text =" + waitfortext);
                Invoke("talkcooldown", 0.2f);
            }
        }
    }

    void OnTriggerExit2D(Collider2D trig)
    {
        if (trig.name == "Room2")
        {
            playerspeed = initialmovespeed;
        }

        if (trig.name == "Room14")
        {
            strangegravityeffects = 0;
        }

        if (trig.name == "tractorbeamthatpullsleft")
        {
            externalforce = new Vector2(0, 0);
        }

        //if player exits ladder's trig zone
        if (trig.tag == "ladder")
        {
            ladderaccess = false;
            onladder = false;

            //Debug.Log("exit ladder");
        }

        if (trig.name == "NPC1")
        {
            readyforyellowcircle = false;
            //Debug.Log("readyforyellowcircle = " + readyforyellowcircle);
        }

        if (trig.name == "BlueLock")
        {
            readyforbluetriangle = false;
        }

        if (trig.name == "NPC2")
        {
            readyforpinkcircle = false;
        }

        if (trig.name == "RedLock")
        {
            readyforredsquare = false;
        }

        if (trig.name == "OrangeLock")
        {
            readyfororangesquare = false;
        }

        if (trig.name == "YellowLock")
        {
            readyforyellowsquare = false;
        }

    }

    IEnumerator addtoinventory()
    {
        for (int i = 0; i <= 7; i++)
        {
            if (inventory[i] == "empty")
            {
                inventory[i] = itemname;
                menuicon.transform.localPosition = slotlocation[i];
                Debug.Log(itemname+"stored in slot "+i);
                yield break;
            }

            Debug.Log(i);
        }
        yield break;
    }


    IEnumerator findinventorycursor()
    {
        currentposition = 0;
        // clear old cursor location list
        cursorlocation.Clear();
        // this loop adds all item locations to possible cursor locations 
        for (int i = 0; i <= 7; i++)
        {
            if (inventory[i] != "empty")
            {
                cursorlocation.Add(i);
            }
        }
        Debug.Log(cursorlocation);

        // this loop finds lowest used item slot, then places the cursor there and ends coroutine
        for (int i = 0; i <= 7; i++)
        {
            if (inventory[i] != "empty")
            {
                inventorycursor.transform.localPosition = slotlocation[i];
                Debug.Log("cursor at slot " + i + " " + slotlocation[i]);
                j = i;
                yield break;
            }

            Debug.Log(i);
        }
        //if no items found, place cursor offscreen and end coroutine
        inventorycursor.transform.localPosition = slotlocation[8];
        yield break;

    }

    IEnumerator initializeinventory()
    {
        Debug.Log("initialize inventory");
        for (int i = 0; i <= 7; i++)
        {
                inventory[i] = "empty";
                Debug.Log(inventory[i] + " stored in slot " + i);
         
        }

        yield break;
    }

    IEnumerator checkforkey()
    {
        Debug.Log("checking for key");
        for (int i = 0; i <= 7; i++)
        {
            if (inventory[i] == "key")
            {
                keyused = true;
                inventory[i] = "empty";
                Debug.Log("inventory " + i + " set to " + inventory[i]);
                Vector3 position = slotlocation[i];
                Debug.Log("will check for an item at position " + position);
                //GameObject myObject = null;
                foreach (GameObject go in pauseObjects)
                {
                    Debug.Log("checking for pause objects in slot " + i);
                    Debug.Log("item found at " + go.transform.localPosition);
                    if (go.transform.localPosition == position && go.name != "pausecursor")
                    {
                        Debug.Log(go.name);
                        //myObject = go;
                        //string name = myObject.name;
                        Debug.Log("item found in designated position");
                        //following line removes the key icon by hiding it offscreen but it doesn't seem to be working
                        go.transform.localPosition = slotlocation[8];
                        //gonna try deleting the icon instead
                        //pauseObjects.Remove(go);
                        //Destroy(myObject);
                        Debug.Log("transform set to " + go.transform.localPosition);
                        break;
                    }
                }

                yield break;
            }
        }
        yield break;
    }

    void OnCollisionStay2D(Collision2D col)
    {
        //allow the player to pass through top of ladder while climbing
        if (col.gameObject.tag == "laddertop" && onladder == true)
        {
            //store laddertop item so you can reference it later
            laddertop = col.gameObject;
            //Debug.Log("collide with top and on ladder");
            //turn off collisions between player and top of ladder
            Physics2D.IgnoreCollision(col.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Hazard")
        {
            GameManager.GM.lives = GameManager.GM.lives - 1;
            if(GameManager.GM.lives==0)
            { SceneManager.LoadScene("funhousetitlescreen"); }
            transform.position = spawningpoint;
        }
        //if player collides with an object on slope layer, find the normal to the slope
        if (collision.gameObject.layer == 8)
        {
            foreach (ContactPoint2D cp2d in collision.contacts)
            {
                slopenormal = cp2d.normal;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //if player when player leaves collision with slope, reset ground normal
        if (collision.gameObject.layer == 8)
        {
            slopenormal = new Vector2(0, 1);
        }
    }


    void jump()
    {
        //playerjumped bool is needed so jump physics work when you jump from a slope
        playerjumped = true;
        //set vertical velocity to zero before applying jumping power so player can't jump extra high from slopes
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.GetComponent<Rigidbody2D>().velocity.x, 0);

        //jumpingcode
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * playerjumppower);
        Invoke("playerjumpedfalse", 0.2f);
    }

    void playerjumpedfalse()
    {
        playerjumped = false;
    }

    void FlipPlayer()
    {
        facingright = !facingright;
        Vector2 localscale = gameObject.transform.localScale;
        localscale.x *= -1;
        transform.localScale = localscale;

    }
    void YellowCircle()
    {
        Debug.Log("ran void YellowCircle");
        if (readyforyellowcircle == true)
        {
            // run pause script to leave inventory menu
            pausegame();

            //remove yellow circle from inventory
            inventory[j] = "empty";
            yellowcircleicon.transform.localPosition = slotlocation[8];

            //add key to inventory
            itemname = "key";
            menuicon = key1;
            StartCoroutine(addtoinventory());

            // do all the talking stuff
            typewriterbox.gameObject.SetActive(true);
            TypeWriterEffect typeWriterEffect = typewriterbox.GetComponent<TypeWriterEffect>();
            typeWriterEffect.fullText = "Thanks! I saw a pink one somewhere but yellow is my favorite color. Here is your key";
            waitfortext = true;      
        }

    }

    void PinkCircle()
    {
        Debug.Log("ran void PinkCircle");
        if (readyforpinkcircle == true)
        {
            // run pause script to leave inventory menu
            pausegame();

            //remove yellow circle from inventory
            inventory[j] = "empty";
            pinkcircleicon.transform.localPosition = slotlocation[8];

            //add key to inventory
            itemname = "OrangeSquare";
            menuicon = orangesquareicon;
            StartCoroutine(addtoinventory());

            // do all the talking stuff
            typewriterbox.gameObject.SetActive(true);
            TypeWriterEffect typeWriterEffect = typewriterbox.GetComponent<TypeWriterEffect>();
            typeWriterEffect.fullText = "Thanks! This is all I have to offer you, I hope that's OK. Got Orange Square.";
            waitfortext = true;
        }
    }


    void BlueTriangle()
    {
        if (readyforbluetriangle == true)
        {
            // run pause script to leave inventory menu
            pausegame();

            //remove part from inventory
            inventory[j] = "empty";
            bluetriangleicon.transform.localPosition = slotlocation[8];

            //put triangle in place
            bluelocktoppiece.gameObject.SetActive(true);

            //open hole in wall
            GameObject wall = GameObject.Find("DestructibleWall");
            Destroy(wall);
        }
    }
    void RedSquare()
    {
        if (readyforredsquare == true)
        {
            // run pause script to leave inventory menu
            pausegame();

            //remove part from inventory
            inventory[j] = "empty";
            redsquareicon.transform.localPosition = slotlocation[8];

            //put triangle in place
            missingred.gameObject.SetActive(true);
            redplaced = true;

            //check if all squares placed and spawn key
            if (orangeplaced == true && redplaced == true && yellowplaced == true)
            {
                GameObject Key = GameObject.Find("key");
                Key.transform.position = new Vector3(1384, -80, 0);
            }
        }
    }

    void OrangeSquare()
    {
        if (readyfororangesquare == true)
        {
            // run pause script to leave inventory menu
            pausegame();

            //remove part from inventory
            inventory[j] = "empty";
            orangesquareicon.transform.localPosition = slotlocation[8];

            //put triangle in place
            missingorange.gameObject.SetActive(true);
            orangeplaced = true;

            //check if all squares placed and spawn key
            if(orangeplaced==true && redplaced==true && yellowplaced == true)
            {
                GameObject Key = GameObject.Find("key");
                Key.transform.position = new Vector3 (1384, -80, 0);
            }
        }
    }

    void YellowSquare()
    {
        if (readyforyellowsquare == true)
        {
            // run pause script to leave inventory menu
            pausegame();

            //remove part from inventory
            inventory[j] = "empty";
            yellowsquareicon.transform.localPosition = slotlocation[8];

            //put triangle in place
            missingyellow.gameObject.SetActive(true);
            yellowplaced = true;

            //check if all squares placed and spawn key
            if (orangeplaced == true && redplaced == true && yellowplaced == true)
            {
                GameObject Key = GameObject.Find("key");
                Key.transform.position = new Vector3(1384, -80, 0);
            }
        }
    }

}
