using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class titlescript : MonoBehaviour
{
    public static float highScore;
    public GameObject options1, options, square;
    private float position1, position2, currentposition;
    private bool optionsmenu = false;
    bool readyForKey = false;
    bool Lassigned = false;
    bool Rassigned = false;
    bool Uassigned = false;
    bool Dassigned = false;
    bool Shieldassigned = false; //shield is jump and I was too lazy to change it when I copied the Monkey Biz script
    bool Checkassigned = false;
    bool Pauseassigned = false;
    bool Upgradeassigned = false;
    AudioSource audSource;
    //public AudioClip titletheme;
    private int Q = 1; //this is to keep track of what keybind question we're on
    private GameObject optionmenu;
    private int[] values;
    private bool[] keys;

    private void Awake()
    {
        GameObject check = GameObject.Find("APP");
        if (check == null)
        { UnityEngine.SceneManagement.SceneManager.LoadScene("preloadscene"); }


        values = (int[])System.Enum.GetValues(typeof(KeyCode));
        keys = new bool[values.Length];
    }

    // Start is called before the first frame update
    void Start()
    {
        audSource = GetComponent<AudioSource>();
        highScore = PlayerPrefs.GetFloat("High Score");
        GameObject NESCursor = GameObject.Find("cursor");
        options1.gameObject.SetActive(false);
        options.gameObject.SetActive(false);
        position1 = -44.5f;
        position2 = -60.5f;
        currentposition = position1;
        //audSource.clip = titletheme;
        audSource.Play();

        //Set Mouse Cursor to not be visible
        Cursor.visible = false;

        // At start, check if any axes are at -1 and if they are, turn them off from the button mapping using some kinda bool.
        // PS 4 Right stick Vertical is inverted. Other controllers may default to having inverted axes as well.

    }

    // Update is called once per frame
    void Update()
    {
        // how to exit game
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }


        DetectPressedKeyOrButton();

        GameObject NESCursor = GameObject.Find("cursor");
        NESCursor.transform.position = new Vector2(-44, currentposition);

        //Debug.Log(GameManager.GM.Horizontal);
        float xVel = Input.GetAxisRaw(GameManager.GM.Horizontal);
        float yVel = Input.GetAxisRaw(GameManager.GM.Vertical);

        //float xVel = Input.GetAxisRaw("Horizontal");
        //float yVel = Input.GetAxisRaw("Vertical");


        if (Input.GetKey(GameManager.GM.up))
        { yVel = 1; }

        if (Input.GetKey(GameManager.GM.down))
        { yVel = -1; }

        if (!Input.GetKey(GameManager.GM.up) && !Input.GetKey(GameManager.GM.down) && Input.GetAxisRaw(GameManager.GM.Vertical) == 0)
        { yVel = 0; }


        if (Input.GetKeyDown(GameManager.GM.pause) && currentposition == position1)
        {
            SceneManager.LoadScene("stage1");
        }

        if (optionsmenu == false)
        {
            if (yVel > 0.2 && currentposition != position1)
            {
                currentposition = position1;
            }

            if (yVel < -0.2 && currentposition != position2)
            {
                currentposition = position2;
            }
        }


        // go to options screen
        if (Input.GetKeyDown(GameManager.GM.pause) && currentposition == position2)
        {
            options1.gameObject.SetActive(true);
            options.gameObject.SetActive(true);
            GameObject optionmenu = Instantiate(square);
            options.gameObject.GetComponent<Text>().text = "PRESS LEFT BUTTON";
            optionsmenu = true;
            readyForKey = false;
            Lassigned = false;
            Rassigned = false;
            Uassigned = false;
            Dassigned = false;
            Shieldassigned = false;
            Checkassigned = false;
            Pauseassigned = false;
            Upgradeassigned = false;
        }

        //  
        if (Input.GetKeyUp(GameManager.GM.pause) && optionsmenu == true)
        {
            readyForKey = true;
        }

        //  Options menu
        if (optionsmenu == true && Q == 1 && readyForKey == true)
        {
            DetectLeftButton();
        }

        if (!Input.anyKey && Lassigned == true && Q < 2)
        {
            Q = 2;
        }

        if (optionsmenu == true && Q == 2)
        {
            DetectRightButton();
        }

        if (!Input.anyKey && Rassigned == true && Q < 3)
        {
            Q = 3;
        }

        if (optionsmenu == true && Q == 3)
        {
            DetectDownButton();
        }

        if (!Input.anyKey && Dassigned == true && Q < 4)
        {
            Q = 4;
        }

        if (optionsmenu == true && Q == 4)
        {
            DetectUpButton();
        }

        if (!Input.anyKey && Uassigned == true && Q < 5)
        {
            Q = 5;
        }

        if (optionsmenu == true && Q == 5)
        {
            DetectShieldButton();
        }

        if (!Input.anyKey && Shieldassigned == true && Q < 6)
        {
            Q = 6;
        }

        if (optionsmenu == true && Q == 6)
        {
            DetectCheckButton();
        }

        if (!Input.anyKey && Checkassigned == true && Q < 7)
        {
            Q = 7;
        }

        if (optionsmenu == true && Q == 7)
        {
            DetectUpgradeButton();
        }

        if (!Input.anyKey && Upgradeassigned == true && Q < 8)
        {
            Q = 8;
        }

        if (!Input.anyKey && Pauseassigned == true && Q == 8)
        {
            Q = 1;
        }

        if (optionsmenu == true && Q == 8)
        {
            DetectStartButton();
        }

    }



    //Display keycode in debuglog
    public void DetectPressedKeyOrButton()
    {
        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
            {
                Debug.Log("KeyCode down: " + kcode);
                Debug.Log(Q);
                Debug.Log(readyForKey);
                Debug.Log("options menu" + optionsmenu);
            }
        }
    }
    //end test code


    // LEFT MOVEMENT INPUT
    public void DetectLeftButton()
    {
        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
        {

            if (Input.GetKeyDown(kcode))
            {
                GameManager.GM.left = kcode;
                //PlayerPrefs.SetString("leftKey", GameManager.GM.left.ToString());
                readyForKey = false;
                Debug.Log("Left Assigned to" + kcode);
                options.gameObject.GetComponent<Text>().text = "PRESS RIGHT BUTTON";
                Lassigned = true;
            }

        }

        if (Input.GetAxis("Horizontal") < -0.8)
        {
            GameManager.GM.Horizontal = "Horizontal";
            readyForKey = false;
            Debug.Log("Left Assigned to X Axis");
            options.gameObject.GetComponent<Text>().text = "PRESS RIGHT BUTTON";
            Lassigned = true;
        }

        if (Input.GetAxis("Axis3") < -0.8)
        {
            GameManager.GM.Horizontal = "Axis3";
            readyForKey = false;
            Debug.Log("Left Assigned to Axis3");
            options.gameObject.GetComponent<Text>().text = "PRESS RIGHT BUTTON";
            Lassigned = true;
        }
        if (Input.GetAxis("Axis4") < -0.5 && Input.GetAxis("Axis4") > -1.0)
        {
            GameManager.GM.Horizontal = "Axis4";
            readyForKey = false;
            Debug.Log("Left Assigned to Axis4");
            options.gameObject.GetComponent<Text>().text = "PRESS RIGHT BUTTON";
            Lassigned = true;
        }
        if (Input.GetAxis("Axis5") < -0.5 && Input.GetAxis("Axis5") > -1.0)
        {
            GameManager.GM.Horizontal = "Axis5";
            readyForKey = false;
            Debug.Log("Left Assigned to Axis5");
            options.gameObject.GetComponent<Text>().text = "PRESS RIGHT BUTTON";
            Lassigned = true;
        }
        if (Input.GetAxis("Axis6") < -0.5 && Input.GetAxis("Axis6") > -1.0)
        {
            GameManager.GM.Horizontal = "Axis6";
            readyForKey = false;
            Debug.Log("Left Assigned to Axis6");
            options.gameObject.GetComponent<Text>().text = "PRESS RIGHT BUTTON";
            Lassigned = true;
        }


        if (Input.GetAxis("Axis7") < -0.8)
        {
            GameManager.GM.Horizontal = "Axis7";
            readyForKey = false;
            Debug.Log("Left Assigned to Axis7");
            options.gameObject.GetComponent<Text>().text = "PRESS RIGHT BUTTON";
            Lassigned = true;
        }

        if (Input.GetAxis("Axis8") < -0.8)
        {
            GameManager.GM.Horizontal = "Axis8";
            readyForKey = false;
            Debug.Log("Left Assigned to Axis8");
            options.gameObject.GetComponent<Text>().text = "PRESS RIGHT BUTTON";
            Lassigned = true;
        }

    }


    // RIGHT MOVEMENT
    public void DetectRightButton()
    {
        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
            {
                GameManager.GM.right = kcode;
                //PlayerPrefs.SetString("rightKey", GameManager.GM.right.ToString());
                Rassigned = true;
                Debug.Log("Right Assigned to" + kcode);
                options.gameObject.GetComponent<Text>().text = "PRESS DOWN BUTTON";
            }
        }

        // JOYSTICK MOVEMENT
        if (Input.GetAxis("Horizontal") > 0.8)
        {
            GameManager.GM.Horizontal = "Horizontal";
            readyForKey = false;
            Debug.Log("Right Assigned to X Axis");
            options.gameObject.GetComponent<Text>().text = "PRESS DOWN BUTTON";
            Rassigned = true;
        }

        if (Input.GetAxis("Axis3") > 0.8)
        {
            GameManager.GM.Horizontal = "Axis3";
            readyForKey = false;
            Debug.Log("Right Assigned to Axis3");
            options.gameObject.GetComponent<Text>().text = "PRESS DOWN BUTTON";
            Rassigned = true;
        }
        if (Input.GetAxis("Axis4") > 0.8)
        {
            GameManager.GM.Horizontal = "Axis4";
            readyForKey = false;
            Debug.Log("Right Assigned to Axis4");
            options.gameObject.GetComponent<Text>().text = "PRESS DOWN BUTTON";
            Rassigned = true;
        }
        if (Input.GetAxis("Axis5") > 0.8)
        {
            GameManager.GM.Horizontal = "Axis5";
            readyForKey = false;
            Debug.Log("Right Assigned to Axis5");
            options.gameObject.GetComponent<Text>().text = "PRESS DOWN BUTTON";
            Rassigned = true;
        }
        if (Input.GetAxis("Axis6") > 0.8)
        {
            GameManager.GM.Horizontal = "Axis6";
            readyForKey = false;
            Debug.Log("Right Assigned to Axis6");
            options.gameObject.GetComponent<Text>().text = "PRESS DOWN BUTTON";
            Rassigned = true;
        }


        if (Input.GetAxis("Axis7") > 0.8)
        {
            GameManager.GM.Horizontal = "Axis7";
            readyForKey = false;
            Debug.Log("Right Assigned to Axis7");
            options.gameObject.GetComponent<Text>().text = "PRESS DOWN BUTTON";
            Rassigned = true;
        }

        if (Input.GetAxis("Axis8") > 0.8)
        {
            GameManager.GM.Horizontal = "Axis8";
            readyForKey = false;
            Debug.Log("Right Assigned to Axis8");
            options.gameObject.GetComponent<Text>().text = "PRESS DOWN BUTTON";
            Rassigned = true;
        }
    }

    // UP MOVEMENT
    public void DetectUpButton()
    {
        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
            {
                GameManager.GM.up = kcode;
                //PlayerPrefs.SetString("upKey", GameManager.GM.up.ToString());
                Uassigned = true;
                Debug.Log("Up Assigned to" + kcode);
                options.gameObject.GetComponent<Text>().text = "PRESS JUMP BUTTON";
            }
        }

        //UP JOYSTICK OPTIONS
        if (Input.GetAxis("Vertical") > 0.8)
        {
            GameManager.GM.Vertical = "Vertical";
            readyForKey = false;
            Debug.Log("Up Assigned to Y Axis");
            options.gameObject.GetComponent<Text>().text = "PRESS JUMP BUTTON";
            Uassigned = true;
        }

        if (Input.GetAxis("Axis3") > 0.8)
        {
            GameManager.GM.Vertical = "Axis3";
            readyForKey = false;
            Debug.Log("Up Assigned to Axis3");
            options.gameObject.GetComponent<Text>().text = "PRESS JUMP BUTTON";
            Uassigned = true;
        }
        if (Input.GetAxis("Axis4") > 0.8)
        {
            GameManager.GM.Vertical = "Axis4";
            readyForKey = false;
            Debug.Log("Up Assigned to Axis4");
            options.gameObject.GetComponent<Text>().text = "PRESS JUMP BUTTON";
            Uassigned = true;
        }
        if (Input.GetAxis("Axis5") > 0.8)
        {
            GameManager.GM.Vertical = "Axis5";
            readyForKey = false;
            Debug.Log("Up Assigned to Axis5");
            options.gameObject.GetComponent<Text>().text = "PRESS JUMP BUTTON";
            Uassigned = true;
        }
        if (Input.GetAxis("Axis6") > 0.8)
        {
            GameManager.GM.Vertical = "Axis6";
            readyForKey = false;
            Debug.Log("Up Assigned to Axis6");
            options.gameObject.GetComponent<Text>().text = "PRESS JUMP BUTTON";
            Uassigned = true;
        }


        if (Input.GetAxis("Axis7") > 0.8)
        {
            GameManager.GM.Vertical = "Axis7";
            readyForKey = false;
            Debug.Log("Up Assigned to Axis7");
            options.gameObject.GetComponent<Text>().text = "PRESS JUMP BUTTON";
            Uassigned = true;
        }

        if (Input.GetAxis("Axis8") > 0.8)
        {
            GameManager.GM.Vertical = "Axis8";
            readyForKey = false;
            Debug.Log("Up Assigned to Axis8");
            options.gameObject.GetComponent<Text>().text = "PRESS JUMP BUTTON";
            Uassigned = true;
        }
    }

    // DOWN MOVEMENT
    public void DetectDownButton()
    {
        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
            {
                GameManager.GM.down = kcode;
                //PlayerPrefs.SetString("downKey", GameManager.GM.down.ToString());
                Dassigned = true;
                Debug.Log("Down Assigned to" + kcode);
                options.gameObject.GetComponent<Text>().text = "PRESS UP BUTTON";
            }
        }

        //JOYSTICK OPTIONS
        if (Input.GetAxis("Vertical") < -0.8)
        {
            GameManager.GM.Vertical = "Vertical";
            readyForKey = false;
            Debug.Log("Down Assigned to Y Axis");
            options.gameObject.GetComponent<Text>().text = "PRESS UP BUTTON";
            Dassigned = true;
        }

        if (Input.GetAxis("Axis3") < -0.8)
        {
            GameManager.GM.Vertical = "Axis3";
            readyForKey = false;
            Debug.Log("Down Assigned to Axis3");
            options.gameObject.GetComponent<Text>().text = "PRESS UP BUTTON";
            Dassigned = true;
        }
        if (Input.GetAxis("Axis4") < -0.5 && Input.GetAxis("Axis4") > -1.0)
        {
            GameManager.GM.Vertical = "Axis4";
            readyForKey = false;
            Debug.Log("Down Assigned to Axis4");
            options.gameObject.GetComponent<Text>().text = "PRESS UP BUTTON";
            Dassigned = true;
        }
        if (Input.GetAxis("Axis5") < -0.5 && Input.GetAxis("Axis5") > -1.0)
        {
            GameManager.GM.Vertical = "Axis5";
            readyForKey = false;
            Debug.Log("Down Assigned to Axis5");
            options.gameObject.GetComponent<Text>().text = "PRESS UP BUTTON";
            Dassigned = true;
        }
        if (Input.GetAxis("Axis6") < -0.5 && Input.GetAxis("Axis6") > -1.0)
        {
            GameManager.GM.Vertical = "Axis6";
            readyForKey = false;
            Debug.Log("Down Assigned to Axis6");
            options.gameObject.GetComponent<Text>().text = "PRESS UP BUTTON";
            Dassigned = true;
        }


        if (Input.GetAxis("Axis7") < -0.8)
        {
            GameManager.GM.Vertical = "Axis7";
            readyForKey = false;
            Debug.Log("Down Assigned to Axis7");
            options.gameObject.GetComponent<Text>().text = "PRESS UP BUTTON";
            Dassigned = true;
        }

        if (Input.GetAxis("Axis8") < -0.8)
        {
            GameManager.GM.Vertical = "Axis8";
            readyForKey = false;
            Debug.Log("Down Assigned to Axis8");
            options.gameObject.GetComponent<Text>().text = "PRESS UP BUTTON";
            Dassigned = true;
        }
    }
    public void DetectShieldButton()
    {
        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
            {
                GameManager.GM.jump = kcode;
                //PlayerPrefs.SetString("jump", GameManager.GM.jump.ToString());
                Shieldassigned = true;
                Debug.Log("Shield Assigned to" + kcode);
                options.gameObject.GetComponent<Text>().text = "PRESS CHECK/TALK BUTTON";
            }
        }

    }

    public void DetectCheckButton()
    {
        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
            {
                GameManager.GM.check = kcode;
                //PlayerPrefs.SetString("jump", GameManager.GM.jump.ToString());
                Checkassigned = true;
                Debug.Log("Shield Assigned to" + kcode);
                options.gameObject.GetComponent<Text>().text = "PRESS UPGRADE BUTTON";
            }
        }

    }

    public void DetectUpgradeButton()
    {
        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
            {
                GameManager.GM.upgrade = kcode;
                //PlayerPrefs.SetString("jump", GameManager.GM.jump.ToString());
                Upgradeassigned = true;
                Debug.Log("Upgrade Assigned to" + kcode);
                options.gameObject.GetComponent<Text>().text = "PRESS START BUTTON";
            }
        }

    }

    public void DetectStartButton()
    {
        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
            {
                GameManager.GM.pause = kcode;
                //PlayerPrefs.SetString("pause", GameManager.GM.pause.ToString());
                Debug.Log("Start Assigned to" + kcode);
                options1.gameObject.SetActive(false);
                options.gameObject.SetActive(false);
                optionsmenu = false;
                Pauseassigned = true;
                GameObject[] optionss = GameObject.FindGameObjectsWithTag("options");
                foreach (GameObject options in optionss)
                    GameObject.Destroy(options);
            }
        }
    }
}
