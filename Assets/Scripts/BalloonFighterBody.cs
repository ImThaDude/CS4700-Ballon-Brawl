using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

/// <summary>
/// This handles the mechanics for the balloon fighter's movement.
/// It handles movement, flapping, jumping, etc.
///
/// An outside script/component must fire off the functions
/// to control what happens.
/// </summary>
public class BalloonFighterBody : MonoBehaviour
{
	public Rigidbody2D rb;
	public Animator anim;

	[Space(10)]
	[Tooltip("Used when jump is hit while airborne.")]
	public float flapImpulse = 10f;
	[Tooltip("Used when jump is hit while grounded.")]
	public float jumpImpulse = 4f;
	[Tooltip("Maximum speed while on the ground.")]
	public float groundMovementSpeed = 4f;
	
	[Space(10)]
	public float groundCastLength = 0.5f;
	public LayerMask groundMask;

	private float moveAmount;
	private bool isJumping;
	
	//George invasion variables.........
	private bool canFly = true;
	public bool isFloating = false;
	public float floatVelocity = 2f;
	public float floatClampLerp = 0.1f;
	public bool hasFainted = false;
	public bool isIdle = false;
	public AudioClip jumpAudioClip;
	//-----------------------------------
	
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

	/// <summary>
	/// Move the character horizontally. Note that if we are in the
	/// air and not flapping, we won't move at all.
	///
	/// This should get called every frame.
	/// </summary>
	/// <param name="context"></param>
	public void MoveHorizontal(InputAction.CallbackContext context) {
		MoveHorizontal(context.ReadValue<float>());
	}

	/// <summary>
	/// Move the character horizontally. Note that if we are in the
	/// air and not flapping, we won't move at all.
	/// 
	/// This should get called every frame.
	/// </summary>
	/// <param name="direction">
	/// -1 is fastest to the left, and 1 is fastest to the right.
	/// </param>
	public void MoveHorizontal(float direction) {
		moveAmount = direction;
	}

	/// <summary>
	/// Jumps/flaps, gaining altitude.
	/// </summary>
	/// <param name="context"></param>
	public void Jump(InputAction.CallbackContext context) {
		if(context.performed) {
			Jump();
		}
	}

	//George invation code...
	public float faintImpulseJump = 100f;

	public void SetFly(bool canFly) {
		this.canFly = canFly;
	}

	public void SetFloat(bool isFloating) {
		this.isFloating = isFloating;
	}

	public void Drop() {
		rb.velocity = Vector3.zero;
		SetFly(false);
	}

	public void Faint() {
		SetFloat(false);
		Drop();
		rb.AddForce(Vector2.up * faintImpulseJump);
		hasFainted = true;
	}
	//------------------------

	/// <summary>
	/// Jumps/flaps, gaining altitude.
	/// </summary>
	public void Jump() {
		isJumping = true;
	}

	private void Awake() {
		Assert.IsNotNull(rb);
		Assert.IsNotNull(anim);
	}

	private void OnEnable() {
		ResetControlVars();
	}

	private void Update()
	{
		//Vector2 dir = new Vector2(moveAmount, 0);
		
		//More george invasion of the code
		if (isFloating) {
			Vector2 v = rb.velocity;
			v.y = Mathf.Lerp (v.y, -floatVelocity, floatClampLerp);
			rb.velocity = v;
		}

		if (IsGrounded) {
			isFloating = false;

			if (hasFainted) {
				//Destroy(gameObject);
				hasFainted = false;
			}

			if (moveAmount == 0) {
				isIdle = true;
			} else {
				isIdle = false;
			}
		} else {
			isIdle = false;
		}
		//-------------------------------

		if(IsGrounded) {

			if(isJumping) {
				rb.AddForce(Vector2.up * jumpImpulse);

				//Some more George invasion.
				AudioSource.PlayClipAtPoint(jumpAudioClip, transform.position);
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
		else if(isJumping && canFly) {
			Vector2 dir = new Vector2(moveAmount, 1).normalized;
			rb.AddForce(dir * flapImpulse);

			anim.SetTrigger("Flap");

			//Some more George invasion.
			AudioSource.PlayClipAtPoint(jumpAudioClip, transform.position);
		}
	
		anim.SetFloat("Movement", Mathf.Abs(moveAmount));
		anim.SetBool("IsGrounded", IsGrounded);
		if (moveAmount != 0) {
			anim.SetFloat("Dir", moveAmount);
		}

		ResetControlVars();
	}

	/// <summary>
	/// Reverts whatever control inputs were given.
	/// </summary>
	private void ResetControlVars() {
		//dir = Vector2.zero;
		isJumping = false;
	}
}
