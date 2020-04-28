using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Threading.Tasks;
using UnityEngine;
using System.Threading;

public class ServerMVP
{
    public ServerMVP(short post)
    {
        this.port = port;
        peerPool = new Dictionary<string, NetPeer>();
        userIdPool = new Dictionary<NetPeer, string>();
        positionPool = new Dictionary<string, PlayerPosition>();
        animationPool = new Dictionary<string, AnimationState>();
        metadataPool = new Dictionary<string, PlayerMetadata>();

        startPositionPool = new List<Vector3>();
    }

    public bool stopRunning = false;
    public short port = 27757;
    public void Run()
    {
        Debug.Log("Server started!");
        EventBasedNetListener listener = new EventBasedNetListener();
        NetManager server = new NetManager(listener);
        server.Start(port /* port */);

        listener.ConnectionRequestEvent += request =>
        {
            if (server.ConnectedPeersCount < 100 /* max connections */)
                request.AcceptIfKey("SomeConnectionKey");
            else
                request.Reject();
        };

        listener.PeerConnectedEvent += peer =>
        {
            Debug.Log(string.Format("We got connection: {0}", peer.EndPoint)); // Show peer ip
            NetDataWriter writer = new NetDataWriter();                 // Create writer class
            writer.Put(0); //Request for user
            writer.Put("Hello client!");                                // Put some string
            peer.Send(writer, DeliveryMethod.ReliableOrdered);             // Send with reliability
        };

        listener.NetworkReceiveEvent += (peer, reader, deliveryMethod) =>
        {
            int func = reader.GetInt();
            switch (func)
            {
                case 0:
                    RegisterUserToServer(peer, reader);
                    break;
                case 1:
                    SendAllData(peer);
                    break;
                case 2:
                    ReceivePosition(peer, reader);
                    SendPositionToAllPeers(peer);
                    break;
                case 3:
                    ReceiveAnimation(peer, reader);
                    SendAnimationToAllPeers(peer);
                    break;
                case 4:
                    ReceiveMetadata(peer, reader);
                    SendMetadataToAllPeers(peer);
                    break;
                case 5:
                    break;
                case 6:
                    break;
                default:
                    break;
            }
            reader.Recycle();
        };

        while (!stopRunning)
        {
            server.PollEvents();
            Thread.Sleep(15);
        }
        server.Stop();
        //Debug.Log("Server has ended...");
    }

    public async void SendAllData(NetPeer peer)
    {

        //Debug.Log("Received request for all data...");
        //Problem with the peerPool. Might be better by userid.
        //foreach (var userId in peerPool.Keys) {
        //Debug.Log(peerPool.Count);
        foreach (var userId in userIdPool.Values)
        {
            //Debug.Log("Sending position to userid: " + userId);
            SendPosition(peer, userId);
            await Task.Delay(2);
            SendAnimation(peer, userId);
            await Task.Delay(2);
            SendMetadata(peer, userId);
            await Task.Delay(2);
        }
    }

    public async void SendPositionToAllPeers(NetPeer peerSending)
    {
        string userId = userIdPool[peerSending];
        foreach (var peer in userIdPool.Keys)
        {
            if (peer != peerSending)
            {
                SendPosition(peer, userId);
                await Task.Delay(2);
            }
        }
    }

    public void SendPosition(NetPeer peer, string UserId)
    {
        if (positionPool.ContainsKey(UserId))
        {
            var cmd = SendCommand(2);
            cmd.Put(UserId);
            var pos = positionPool[UserId].position;
            cmd.Put(pos.x);
            cmd.Put(pos.y);
            cmd.Put(pos.z);
            peer.Send(cmd, DeliveryMethod.ReliableOrdered);
            //Debug.Log("[Server]Sent " + UserId + " position of " + pos + " to " + userIdPool[peer]);
        }
    }

    public async void SendAnimationToAllPeers(NetPeer peerSending)
    {
        string userId = userIdPool[peerSending];
        foreach (var peer in userIdPool.Keys)
        {
            if (peer != peerSending)
            {
                SendAnimation(peer, userId);
                await Task.Delay(2);
            }
        }
    }

    public void SendAnimation(NetPeer peer, string UserId)
    {
        if (animationPool.ContainsKey(UserId))
        {
            var cmd = SendCommand(3);
            cmd.Put(UserId);
            var anim = animationPool[UserId];
            cmd.Put(anim.HP);
            cmd.Put(anim.IsGrounded);
            cmd.Put(anim.Movement);
            cmd.Put(anim.Dir);
            cmd.Put(anim.Flap);
            cmd.Put(anim.PumpProgress);
            peer.Send(cmd, DeliveryMethod.ReliableOrdered);
        }
    }

    public async void SendMetadataToAllPeers(NetPeer peerSending)
    {
        string userId = userIdPool[peerSending];
        foreach (var peer in userIdPool.Keys)
        {
            if (peer != peerSending)
            {
                SendMetadata(peer, userId);
                await Task.Delay(2);
            }
        }
    }

    public void SendMetadata(NetPeer peer, string UserId)
    {
        if (metadataPool.ContainsKey(UserId))
        {
            var cmd = SendCommand(4);
            cmd.Put(UserId);
            var meta = metadataPool[UserId];
            cmd.Put(meta.HP);
            peer.Send(cmd, DeliveryMethod.ReliableOrdered);
        }
    }

    NetDataWriter SendCommand(int index)
    {
        NetDataWriter writer = new NetDataWriter();
        writer.Put(index);
        return writer;
    }

    public Dictionary<string, NetPeer> peerPool;
    public Dictionary<NetPeer, string> userIdPool;
    public void RegisterUserToServer(NetPeer peer, NetDataReader reader)
    {
        var userId = reader.GetString();
        if (!peerPool.ContainsKey(userId))
        {
            peerPool.Add(userId, peer);
        }
        if (!userIdPool.ContainsKey(peer))
        {
            userIdPool.Add(peer, userId);
        }
        NetDataWriter writer = new NetDataWriter();
        writer.Put(1); //Send received user
        peer.Send(writer, DeliveryMethod.ReliableOrdered);
        //Debug.Log("[Server]RegisteredUserToServer: " + userId);


        //Adding user and their started position;
        if (!positionPool.ContainsKey(userId))
        {
            positionPool.Add(userId, new PlayerPosition() { });
        }
        float x = startPositionPool[startPositionIndex].x, y = startPositionPool[startPositionIndex].y, z = startPositionPool[startPositionIndex].z;
        positionPool[userId].position.x = x;
        positionPool[userId].position.y = y;
        positionPool[userId].position.z = z;
        startPositionIndex = (startPositionIndex + 1) % startPositionPool.Count;
        //Debug.Log("[Server]Assigned initial position of: " + startPositionPool[startPositionIndex] + " to userid: " + userId);
    }

    public class AnimationState
    {
        //All animations will go here and will get updated
        public float HP;
        public bool IsGrounded;
        public float Movement;
        public float Dir;
        public bool Flap;
        public float PumpProgress;
    }

    public Dictionary<string, AnimationState> animationPool;

    public void ReceiveAnimation(NetPeer peer, NetDataReader reader)
    {
        string UserId = reader.GetString();
        if (!animationPool.ContainsKey(UserId))
        {
            animationPool.Add(UserId, new AnimationState() { });
        }
        animationPool[UserId].HP = reader.GetFloat();
        animationPool[UserId].IsGrounded = reader.GetBool();
        animationPool[UserId].Movement = reader.GetFloat();
        animationPool[UserId].Dir = reader.GetFloat();
        animationPool[UserId].Flap = reader.GetBool();
        animationPool[UserId].PumpProgress = reader.GetFloat();
    }

    public class PlayerPosition
    {
        //All positions will go here
        public Vector3 position;
    }

    public Dictionary<string, PlayerPosition> positionPool;

    public void ReceivePosition(NetPeer peer, NetDataReader reader)
    {
        string UserId = reader.GetString();
        if (!positionPool.ContainsKey(UserId))
        {
            positionPool.Add(UserId, new PlayerPosition() { });
        }
        float x = reader.GetFloat(), y = reader.GetFloat(), z = reader.GetFloat();
        positionPool[UserId].position.x = x;
        positionPool[UserId].position.y = y;
        positionPool[UserId].position.z = z;
        //Debug.Log("[Server]ReceivedPosition: " + x + " " + y + " " + z + " for user " + UserId);
    }

    public class PlayerMetadata
    {
        //all metadata will go here
        public int HP;
    }

    public Dictionary<string, PlayerMetadata> metadataPool;

    public void ReceiveMetadata(NetPeer peer, NetDataReader reader)
    {
        string UserId = reader.GetString();
        if (!metadataPool.ContainsKey(UserId))
        {
            metadataPool.Add(UserId, new PlayerMetadata() { });
        }
        metadataPool[UserId].HP = reader.GetInt();
    }

    public List<Vector3> startPositionPool;
    public int startPositionIndex = 0;

    public void PushPositionIntoDatabase(Vector3 position)
    {
        startPositionPool.Add(position);
    }

}
