using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BalloonTripHazardSpawner : MonoBehaviour
{
	public BoxCollider2D playField;
	public GameObject lightningBallPrefab;

	[Tooltip("Frequency between spawns")]
	public float spawnPeriod;

	private void Awake() {
		Assert.IsNotNull(playField);
		Assert.IsNotNull(lightningBallPrefab);
	}

	private void Start() {
		SpawnLightningBall();
	}

	void Update()
    {
    }

	private void SpawnLightningBall() {
		Vector3 hazardPos = new Vector3(
			playField.bounds.min.x,
			Random.Range(playField.bounds.min.y, playField.bounds.max.y)
		);

		GameObject ball = Instantiate(
			lightningBallPrefab, 
			hazardPos,
			lightningBallPrefab.transform.rotation,
			transform
		);

		ball.GetComponent<LightningBallMover>().playField =
			playField.bounds;
	}
}
