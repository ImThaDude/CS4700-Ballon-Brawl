using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendPositionMVP : MonoBehaviour
{
    public ClientNetworkManagerMVP client;
    void Update()
    {
        client.SendPosition(transform.position);
    }
}
