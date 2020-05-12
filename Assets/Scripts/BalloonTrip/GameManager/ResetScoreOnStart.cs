using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetScoreOnStart : MonoBehaviour
{
    private void Start() {
		ScoreTracker.Reset();
    }

}
