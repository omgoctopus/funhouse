using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardPlayerScript : MonoBehaviour
{
    private GameObject wizardbody, wizardparent;

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
        if (trig.name == "WizardTrigger")
        {
            wizardparent = trig.transform.parent.gameObject;
            wizardbody = wizardparent.transform.GetChild(1).gameObject;
            WizardEnemyScript wizardscript = wizardbody.GetComponent<WizardEnemyScript>();
            wizardscript.playerisinrange = true;
            if (wizardscript.needtoswap == true)
            {
                transform.position = wizardscript.wizardposition;
                wizardscript.needtoswap = false;
            }
        }
    }
    void OnTriggerExit2D(Collider2D trig)
    {
        if (trig.name == "Mirrormantrigger")
        {
            wizardparent = trig.transform.parent.gameObject;
            wizardbody = wizardparent.transform.GetChild(1).gameObject;
            WizardEnemyScript wizardscript = wizardbody.GetComponent<WizardEnemyScript>();
            wizardscript.playerisinrange = false;
            wizardscript.needtoresetposition = true;
        }
    }
}
