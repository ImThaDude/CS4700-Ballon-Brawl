using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BalloonFighterBody : MonoBehaviour
{
	public Rigidbody2D rb;
	public Animator anim;

	public float flapImpulse = 10f;
	
	public float groundCastLength = 0.5f;
	public LayerMask groundMask;
	public float groundMovementSpeed = 4f;
	public float jumpImpulse = 4f;
	// TODO Create a jump impulse

	private float moveAmount;
	private bool isJumping;
	
	public bool IsGrounded {
		get {
			return Physics2D.Raycast(
				rb.transform.position,
				Vector2.down,
				groundCastLength,
				groundMask
			);
		}
	}

	public void MoveHorizontal(InputAction.CallbackContext context) {
		MoveHorizontal(context.ReadValue<float>());
	}

	public void MoveHorizontal(float direction) {
		moveAmount = direction;

		//Debug.Log(direction);
	}

	public void Jump(InputAction.CallbackContext context) {
		if(context.performed) {
			Jump();
		}
	}

	public void Jump() {
		isJumping = true;

		//Debug.Log("Jumping");
	}

	private void OnEnable() {
		ResetControlVars();
	}

	private void Update()
	{
		//Vector2 dir = new Vector2(moveAmount, 0);

		if(isJumping) {
			Debug.Log("On jump, is on ground? " + IsGrounded);
		}

		if(IsGrounded) {

			if(isJumping) {
				rb.AddForce(Vector2.up * jumpImpulse);
			}
			else {
				//rb.velocity = dir * groundMovementSpeed;
				rb.velocity = new Vector2(
					//dir.x * groundMovementSpeed,
					moveAmount * groundMovementSpeed,
					rb.velocity.y
				);

			}
		}
		else if(isJumping) {
			Vector2 dir = new Vector2(moveAmount, 1).normalized;
			rb.AddForce(dir * flapImpulse);

			anim.SetTrigger("Flap");
		}

		// TODO add animation stuff
		anim.SetFloat("Movement", Mathf.Abs(moveAmount));
		anim.SetBool("IsGrounded", IsGrounded);
		anim.SetFloat("Dir", moveAmount);

		ResetControlVars();
	}

	private void ResetControlVars() {
		//dir = Vector2.zero;
		isJumping = false;
	}
}
