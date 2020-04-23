using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColliderGameObjectDelegate : MonoBehaviour
{
    public int AttachedIndex = 0;
    public void AttachCollisionBox(string entityId, int layerMask, GameObject hit) {
        if (layerMask == AttachedIndex) {
            hit.AddComponent<BoxCollider2D>();
        }
    }
}
