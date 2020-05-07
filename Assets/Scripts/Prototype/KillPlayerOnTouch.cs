using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype {
	public class KillPlayerOnTouch : MonoBehaviour {
		private void OnTriggerEnter2D(Collider2D other) {
			BalloonFighterHealth otherHealth =
				other.GetComponent<BalloonFighterHealth>();

			if(otherHealth != null) {
				otherHealth.Health = -1;
			}
		}
	}
}
