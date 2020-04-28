using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class ClientNetworkManagerMVP : MonoBehaviour
{
    ClientMVP client;
    Thread t;

    public string ip = "localhost";
    public short port = 27757;
    public string userId = "TestUser";

    // Start is called before the first frame update
    void Start()
    {
        //Quick patch to create a quick user.
        userId = DateTime.Now.Ticks.ToString();
    }

    // Update is called once per frame
    public bool StopClient = false;
    public bool StopThread = false;
    public bool StartClient = false;
    public Vector3 toBeSentPosition;
    public bool ClientConnected = false;

    public PlayerHandlerMVP playerHandler;

    void Update()
    {
        if (StartClient)
        {
            client = new ClientMVP(ip, port, userId);
            t = new Thread(new ThreadStart(client.Run));
            t.Start();

            client.OnReceivePlayerPositionData += playerHandler.OnReceivePositionFromPlayer;
            client.OnReceiveAnimationPlayerUpdate += playerHandler.OnReceiveAnimation;
            playerHandler.client = this;

            client.OnConnectedToServer += RequestForServerData;

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
    }

    public void SendPosition(Vector3 pos) {
        if (ClientConnected) {
            client.SendPosition(pos);
        }
    }

    public void SendAnimation(float HP, bool IsGrounded, float Movement, float Dir, bool Flap, float PumpProgress) {
        if (ClientConnected) {
            client.SendAnimation(HP, IsGrounded, Movement, Dir, Flap, PumpProgress);
        }
    }

    public void RequestForServerData() {
        client.RequestForAllData();
        ClientConnected = true;
        Debug.Log("Requesting for all server data...");
    }

    void OnDestroy() {
        client.stopRunning = true;
    }
}
