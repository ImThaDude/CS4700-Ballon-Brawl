using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class LightningBallMover : MonoBehaviour
{
	// This is really, *really* shoddy. We should be using triggers
	// or whatnot to markout the zone, but for now, I'll just have
	// to give you some floats to track the areas.
	public Bounds playField;
	public Vector2 initVelocity;


	private Rigidbody2D rb;

	private void Awake() {
		rb = GetComponent<Rigidbody2D>();

		Assert.IsNotNull(rb);
	}

	private void OnEnable()
    {
		rb.velocity = initVelocity;
    }

    private void Update()
    {
		if(transform.position.x > playField.max.x) {
			Destroy(gameObject);
		}

		if(transform.position.y > playField.max.y || transform.position.y < playField.min.y) {
			rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
		}
    }
}
