﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roomtrigger : MonoBehaviour
{
    private GameObject viewport, player;

    // this script uses the center of the player to check which room they are in and adjust camera accordingly

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
        //get script for camera system
        viewport = GameObject.FindGameObjectWithTag("MainCamera");
        CameraSystem cameraScript = viewport.GetComponent<CameraSystem>();

        player = GameObject.FindGameObjectWithTag("Player");

        //change camera range when player enters another room
        if (trig.name == "Room1")
        {
            cameraScript.xMin = 0;
            cameraScript.xMax = 320;
            cameraScript.yMin = 0;
            cameraScript.yMax = 240;
        }
        if (trig.name == "Room2")
        {
            cameraScript.xMin = 0;
            cameraScript.xMax = 320;
            cameraScript.yMin = 480;
            cameraScript.yMax = 480;
        }
        if (trig.name == "Room3")
        {
            cameraScript.xMin = 0;
            cameraScript.xMax = 0;
            cameraScript.yMin = 720;
            cameraScript.yMax = 720;
        }
        if (trig.name == "Room4")
        {
            cameraScript.xMin = 640;
            cameraScript.xMax = 640;
            cameraScript.yMin = 240;
            cameraScript.yMax = 480;
        }
        if (trig.name == "Room5")
        {
            cameraScript.xMin = 640;
            cameraScript.xMax = 640;
            cameraScript.yMin = 0;
            cameraScript.yMax = 0;
        }
        if (trig.name == "Room6")
        {
            cameraScript.xMin = 960;
            cameraScript.xMax = 1280;
            cameraScript.yMin = 0;
            cameraScript.yMax = 0;
        }
        if (trig.name == "Room7")
        {
            cameraScript.xMin = 960;
            cameraScript.xMax = 960;
            cameraScript.yMin = -480;
            cameraScript.yMax = -240;
        }
        if (trig.name == "Room8")
        {
            cameraScript.xMin = 640;
            cameraScript.xMax = 640;
            cameraScript.yMin = -240;
            cameraScript.yMax = -240;
        }
        if (trig.name == "Room9")
        {
            cameraScript.xMin = 1280;
            cameraScript.xMax = 1280;
            cameraScript.yMin = -480;
            cameraScript.yMax = -480;
        }
        if (trig.name == "Room10")
        {
            cameraScript.xMin = 960;
            cameraScript.xMax = 1280;
            cameraScript.yMin = -720;
            cameraScript.yMax = -720;
        }
        if (trig.name == "Room11")
        {
            cameraScript.xMin = 960;
            cameraScript.xMax = 960;
            cameraScript.yMin = 240;
            cameraScript.yMax = 240;
        }
        if (trig.name == "Room12")
        {
            cameraScript.xMin = 1280;
            cameraScript.xMax = 1280;
            cameraScript.yMin = 240;
            cameraScript.yMax = 240;
        }
        if (trig.name == "Room13")
        {
            cameraScript.xMin = -336;
            cameraScript.xMax = -336;
            cameraScript.yMin = 480;
            cameraScript.yMax = 720;
        }

        if (trig.name == "Room14")
        {
            cameraScript.xMin = 0;
            cameraScript.xMax = 0;
            cameraScript.yMin = -520;
            cameraScript.yMax = -440;
        }


        //teleport player somewhere else on the map as if they have entered adjacent room
        if (trig.name == "Gateway1")
        {
            player.transform.position = new Vector2 (-176, 648);
        }
        if (trig.name == "Gateway2" || trig.name == "Gateway3")
        {
            player.transform.position = new Vector2(-160, -72);
        }
    }
}