using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallScript : MonoBehaviour
{
    public float cannonballvelocity;
    public int firedirection=-1; //fires left by default. If cannon is facing right, set to positive one

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("cannonballvelocity="+cannonballvelocity);
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(firedirection, 1)*cannonballvelocity;
        //transform.position = transform.position + new Vector3(firedirection*cannonballvelocity, cannonballvelocity);
    }

    void OnTriggerExit2D(Collider2D trig)
    {
        if (trig.name == "CannonRespawnTrigger")
        {
            gameObject.SetActive(false);
        }

        if (trig.name == "CannonFireTrigger")
        {
            gameObject.SetActive(false);
        }
    }
}
