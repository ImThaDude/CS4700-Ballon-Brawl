using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScoreTracker
{
	static ScoreTracker() {
		Reset();
	}

	public static int Score {
		get; private set;
	}

	public static void Reset() {
		Score = 0;
	}

	public static void AddPoints(int amount) {
		if(amount > 0) {
			Score += amount;
		}
	}
	
}
