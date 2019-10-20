using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirPatrolMove : MonoBehaviour
{
    public float movespeed, left_end_of_loop_xvalue, right_end_of_loop_xvalue, riseheight;
    private Vector2 startposition;
    public bool movingright;
    private bool movingup, yaxismovement;
    private bool needtorise=false;
    private int changeddirections=0;
    private float riseX;


    // Start is called before the first frame update
    void Start()
    {
        startposition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log("x="+transform.position.x + "needtorise="+needtorise+"RiseX="+riseX);
        if(changeddirections==2)
        { timetorise(); }

        if (transform.position.x < right_end_of_loop_xvalue && movingright == true && yaxismovement!=true)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(movespeed, 0);
        }
        if (transform.position.x >= right_end_of_loop_xvalue && movingright == true && yaxismovement != true)
        {
            movingright = false;
            changeddirections = changeddirections + 1;
            //Debug.Log("changed directions" + changeddirections);
        }
        if (transform.position.x > left_end_of_loop_xvalue && movingright == false && yaxismovement != true)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-movespeed, 0);
        }
        if (transform.position.x <= left_end_of_loop_xvalue && movingright == false && yaxismovement != true)
        {
            movingright = true;
            changeddirections = changeddirections + 1;
            //Debug.Log("changed directions" + changeddirections);
        }
        if (transform.position.x <= (riseX + 1) && transform.position.x >= (riseX - 1))
        {
            if (needtorise == true) { rise(); }
        }
    }

    void timetorise()
    {
        needtorise = true;
        movingup = true;
        riseX= Random.Range(left_end_of_loop_xvalue+16, right_end_of_loop_xvalue-16);
        changeddirections = 0;
    }

    void rise()
    {
        yaxismovement = true;
        if (transform.position.y < riseheight && movingup == true)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, movespeed);
        }
        if (transform.position.y >= riseheight && movingup == true)
        {
            movingup = false;
        }
        if (transform.position.y > startposition.y && movingup == false)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -movespeed);
        }
        if (transform.position.y <= startposition.y && movingup == false)
        {
            needtorise = false;
            yaxismovement = false;
        }

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
