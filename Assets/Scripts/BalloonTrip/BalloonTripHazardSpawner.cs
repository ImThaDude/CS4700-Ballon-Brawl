using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BalloonTripHazardSpawner : MonoBehaviour
{
	public Logger logger;
	public BoxCollider2D playField;
	public GameObject lightningBallPrefab;
	public GameObject balloonPrefab;

	[Header("Wave settings")]
	public Vector2 objMinInitVelocity = new Vector2(1, -2);
	public Vector2 objMaxInitVelocity = new Vector2(1.5f, 2);

	[Space(10)]
	public float breakDuration = 5f;
	public float waveDuration = 10f;
	public float minSpawnDelay = 0.5f;
	public float maxSpawnDelay = 1.0f;
	public int minSpawnCount = 1;
	public int maxSpawnCount = 3;

	[Space(10)]
	[Range(0f, 1f)]
	public float balloonSpawnProbability = 0.25f;

	[Space(10)]
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

				if(Random.Range(0f, 1f) <= balloonSpawnProbability) {
					SpawnBalloon();
				}

				nextSpawnTime = Time.time + Random.Range(minSpawnDelay, maxSpawnDelay);
			}
		}
		else if(Time.time > NextWaveStartTime) {
			waveStartTime = Time.time;
		}
		
    }

	private void SpawnLightningBall() {

		GameObject ball = Instantiate(
			lightningBallPrefab, 
			GetRandomSpawnPos(),
			lightningBallPrefab.transform.rotation,
			transform
		);

		ball.GetComponent<BalloonTripElement>().Init(
			playField.bounds,
			new Vector2(
				Random.Range(objMinInitVelocity.x, objMaxInitVelocity.x),
				Random.Range(objMinInitVelocity.y, objMaxInitVelocity.y)
			)
		);
	}

	private void SpawnBalloon() {
		
		GameObject balloon = Instantiate(
			balloonPrefab, 
			GetRandomSpawnPos(),
			lightningBallPrefab.transform.rotation,
			transform
		);

		balloon.GetComponent<BalloonTripElement>().Init(
			playField.bounds,
			new Vector2( objMinInitVelocity.x, 0f )
		);
	}

	private Vector3 GetRandomSpawnPos() {

		return new Vector3(
			playField.bounds.max.x,
			Random.Range(playField.bounds.min.y, playField.bounds.max.y)
		);
	}
}
