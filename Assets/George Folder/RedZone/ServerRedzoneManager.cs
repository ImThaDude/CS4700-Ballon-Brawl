using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerRedzoneManager : MonoBehaviour
{
    public bool StartReduceRedZone = false;
    public float RedZoneTime = 300f;
    public float RedZoneDistance = 63f;
    public float currTime = 0;
    public TestingServerMVP server;

    void Start() {
        currTime = RedZoneTime;
    }

    int tempTime;
    // Update is called once per frame
    void Update()
    {
        if (StartReduceRedZone) {
            currTime -= Time.deltaTime;
            if (tempTime != (int) currTime) {
                tempTime = (int) currTime;
                server.SendRedZone((currTime / RedZoneTime) * RedZoneDistance);
            }
        }
    }

    public void StartRedZone() {
        StartReduceRedZone = true;
    }
}
