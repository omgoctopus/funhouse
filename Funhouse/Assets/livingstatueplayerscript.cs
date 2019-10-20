using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class livingstatueplayerscript : MonoBehaviour
{
    private bool readytoattack = false;
    public float attacktimer;
    private float timer;
    private GameObject livingstatue;

    // Start is called before the first frame update
    void Start()
    {
        timer = attacktimer;
    }

    // Update is called once per frame
    void Update()
    {
        if (readytoattack == true)
           timer -= Time.deltaTime;

        if (timer <= 0)
        {
            livingstatue_enemyscript statuescript = livingstatue.GetComponent<livingstatue_enemyscript>();
            statuescript.statueisalive = true;
            readytoattack = false;
        }

        if (readytoattack == false)
            timer = attacktimer;
    }

    void OnTriggerExit2D(Collider2D trig)
    {

        //change camera range when player enters another room
        if (trig.name == "StatueAttackTrigger" & readytoattack==true)
        {
            livingstatue_enemyscript statuescript = livingstatue.GetComponent<livingstatue_enemyscript>();
            statuescript.statueisalive = true;
            readytoattack=false;
        }

        //ready the statue for attack
        if (trig.name == "StatueReadyTrigger")
        {
            readytoattack = true;
            livingstatue = trig.transform.parent.gameObject;

        }
    }
}
