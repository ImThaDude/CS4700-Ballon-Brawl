using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SpawnPlatformController : MonoBehaviour
{
	public Vector2 initVelocity;
	public Transform unparentOnDestroy;

	private void Start() {
		GetComponent<Rigidbody2D>().velocity = initVelocity;
	}

	private void OnCollisionExit2D(Collision2D other) {
		//unparentOnDestroy.parent = null;
		Destroy(gameObject);
	}
}
