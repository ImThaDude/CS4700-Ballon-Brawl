using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DelayedRestart : MonoBehaviour
{

	public float restartDelay = 1f;

	private void Start() {
		StartCoroutine(DoRestart());
	}

    private IEnumerator DoRestart()
    {
		yield return new WaitForSeconds(restartDelay);

		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(
			SceneManager.GetActiveScene().buildIndex
		);

		// Wait until the asynchronous scene fully loads
		while(!asyncLoad.isDone) {
			yield return null;
		}
	}
}
