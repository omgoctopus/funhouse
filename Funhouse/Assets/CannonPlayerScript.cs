using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonPlayerScript : MonoBehaviour
{
    private GameObject cannonparent, cannonbody;
    private bool aggrocondition1, aggrocondition2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.name == "CannonRespawnTrigger")
        {
            Debug.Log("cannonrespawntrigger active");
            aggrocondition1 = true;
            if(aggrocondition1==true && aggrocondition2 == true)
            {
                cannonparent = collision.transform.parent.gameObject;
                cannonbody = cannonparent.transform.GetChild(0).gameObject;
                CannonEnemyScript cannonscript = cannonbody.GetComponent<CannonEnemyScript>();
                cannonscript.cannonisfiring = true;
            }
        }

        if (collision.name == "CannonFireTrigger")
        { aggrocondition2 = true;
            Debug.Log("cannonfiretrigger active");
        }
    }

    void OnTriggerExit2D(Collider2D trig)
        {
            if (trig.name == "CannonRespawnTrigger")
            {
                cannonparent = trig.transform.parent.gameObject;
                cannonbody = cannonparent.transform.GetChild(0).gameObject;
                CannonEnemyScript cannonscript = cannonbody.GetComponent<CannonEnemyScript>();
                cannonscript.needtoresetposition = true;
            cannonscript.cannonisfiring = false;
            aggrocondition1 = false;
            Debug.Log("cannonshouldhaverespawned");
            }

        if (trig.name == "CannonFireTrigger")
        {
            cannonbody = trig.transform.parent.gameObject;
            //cannonbody = cannonparent.transform.GetChild(1).gameObject;
            CannonEnemyScript cannonscript = cannonbody.GetComponent<CannonEnemyScript>();
            cannonscript.cannonisfiring = false;
            aggrocondition2 = false;
        }
    }
    
}
