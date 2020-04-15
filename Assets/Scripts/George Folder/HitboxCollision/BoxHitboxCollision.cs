using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoxHitboxCollision : MonoBehaviour
{
    public BoxCollider2D BoxHitbox;
    
    public CollisionsEvent OnBoxCollisions;

    public LayerMask DetectionMask;

    public bool ColliderEnabled = true;

    void Start() {
        BoxHitbox.isTrigger = true;
    }

    void Update() {
        if (ColliderEnabled && BoxHitbox.isActiveAndEnabled) {
            var cols = Physics2D.BoxCastAll(BoxHitbox.transform.position, BoxHitbox.size, 0, Vector2.zero, 0, DetectionMask);
            if (cols.Length > 0) {
                Logger.DefaultLogger.Log("Collision detected.");
                OnBoxCollisions.Invoke(cols);
                Logger.DefaultLogger.Log(cols[0].transform.name);
            }
        }
    }

}
