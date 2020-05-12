using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingBoxCastAll : MonoBehaviour
{
    public LayerMask collisionLayerMask;
    // Update is called once per frame
    void Update()
    {
        var collisions = Physics2D.BoxCastAll(transform.position, transform.localScale, 0, Vector2.zero, 0, collisionLayerMask);
        if (collisions.Length > 0) {
            Debug.Log("Collided");
        }
    }
}
