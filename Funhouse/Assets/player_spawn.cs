using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class player_spawn : MonoBehaviour
{
    public GameObject livesUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        livesUI.gameObject.GetComponent<Text>().text = "Lives x"+ GameManager.GM.lives;
    }
}
