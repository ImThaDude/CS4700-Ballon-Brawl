using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BalloonTripHazardSpawner : MonoBehaviour
{
	public Logger logger;
	public BoxCollider2D playField;
	public GameObject lightningBallPrefab;

	public Vector2 objMinInitVelocity = new Vector2(1, -2);
	public Vector2 objMaxInitVelocity = new Vector2(1.5f, 2);

	public float breakDuration = 5f;
	public float waveDuration = 10f;
	public float minSpawnDelay = 0.5f;
	public float maxSpawnDelay = 1.0f;
	public int minSpawnCount = 1;
	public int maxSpawnCount = 2;

	private float waveStartTime = 0f;
	private float nextSpawnTime = 0f;
	private bool IsInWaveCurrently {
		get {
			return Time.time < waveStartTime + waveDuration; 
		}
	}
	private float NextWaveStartTime {
		get {
			return waveStartTime + waveDuration + breakDuration;
		}
	}

	private void Awake() {
		Assert.IsNotNull(playField);
		Assert.IsNotNull(lightningBallPrefab);
		Assert.IsNotNull(lightningBallPrefab.GetComponent<LightningBallMover>());
		Assert.IsNotNull(lightningBallPrefab.GetComponent<Rigidbody2D>());

		if(logger == null) {
			logger = Logger.DefaultLogger;
		}
	}

	private void Start() {
		//SpawnLightningBall();
		waveStartTime = -waveDuration;
	}

	private void Update()
    {
		if(IsInWaveCurrently) {
			if(Time.time > nextSpawnTime) {

				int spawnCount = Random.Range(minSpawnCount, maxSpawnCount + 1);
				for(int i = 0; i < spawnCount; i++) {
					SpawnLightningBall();
				}

				nextSpawnTime = Time.time + Random.Range(minSpawnDelay, maxSpawnDelay);
			}
		}
		else if(Time.time > NextWaveStartTime) {
			waveStartTime = Time.time;
		}
		
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

		ball.GetComponent<Rigidbody2D>().velocity = new Vector2(
			Random.Range(objMinInitVelocity.x, objMaxInitVelocity.x),
			Random.Range(objMinInitVelocity.y, objMaxInitVelocity.y)
		);
	}
}
