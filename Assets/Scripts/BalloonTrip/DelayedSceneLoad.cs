using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DelayedSceneLoad : MonoBehaviour
{
	[Tooltip("If -1, reload the active scene instead.")]
	public int sceneIndex = -1;
	public float restartDelay = 1f;

	private void Awake() {
		if(sceneIndex == -1) {
			sceneIndex = SceneManager.GetActiveScene().buildIndex;
		}
	}

	private void Start() {
		StartCoroutine(DoLoad());
	}

    private IEnumerator DoLoad()
    {
		yield return new WaitForSeconds(restartDelay);

		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync( sceneIndex );

		// Wait until the asynchronous scene fully loads
		while(!asyncLoad.isDone) {
			yield return null;
		}
	}
}
