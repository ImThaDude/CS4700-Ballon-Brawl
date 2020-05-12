using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonController : MonoBehaviour
{
    public AudioClip popSound;
	public int points = 100;

	private void OnTriggerEnter2D(Collider2D other) {
		AudioSource.PlayClipAtPoint(popSound, transform.position);

		ScoreTracker.AddPoints(points);

		Destroy(gameObject);
	}
}
