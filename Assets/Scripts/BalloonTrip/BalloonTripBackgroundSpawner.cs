using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BalloonTripBackgroundSpawner : MonoBehaviour
{
	public BoxCollider2D playField;

	public GameObject starPrefab;
	public Vector2 starVelocity;
	public float minSpawnPeriod = 0.05f;
	public float maxSpawnPeriod = 0.5f;

	[Space(10)]
	public bool prepopulate = true;
	public float prepopulateCount = 50;

	private float nextSpawnTime = 0f;

	private void Awake() {
		Assert.IsNotNull(playField);
	}

	private void Start() {
		if(prepopulate) {
			for(int i = 0; i < prepopulateCount; i++) {
				SpawnStar(new Vector3(
					Random.Range(playField.bounds.min.x, playField.bounds.max.x),
					Random.Range(playField.bounds.min.y, playField.bounds.max.y)
				));
			}
		}
	}

    private void Update()
    {
		if(Time.time > nextSpawnTime) {

			SpawnStar();

			nextSpawnTime = Time.time + Random.Range(minSpawnPeriod, maxSpawnPeriod);
		}
    }

	private void SpawnStar() {
		SpawnStar(new Vector3(
			playField.bounds.max.x,
			Random.Range(playField.bounds.min.y, playField.bounds.max.y)
		));
	}

	private void SpawnStar(Vector3 pos) {


		GameObject star = Instantiate(
			starPrefab,
			pos,
			starPrefab.transform.rotation,
			transform
		);

		star.GetComponent<BalloonTripElement>().Init( playField.bounds, starVelocity );
	}
}
