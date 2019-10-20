using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SneekPlayerScript : MonoBehaviour
{
    private bool readytoattack = false, needtoestablishsneeklocation=true, playerisleftofsneek;
    private GameObject sneekbody, sneekparent;
    private float sneekxvalue;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //if you die, sneek is no longer aggro and re-establish location of sneek compared to player
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Hazard")
        {
            sneekenemyscript sneekscript = sneekbody.GetComponent<sneekenemyscript>();
            sneekscript.sneekisaggro = false;
            needtoestablishsneeklocation = true;
            sneekscript.sneekseesplayer = false;
        }
    }

        void OnTriggerStay2D(Collider2D trig)
    {
        if (trig.name == "SneakySneekTrigger")
        {
            sneekparent = trig.transform.parent.gameObject;
            sneekbody = sneekparent.transform.GetChild(1).gameObject;
            sneekenemyscript sneekscript = sneekbody.GetComponent<sneekenemyscript>();
            sneekscript.playerisinsneekrange = true;

            //if it hasn't been done yet, check where sneek is in relation to player
            if (needtoestablishsneeklocation == true)
            {
                sneekxvalue = sneekbody.transform.position.x;
                if(transform.position.x<sneekxvalue)
                { playerisleftofsneek = true; }
                if (transform.position.x > sneekxvalue)
                { playerisleftofsneek = false; }
                needtoestablishsneeklocation = false;
            }

            if (sneekscript.sneekisaggro == false)
            {
                checkforsneekaggro();
            }
        }
    }
    void OnTriggerExit2D(Collider2D trig)
    {
        if (trig.name == "SneakySneekTrigger")
        {
            sneekparent = trig.transform.parent.gameObject;
            sneekbody = sneekparent.transform.GetChild(1).gameObject;
            sneekenemyscript sneekscript = sneekbody.GetComponent<sneekenemyscript>();
            sneekscript.playerisinsneekrange = false;
            sneekscript.needtoresetposition = true;
            sneekscript.sneekisaggro = false;
            needtoestablishsneeklocation = true;
        }
    }

    void checkforsneekaggro()
    {
        if(playerisleftofsneek==true && transform.position.x > sneekxvalue + 16)
        {
            sneekenemyscript sneekscript = sneekbody.GetComponent<sneekenemyscript>();
            sneekscript.sneekisaggro = true;
        }
        if (playerisleftofsneek == false && transform.position.x < sneekxvalue - 16)
        {
            sneekenemyscript sneekscript = sneekbody.GetComponent<sneekenemyscript>();
            sneekscript.sneekisaggro = true;
        }
    }
}

