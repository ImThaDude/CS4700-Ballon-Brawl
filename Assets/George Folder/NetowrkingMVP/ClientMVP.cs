using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using System.Threading;

public class ClientMVP
{
    public string UserId = "";
    public string ip = "localhost";
    public short port = 25565;
    public bool stopRunning = false;

    public ClientMVP(string ip, short port, string userId) {
        UserId = userId;
        this.ip = ip;
        this.port = port;
    }

    public void Run()
    {
        Debug.Log("Client started!");
        EventBasedNetListener listener = new EventBasedNetListener();
        NetManager client = new NetManager(listener);
        client.Start();
        client.Connect(ip /* host ip or name */, port /* port */, "SomeConnectionKey" /* text key or NetDataWriter */);

        listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
        {
            int func = dataReader.GetInt();
            switch (func)
            {
                case 0:
                    //This will request for UserId
                    SendUserId(fromPeer);
                    break;
                case 1:
                    if (OnConnectedToServer != null)
                    {
                        OnConnectedToServer();
                    }
                    break;
                case 2:
                    ReceivePosition(fromPeer, dataReader);
                    break;
                case 3:
                    ReceiveAnimation(fromPeer, dataReader);
                    break;
                case 4:
                    ReceiveMetadata(fromPeer, dataReader);
                    break;
                case 5:
                    break;
                case 6:
                    break;
                default:
                    break;

            }
            //Debug.Log(string.Format("We got: {0}", dataReader.GetString(100 /* max length of string */)));
            dataReader.Recycle();
        };

        while (!stopRunning)
        {
            client.PollEvents();
            Thread.Sleep(15);
        }

        client.Stop();
        Debug.Log("Client has ended...");
    }

    NetPeer serverPeer;

    void SendUserId(NetPeer peer)
    {
        serverPeer = peer;
        NetDataWriter writer = new NetDataWriter();
        writer.Put(0);
        writer.Put(UserId);
        peer.Send(writer, DeliveryMethod.ReliableOrdered);
        Debug.Log("[Client]Sent user id: " + UserId);
    }

    public void RequestForAllData() {
        NetDataWriter writer = new NetDataWriter();
        writer.Put(1);
        writer.Put(UserId);
        serverPeer.Send(writer, DeliveryMethod.ReliableOrdered);
    }

    public void SendPosition(Vector3 pos) {
        var cmd = SendCommand(2);
        cmd.Put(pos.x);
        cmd.Put(pos.y);
        cmd.Put(pos.z);
        serverPeer.Send(cmd, DeliveryMethod.ReliableOrdered);
        //Debug.Log("[Client]Sending Position: " + pos);
    }

    public void SendAnimation(float HP, bool IsGrounded, float Movement, float Dir, bool Flap, float PumpProgress) {
        var cmd = SendCommand(3);
        cmd.Put(HP);
        cmd.Put(IsGrounded);
        cmd.Put(Movement);
        cmd.Put(Dir);
        cmd.Put(Flap);
        cmd.Put(PumpProgress);
        serverPeer.Send(cmd, DeliveryMethod.ReliableOrdered);
    }

    public void SendMetadata(int HP) {
        var cmd = SendCommand(4);
        cmd.Put(HP);
        serverPeer.Send(cmd, DeliveryMethod.ReliableOrdered);
    }

    public void ReceivePosition(NetPeer peer, NetDataReader reader) {
        var userId = reader.GetString();
        var x = reader.GetFloat();
        var y = reader.GetFloat();
        var z = reader.GetFloat();
        var pos = new Vector3(x, y, z);
        if (OnReceivePlayerPositionData != null) {
            OnReceivePlayerPositionData(userId, pos);
        }
        //Debug.Log("[Client]Received Position: " + pos);
    }

    public void ReceiveAnimation(NetPeer peer, NetDataReader reader) {
        var userId = reader.GetString();
        var HP = reader.GetFloat();
        var IsGrounded = reader.GetBool();
        var Movement = reader.GetFloat();
        var Dir = reader.GetFloat();
        var Flap = reader.GetBool();
        var PumpProgress = reader.GetFloat();
        if (OnReceiveAnimationPlayerUpdate != null) {
            OnReceiveAnimationPlayerUpdate(userId, HP, IsGrounded, Movement, Dir, Flap, PumpProgress);
        }
    }

    public void ReceiveMetadata(NetPeer peer, NetDataReader reader) {
        var userId = reader.GetString();
        var HP = reader.GetInt();
        if (OnReceivePlayerMetadata != null) {
            OnReceivePlayerMetadata(userId, HP);
        }
    }

    NetDataWriter SendCommand(int index) {
        NetDataWriter writer = new NetDataWriter();
        writer.Put(index);
        writer.Put(UserId);
        return writer;
    }

    public delegate void ConnectedToServerEvent();
    public ConnectedToServerEvent OnConnectedToServer;

    //This will send the player position data
    public delegate void ReceivePlayerPositionDataEvent(string userId, Vector3 position);
    public ReceivePlayerPositionDataEvent OnReceivePlayerPositionData;

    //This will send the animation updates
    public delegate void ReceiveAnimationPlayerUpdateEvent(string userId, float HP, bool IsGrounded, float Movement, float Dir, bool Flap, float PumpProgress);
    public ReceiveAnimationPlayerUpdateEvent OnReceiveAnimationPlayerUpdate;

    //This will send the hp to all the players this will change the state to dead
    public delegate void ReceivePlayerMetadataEvent(string userId, int health);
    public ReceivePlayerMetadataEvent OnReceivePlayerMetadata;

    //This will send if this player has collided with someone else
    public delegate void ReceivePlayerCollisionEvent(string userId, Vector3 position);
    public ReceivePlayerCollisionEvent OnReceivePlayerCollision;
}
