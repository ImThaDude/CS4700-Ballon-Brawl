using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib.Utils;

public class TestingFunctionHandler : FunctionHandler
{
    public bool TestSendInteraction = false;

    void Start() {

    }

    void Update() {
        if (TestSendInteraction) {
            TestSend();
            TestSendInteraction = false;
        }
    }

    public void TestSend() {
        NetDataWriter writer = CreateDataWriter();
        writer.Put("Hahaha");
        Send(writer);
    }

    public override void Receive(NetDataReader reader) {
        Debug.Log("Test" + reader.GetString());
    }

}
