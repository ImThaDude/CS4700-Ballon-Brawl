using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib.Utils;

public delegate void SendNetManagerEvent(NetDataWriter write);
public delegate void ReceivedNetManagerEvent(NetDataReader reader);

[System.Serializable]
public class FunctionHandler : MonoBehaviour
{
    public SendNetManagerEvent OnSendNetManager;
    public CreateDataWriterEvent OnCreateDataWriter;

    public void Send(NetDataWriter write) {
        if (OnSendNetManager != null) {
            OnSendNetManager(write);
        }
    }

    public virtual void Receive(NetDataReader reader) {

    }

    public NetDataWriter CreateDataWriter() {
        if (OnCreateDataWriter != null) {
            return OnCreateDataWriter();
        }

        return null;
    }
}
