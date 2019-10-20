using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LRmovescript : MonoBehaviour
{
    public float startspeed, speedafterupgradeisused, left_end_of_loop_xvalue, right_end_of_loop_xvalue;
    public bool movingright;
    private float movespeed;
    private Vector2 startposition;


    // Start is called before the first frame update
    void Start()
    {
        startposition=transform.position;
        movespeed = startspeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.GM.playerusingupgrade == true)
            movespeed = speedafterupgradeisused;

        if (GameManager.GM.playerusingupgrade == false)
            movespeed = startspeed;


        if (transform.position.x<right_end_of_loop_xvalue&&movingright==true)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(movespeed, 0);
        }
        if (transform.position.x >= right_end_of_loop_xvalue && movingright == true)
        {
            movingright = false;
        }
        if (transform.position.x > left_end_of_loop_xvalue && movingright == false)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-movespeed, 0);
        }
        if (transform.position.x <= left_end_of_loop_xvalue && movingright == false)
        {
            movingright = true;
        }
    }

}
