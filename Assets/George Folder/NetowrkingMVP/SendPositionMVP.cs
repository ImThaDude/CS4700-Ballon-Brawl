using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendPositionMVP : MonoBehaviour
{
    public ClientNetworkManagerMVP client;
    public Rigidbody2D rb;
    Vector3 cachedPosition;
    Vector3 catchedVelocty;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (cachedPosition != transform.position) {
            client.SendPosition(transform.position, ((rb != null) ? rb.velocity : Vector2.zero));
            cachedPosition = transform.position;
        }
    }
}
