using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardEnemyScript : MonoBehaviour
{
    public bool playerisinrange = false, needtoresetposition = false, needtoswap=false;
    public Vector2 startingposition, wizardposition, playerposition;
    public float teleporttimer;
    private float timer;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        startingposition = transform.position;
        timer = teleporttimer;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerisinrange == true)
            timer -= Time.deltaTime;

        if (timer <= 0)
        {
            swap();
        }

        if (playerisinrange == false)
            timer = teleporttimer;

        if (needtoresetposition == true)
        {
            transform.position = startingposition;
            needtoresetposition = false;
        }
    }

    void swap()
    {
        if (playerisinrange == true)
        {
            wizardposition = transform.position;
            player = GameObject.FindGameObjectWithTag("Player");
            playerposition = player.transform.position;
            needtoswap = true;
            transform.position = playerposition;
            timer = teleporttimer;
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
