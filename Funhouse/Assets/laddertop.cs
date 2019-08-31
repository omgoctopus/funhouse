using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laddertop : MonoBehaviour
{
    public GameObject player;
    private player_move playerScript;


    // Start is called before the first frame update
    void Start()
    {
        playerScript = player.GetComponent<player_move>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerScript.onladder == true && playerScript.moveY < 0)
        {
            Debug.Log("player on ladder");
            Physics2D.IgnoreCollision(player.GetComponent<BoxCollider2D>(), GetComponent<BoxCollider2D>());
        }
    }

    
}
