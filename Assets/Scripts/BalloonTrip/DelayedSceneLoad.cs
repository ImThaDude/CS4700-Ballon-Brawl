using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DelayedSceneLoad : MonoBehaviour
{
	public int sceneIndex = 0;
	public float restartDelay = 1f;

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
