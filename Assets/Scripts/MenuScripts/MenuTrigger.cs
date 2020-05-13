using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuTrigger : MonoBehaviour
{
	public Text labelObj;
	public string label;
	public int targetSceneIndex;
	public int countDown = 3;
	public bool quitInsteadOfLoad = false;

	private float triggerStartTime;
	private bool triggered = false;

	private void OnTriggerEnter2D(Collider2D other) {
		triggered = true;
		triggerStartTime = Time.time;

		labelObj.color = Color.green;
	}

	private void OnTriggerExit2D(Collider2D other) {
		triggered = false;

		labelObj.text = label;
		labelObj.color = Color.white;
	}
	

    private void Update() {
		if(triggered) {
			int secondsSinceTriggerStart = Mathf.FloorToInt(Time.time - triggerStartTime);
			int secondsToTrigger = countDown - secondsSinceTriggerStart;
			
			labelObj.text = label + "(" + secondsToTrigger + ")";

			if(secondsToTrigger <= 0) {
				if(quitInsteadOfLoad) {
					Application.Quit();
				}
				else {
					SceneManager.LoadScene(targetSceneIndex);
				}
			}
		}
    }
}
