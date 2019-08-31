using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class player_move : MonoBehaviour
{
    // basic movements/actions
    public int playerspeed = 100;
    private bool facingright = true, readyforyellowcircle = false, readyforpinkcircle=false, keyused = false, door3locked=true, door7locked=true;
    private bool readyforbluetriangle = false, readyforredsquare=false, readyforyellowsquare=false, readyfororangesquare=false;
    private bool redplaced = false, orangeplaced = false, yellowplaced = false;
    public int playerjumppower = 1250;
    public float moveX, moveY, ladderX;
    public bool isGrounded, ladderaccess, onladder, readytodismount, readyfordoor;
    public float timeuntildismount = 0.12f;
    private float leaveladdertimer = 0.0f;
    Vector2 newposition;
    private GameObject laddertop;
    public GameObject dialoguebox, door3spritelocked, door3spriteunlocked, door7spritelocked, door7spriteunlocked;
    public GameObject typewriterbox;
    private GameObject menuicon;
    public bool waitforcommand = false, readyforcursormovement = false;
    public bool waitfortext = false;
    bool isPaused = false;
    GameObject[] pauseObjects;
    public string[] inventory = new string[8];
    public Vector2[] slotlocation = { new Vector2(-56, 32), new Vector2(-24, 32), new Vector2(8, 32), new Vector2(40, 32), new Vector2(-56, 0), new Vector2(-24, 0), new Vector2(8, 0), new Vector2(40, 0), new Vector2 (-400, 0) };
    // slot locations 0-7 correspond to inventory 0-7. Slot Location 8 is for cursor or items when not in use
    private string itemname;
    private List<int> cursorlocation = new List<int>();
    private int currentposition, j;

    // level item menu icons
    public GameObject inventorycursor;
    public GameObject yellowcircleicon, key2icon;
    public GameObject pinkcircleicon, orangesquareicon, yellowsquareicon, redsquareicon, bluetriangleicon;
    public GameObject key1, bluelocktoppiece, missingred, missingorange, missingyellow;



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

        pauseObjects = GameObject.FindGameObjectsWithTag("pause");
        hidePaused();

        StartCoroutine(initializeinventory());
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
        Debug.Log(moveY);

        if (Input.GetKeyDown(GameManager.GM.check) && waitfortext == true)
            {
            TypeWriterEffect Typescript = typewriterbox.GetComponent<TypeWriterEffect>();
            Debug.Log(Typescript.fullText);
            //Typescript.i = Typescript.fullText.Length;
            dialoguebox.gameObject.SetActive(true);
            dialoguebox.gameObject.GetComponent<Text>().text = Typescript.fullText;
            typewriterbox.gameObject.SetActive(false);
            Invoke("ready", 0.5f);
            Debug.Log("ready invoked");
        }

            if (waitforcommand != true && waitfortext != true && isPaused  == false)
        PlayerMove();

        GroundedUpdater();

        //pause
        if (Input.GetKeyDown(GameManager.GM.pause))
        {
            pausegame();
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
        }
    }

    void cursormovement()
    {
        if (Input.GetKeyDown(GameManager.GM.check))
        {
            Invoke(inventory[j], 0.0f);
        }


        moveX = Input.GetAxis(GameManager.GM.Horizontal);
        //these lines allow player to use keys or buttons if they prefer
        if (Input.GetKey(GameManager.GM.right))
        { moveX = 1;
            Debug.Log("moveX =" +moveX);
            Debug.Log("ready for cursor movement =" + readyforcursormovement);
        }

        if (Input.GetKey(GameManager.GM.left))
        {
            moveX = -1;
        }

        //up-down controls
        moveY = Input.GetAxis(GameManager.GM.Vertical);

        if (Input.GetKey(GameManager.GM.up))
        { moveY = 1; }

        if (Input.GetKey(GameManager.GM.down))
        {
            moveY = -1;
        }

        if (moveX == 0 && moveY == 0)
            readyforcursormovement = true;

        if (moveX > 0.2 && readyforcursormovement == true)
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
        if (moveX < -0.2 && readyforcursormovement == true)
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
        if (Input.GetKeyDown(GameManager.GM.check))
        {
            //Debug.Log("left");
            Time.timeScale = 1;
            dialoguebox.gameObject.SetActive(false);
            typewriterbox.gameObject.SetActive(false);
            waitforcommand = false;
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

    void PlayerMove()
    {

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

        //up-down controls
        moveY = Input.GetAxis(GameManager.GM.Vertical);

        if (Input.GetKey(GameManager.GM.up))
        { moveY = 1; }

        if (Input.GetKey(GameManager.GM.down))
        {
            moveY = -1;
        }

        //if you have just used a door,
        //Y-axis has to be reset to zero before you can enter a door again
        if (moveY == 0)
            readyfordoor = true;


        //animations
        //player direction
        if (moveX < 0.0f && facingright == false)
        {
            FlipPlayer();
        }
        else if (moveX > 0.0f && facingright == true)
        {
            FlipPlayer();
        }
        //left-right movement
        if(onladder!=true)
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(moveX * playerspeed, gameObject.GetComponent<Rigidbody2D>().velocity.y);

        //ladder movement
        if(moveY != 0 && ladderaccess == true)
        {
            onladder = true;
            //set horizontal velocity to zero and allow vertical movement
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, moveY * playerspeed * 0.5f);
        }

        if (onladder == true &&  isGrounded!=true)
        {
            //if you're on the ladder, snap xposition to center of ladder
            if(transform.position.x > ladderX || transform.position.x < ladderX)
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
        }
    }

    void OnTriggerEnter2D(Collider2D trig)
    {


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
            if (Input.GetKeyUp(GameManager.GM.check) && waitforcommand == false && waitfortext == false && isPaused == false)
            {
                //Debug.Log("start talking");
                    typewriterbox.gameObject.SetActive(true);
                    TypeWriterEffect typeWriterEffect = typewriterbox.GetComponent<TypeWriterEffect>();
                    typeWriterEffect.fullText = "Bring me the yellow ball and I'll give you a key";
                    waitfortext = true;
                //Debug.Log("wait for text =" + waitfortext);
            }
        }

        if (trig.name == "NPC2")
        {
            if (Input.GetKeyUp(GameManager.GM.check) && waitforcommand == false && waitfortext == false && isPaused == false)
            {
                //Debug.Log("start talking");
                typewriterbox.gameObject.SetActive(true);
                TypeWriterEffect typeWriterEffect = typewriterbox.GetComponent<TypeWriterEffect>();
                typeWriterEffect.fullText = "Bring me the pink ball and I'll give you a key";
                waitfortext = true;
                //Debug.Log("wait for text =" + waitfortext);
            }
        }
    }

    void OnTriggerExit2D(Collider2D trig)
    {


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
        if (col.gameObject.tag == "laddertop" && onladder == false)
        {
            //Debug.Log("collide with top and off ladder");
            
        }
    }


    void jump()
    {
        //jumpingcode
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * playerjumppower);
        //isGrounded = false;
        onladder = false;

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
                GameObject Key = GameObject.Find("Key");
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
