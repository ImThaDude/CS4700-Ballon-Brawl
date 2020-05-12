using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Assertions;

public class ScoreDisplay : MonoBehaviour
{
	public string prefix = "Score: ";
	public int digitCount = 7;

	private TMP_Text textObj;

	private void Awake() {
		textObj = GetComponent<TMP_Text>();
		Assert.IsNotNull(textObj);
	}

    private void OnGUI() {
		textObj.text = prefix + ScoreTracker.Score.ToString("D" + digitCount);
    }
}
