using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class ForceAssignKeyboard : MonoBehaviour
{
	//public PlayerInput input;
	//public string schemeName;

	private void Awake() {
		PlayerInput input = GetComponent<PlayerInput>();

		if(input.defaultControlScheme.Contains("Keyboard")) {
			Debug.Log(gameObject.name + ": Using keyboard hack. Should be removed spawning players with code.");
			input.SwitchCurrentControlScheme( input.defaultControlScheme, Keyboard.current );
		}
	}
}
