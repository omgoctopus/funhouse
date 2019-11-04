using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorManPlayerscript : MonoBehaviour
{
    private GameObject mirrormanbody, mirrormanparent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay2D(Collider2D trig)
    {
        if (trig.name == "Mirrormantrigger")
        {
            mirrormanparent = trig.transform.parent.gameObject;
            mirrormanbody = mirrormanparent.transform.GetChild(1).gameObject;
            MirrormanEnemy mirrormanscript = mirrormanbody.GetComponent<MirrormanEnemy>();
            mirrormanscript.playerisinrange = true;
        }
    }
    void OnTriggerExit2D(Collider2D trig)
    {
        if (trig.name == "Mirrormantrigger")
        {
            mirrormanparent = trig.transform.parent.gameObject;
            mirrormanbody = mirrormanparent.transform.GetChild(1).gameObject;
            MirrormanEnemy mirrormanscript = mirrormanbody.GetComponent<MirrormanEnemy>();
            mirrormanscript.playerisinrange = false;
            mirrormanscript.needtoresetposition = true;
        }
    }
}
