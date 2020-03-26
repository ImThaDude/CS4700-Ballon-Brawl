using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonPop : MonoBehaviour
{

    float BallonPopForce = 500;

    public Transform GetBodyInParent(Transform hitbox)
    {
        if (hitbox != null)
        {
            var body = hitbox.GetComponent<BalloonFighterBody>();
            if (body == null)
            {
                return GetBodyInParent(hitbox.parent);
            }
            return hitbox;
        }
        return null;
    }

    public void PopBalloon(RaycastHit2D[] colliders)
    {
        var detectorsTransform = GetBodyInParent(transform);

        var collidersTransform = GetBodyInParent(colliders[0].collider.transform);

        var impactPoint = colliders[0].point;

        Vector3 velocityReflectedTotal = Vector3.zero;
        if (detectorsTransform != null) {
            var detectorsBody = detectorsTransform.GetComponent<BalloonFighterBody>();

            var detectorsMetadata = detectorsTransform.GetComponent<PlayerMetadata>();

            var detectorVel = detectorsBody.rb.velocity;
            var reflectiveDirection = detectorsBody.transform.position - new Vector3(impactPoint.x, impactPoint.y, 0);
            //detectorsBody.rb.velocity = reflectiveDirection.normalized * detectorVel.magnitude;

            detectorsMetadata.Damage();

            if (detectorsMetadata.Health > 0) {
                detectorsBody.rb.AddForce(BallonPopForce * reflectiveDirection.normalized);
            }
        }
        if (collidersTransform != null) {
            var collidersBody = collidersTransform.GetComponent<BalloonFighterBody>();

            var collidersVel = collidersBody.rb.velocity;
            var reflectiveDirection = collidersBody.transform.position - new Vector3(impactPoint.x, impactPoint.y, 0);
            //collidersBody.rb.velocity = reflectiveDirection.normalized * collidersVel.magnitude;

            collidersBody.rb.AddForce(BallonPopForce * reflectiveDirection);
        }
    }
}
