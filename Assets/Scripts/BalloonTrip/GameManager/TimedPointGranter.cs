using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedPointGranter : MonoBehaviour
{
	public float interval = 3f;
	public int pointAmount = 5;

	private float timeOfNextReward = 0f;

    private void OnEnable()
    {
		timeOfNextReward = Time.time + interval;
    }

    void Update()
    {
		while(Time.time > timeOfNextReward) {
			timeOfNextReward += interval;
			ScoreTracker.AddPoints(pointAmount);
		}
    }
}
