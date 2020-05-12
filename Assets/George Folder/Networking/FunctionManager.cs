using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib.Utils;
using System;

public delegate void ReceivedIndexNetManagerEvent(int functionHandlerId, NetDataReader reader);
public delegate NetDataWriter CreateDataWriterEvent();

public class FunctionManager : MonoBehaviour
{
    public class NetFunctionHandler {

        public NetFunctionHandler(int functionHandlerId, SendNetManagerEvent onSend, ReceivedNetManagerEvent onReceive) {
            this.functionHandlerId = functionHandlerId;
            this.OnSend = onSend;
            this.OnReceive = onReceive;
        }

        public int functionHandlerId = 0;
        
        public void Send(NetDataWriter writer) {
            if (OnSend != null) {
                OnSend(writer);
            }
        }

        public void Receive(NetDataReader receive) {
            if (OnReceive != null) {
                OnReceive(receive);
            }
        }

        public NetDataWriter CreateWriter() {
            var tempWriter = new NetDataWriter();
            tempWriter.Put(functionHandlerId);
            return tempWriter;
        }

        public SendNetManagerEvent OnSend;
        public ReceivedNetManagerEvent OnReceive;
    }

    public string ip = "0.0.0.0";
    public bool isLocal = true;
    public string deviceId = "";
    public FunctionHandler[] FunctionPool;
    List<NetFunctionHandler> netFunctionPool;

    void Start() {
        netFunctionPool = new List<NetFunctionHandler>();

        for (int i = 0; i < FunctionPool.Length; i++) {
            var functionHandler = FunctionPool[i];
            var netFunction = new NetFunctionHandler(i, Send, functionHandler.Receive);
            functionHandler.OnSendNetManager += netFunction.Send;
            functionHandler.OnCreateDataWriter += netFunction.CreateWriter;
            netFunctionPool.Insert(i, netFunction);
        }
    }

    public void InitializeServer(bool isLocal) {
        if (isLocal) {
            ConnectToLocalServer();
            return;
        }

        ConnectToOnlineServer();
        return;
    }

    public void ConnectToOnlineServer() {

    }

    //For local the client will just create a prefab that has the server ready made. It will communicate through the net manager and the net manager will be directly connected to each other.
    public void ConnectToLocalServer() {

    }

    public FunctionManager localInstance;

    public void Send(NetDataWriter writer) {
        if (isLocal) {
            var temp = new NetDataReader();
            temp.SetSource(writer);
            localInstance.Receive(temp);
            return;
        }

        Debug.Log("Not yet implemented.");
        return;
    }

    public void Receive(NetDataReader reader) {
        var functionHandlerId = reader.GetInt();
        netFunctionPool[functionHandlerId].Receive(reader);
    }
    
}
