using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BalloonTripElement))]
public class LightningBallMover : MonoBehaviour
{
	private Rigidbody2D rb;
	private BalloonTripElement btElem;

	private void Awake() {
		rb = GetComponent<Rigidbody2D>();
		btElem = GetComponent<BalloonTripElement>();

		Assert.IsNotNull(rb);
		Assert.IsNotNull(btElem);
	}

	private void OnEnable()
    {
		//rb.velocity = initVelocity;
    }

    private void Update()
    {
		if(transform.position.y > btElem.PlayField.max.y || 
		   transform.position.y < btElem.PlayField.min.y) {

			rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
		}
    }
}
