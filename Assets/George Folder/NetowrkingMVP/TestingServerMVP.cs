using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class TestingServerMVP : MonoBehaviour
{
    ServerMVP client;
    Thread t;
    public short port = 27757;

    // Start is called before the first frame update
    void Start()
    {
        client = new ServerMVP(port);
        t = new Thread(new ThreadStart(client.Run));
        var gms = GameObject.FindGameObjectsWithTag("spawnLocation");
        foreach (var l in gms) {
            client.PushPositionIntoDatabase(l.transform.position);
        }
        t.Start();
    }

    // Update is called once per frame
    public bool StopClient = false;
    public bool StopThread = false;
    void Update()
    {
        if (StopClient) {
            client.stopRunning = true;
            StopClient = false;
        }

        if (StopThread) {
            t.Abort();
            StopThread = false;
        }
    }

    void OnDestroy() {
        client.stopRunning = true;
    }
}
