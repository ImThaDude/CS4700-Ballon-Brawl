using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class TestingClientMVP : MonoBehaviour
{

    ClientMVP client;
    Thread t;

    public string ip = "localhost";
    public short port = 25565;
    public string userId = "TestUser";

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public bool StopClient = false;
    public bool StopThread = false;
    public bool StartClient = false;
    public Vector3 toBeSentPosition;
    public bool SendPosition = false;
    void Update()
    {
        if (StartClient)
        {
            client = new ClientMVP(ip, port, userId);
            t = new Thread(new ThreadStart(client.Run));
            t.Start();
            StartClient = false;
        }
        if (StopClient)
        {
            client.stopRunning = true;
            StopClient = false;
        }

        if (StopThread)
        {
            t.Abort();
            StopThread = false;
        }

        if (SendPosition)
        {
            client.SendPosition(toBeSentPosition, Vector3.zero);
            SendPosition = false;
        }
    }

    void OnDestroy() {
        client.stopRunning = true;
    }
}
