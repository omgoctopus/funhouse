using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeWriterEffect : MonoBehaviour
{
    public float delay = 0.02f;
    public string fullText;
    private string currentText = "";
    public bool waitfortext = true;
    public int i;
    private GameObject player;
    // private Coroutine co;

    // Start is called before the first frame update
    void OnEnable()
    {
        // co = StartCoroutine(ShowText());
        StartCoroutine(ShowText());
        //Debug.Log("typewriter awake");
    }

    private void Update()
    {

    }

    IEnumerator ShowText()
    {
        for (i = 0; i <= fullText.Length; i++)
        {


            //Debug.Log(i);
            currentText = fullText.Substring(0, i);
            this.GetComponent<Text>().text = currentText;
            yield return new WaitForSeconds(delay);
        }
        player = GameObject.FindGameObjectWithTag("Player");
        player_move Playerscript = player.GetComponent<player_move>();
        Playerscript.waitfortext = false;
        Playerscript.waitforcommand = true;
        yield break;

    }
}
