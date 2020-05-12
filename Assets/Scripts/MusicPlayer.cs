using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
	public AudioSource trackStart;
	public AudioSource trackLoop;

	private void OnEnable() {
		float startClipLength = (trackStart.clip != null ? trackStart.clip.length : 0f);

		trackStart.Play();
		trackLoop.PlayDelayed(startClipLength);
	}

	private void OnDisable() {
		trackStart.Stop();
		trackLoop.Stop();
	}
}
