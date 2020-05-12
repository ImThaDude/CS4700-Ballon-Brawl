using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerLifeTracker : MonoBehaviour
{
	[Tooltip("This should be the player in the scene, not a prefab.")]
	public GameObject playerObject;
	[Tooltip("Also disabled this component")]
	public UnityEvent onPlayerDeath;

	private void Update() {
		if(playerObject == null) {
			enabled = false;
			onPlayerDeath.Invoke();
		}
	}
}
