using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

public class BoxHitboxCollision : MonoBehaviour
{
    public BoxCollider2D BoxHitbox;

    public CollisionsEvent OnBoxCollisions;

    public LayerMask DetectionMask;

    public bool ColliderEnabled = true;

    public bool isHit = false;

	public Logger logger;

	private void Awake() {
		logger = Logger.DefaultLogger;
	}

    void Start()
    {
        BoxHitbox.isTrigger = true;
    }

    void Update()
    {
        if (ColliderEnabled && BoxHitbox.isActiveAndEnabled)
        {
            var cols = Physics2D.BoxCastAll(BoxHitbox.transform.position, BoxHitbox.size, 0, Vector2.zero, 0, DetectionMask);
            if (cols.Length > 0)
            {
                if (!isHit)
                {
                    logger.Log("Collision detected.", this);
                    OnBoxCollisions.Invoke(cols);
                    logger.Log(cols[0].transform.name, this);
                    isHit = true;
                }
                return;
            }
        }
        isHit = false;
    }

    void OnDrawGizmos() {
        //Gizmos.DrawCube(BoxHitbox.transform.position, BoxHitbox.size);
    }

}
