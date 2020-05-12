using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

/// <summary>
/// This is the interface between the PlayerInput component and
/// the balloon fighter body.
/// </summary>
public class BalloonFighterPlayerBrain : MonoBehaviour
{
	public BalloonFighterBody body;

	private void Awake() {
		Assert.IsNotNull(body);
	}

	/// <summary>
	/// Move the character horizontally. Note that if we are in the
	/// air and not flapping, we won't move at all.
	///
	/// This should get called every frame.
	/// </summary>
	/// <param name="context"></param>
	public void MoveHorizontal(InputAction.CallbackContext context) {
		body.MoveHorizontal(context.ReadValue<float>());
	}

	/// <summary>
	/// Jumps/flaps, gaining altitude.
	/// </summary>
	/// <param name="context"></param>
	public void Jump(InputAction.CallbackContext context) {
		if(context.performed) {
			body.SetJump(true);
		}
		else if(context.canceled) {
			body.SetJump(false);
		}
	}
}
