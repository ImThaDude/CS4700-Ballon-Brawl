using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayerCollision : MonoBehaviour
{

    public ClientNetworkManagerMVP client;

    public void SendBalloonCollision(RaycastHit2D[] colliders)
    {
        if (client != null)
        {
            var userId = colliders[0].transform.parent.name;
            var collisionPosition = colliders[0].point;
            client.SendCollision(userId, collisionPosition);
        }
    }

}
