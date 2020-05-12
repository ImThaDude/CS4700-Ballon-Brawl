using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BalloonTripElement : MonoBehaviour
{
	public Bounds PlayField {
		get; private set;
	}

	public void Init(Bounds pf, Vector2 initVelocity) {
		GetComponent<Rigidbody2D>().velocity = initVelocity;
		PlayField = pf;
	}


    void Update()
    {
		if(transform.position.x < PlayField.min.x) {
			Destroy(gameObject);
		}
    }
}
