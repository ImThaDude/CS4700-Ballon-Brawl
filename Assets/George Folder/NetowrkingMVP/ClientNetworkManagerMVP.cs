﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using UnityEngine.Events;

public class ClientNetworkManagerMVP : MonoBehaviour
{
    ClientMVP client;
    Thread t;

    public string ip = "localhost";
    public short port = 27757;
    public string userId = "TestUser";

    public UnityEvent OnStartedClient;

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
    public bool startClientQueue;

    void Update()
    {
        if (StartClient)
        {
            client = new ClientMVP(ip, port, userId);
            t = new Thread(new ThreadStart(client.Run));
            t.Start();

            client.OnReceivePlayerPositionData += playerHandler.OnReceivePositionFromPlayer;
            client.OnReceiveAnimationPlayerUpdate += playerHandler.OnReceiveAnimation;
            client.OnReceivePlayerCollision += playerHandler.OnReceiveCollision;
            client.OnReceiveDeletePlayer += playerHandler.OnDeletePlayer;
            client.OnReceiveRedZone += playerHandler.OnReceiveRedZone;
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

        if (startClientQueue) {
            startClientQueue = false;
            OnStartedClient.Invoke();
        }
    }

    public void SendPosition(Vector3 pos, Vector3 vel)
    {
        if (ClientConnected)
        {
            client.SendPosition(pos, vel);
        }
    }

    public void SendAnimation(float HP, bool IsGrounded, float Movement, float Dir, bool Flap, float PumpProgress)
    {
        if (ClientConnected)
        {
            client.SendAnimation(HP, IsGrounded, Movement, Dir, Flap, PumpProgress);
        }
    }

    public void SendCollision(string collidedId, Vector3 collisionPosition)
    {
        if (ClientConnected)
        {
            client.SendCollision(collidedId, collisionPosition);
        }
    }

    public void RequestForServerData()
    {
        client.RequestForAllData();
        ClientConnected = true;
        Debug.Log("Requesting for all server data...");
        startClientQueue = true;
    }

    public void DisconnectFromServer()
    {
        if (ClientConnected)
        {
            client.DisconnectFromServer();
        }
    }

    void OnDestroy()
    {
        client.stopRunning = true;
    }

    public void ChangeIP(string ip)
    {
        this.ip = ip;
    }

    public void InitializeClient()
    {
        StartClient = true;
    }
}
