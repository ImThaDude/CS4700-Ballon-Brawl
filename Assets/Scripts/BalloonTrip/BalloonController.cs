using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonController : MonoBehaviour
{
    public AudioClip popSound;

	private void OnTriggerEnter2D(Collider2D other) {
		AudioSource.PlayClipAtPoint(popSound, transform.position);
		// TODO Give points

		Destroy(gameObject);
	}
}
