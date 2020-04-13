using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
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
    public AudioClip jumpAudioClip;

    [Header("Jumping")]
    [Tooltip("Used when jump is hit while grounded.")]
    public float jumpImpulse = 4f;
    [Tooltip("If we are holding jump, this is number of seconds " +
             "before autoflap begins.")]
    public float jumpToAutoFlapDelay = 1.0f;

    [Header("Flapping")]
    [Tooltip("Used when jump is pressed while airborne.")]
    public float flapImpulse = 10f;
    [Tooltip("Used when jump is held while airborne.")]
    public float autoFlapImpulse = 10f;
    [Tooltip("Time between flaps (in seconds) while jump is held.")]
    [Range(0f, 2f)] public float autoFlapPeriod = 0.5f;
    [Tooltip("This controls how much the player can influence the " +
             "left/right movement. Higher numbers makes it feel " +
             "more stiff, but low numbers make it so that the " +
             "player will fall more when trying to move and flap.")]
    [Range(0.01f, 5f)] public float upwardFlapBias = 1f;

	//public bool isFloating = false;

	[Header("Parachuting")]
    public float parachuteVelocity = 2f;
    public float parachuteClampLerp = 0.1f;
    //public bool isIdle = false;

	// TODO These should get removed and we should make a death object
    public bool hasFainted = false;
    public float faintImpulseJump = 100f;

    [Header("Standing")]
    [Tooltip("Maximum speed while on the ground.")]
    public float groundMovementSpeed = 4f;
    [Space(10)]
    public float groundCastLength = 0.5f;
    public LayerMask groundMask;

	/// <summary>
	/// Used to track how much the player wants to move left/right.
	/// </summary>
    private float moveAmount;
	/// <summary>
	/// True as long as our intent is to go up.
	/// </summary>
    private bool isJumpHeld;
	/// <summary>
	/// Tracks the next time we would do an automatic flap, in any.
	/// Note that a time of 0 means that the next flap is NOT an
	/// autoflap; it's a flap triggered by a recent button press.
	/// </summary>
    private float timeOfNextAutoflap = 0f;
    [SerializeField] private bool canFly = true;

	#region State tracking
	public enum State {
		/// <summary>
		/// We're holding still on the ground. We may or may not
		/// be able to fly.
		/// </summary>
		Idle,

		/// <summary> 
		/// We're moving around on the ground. We may or may not
		/// be able to fly.
		/// </summary>
		Walking,

		/// <summary>
		/// We're currently flying. This is used both when just flying
		/// and when flapping.
		/// </summary>
		Flying,

		/// <summary>
		/// We are in the air but cannot fly, and we are for some
		/// reason in the parachute state.
		/// </summary>
		Parachuting,

		/// <summary>
		/// We are in the air but cannot fly. This is the default
		/// state for this situation, but we could also be
		/// parachuting. Thus, it may be smart to check both for this
		/// and for parachuting.
		/// </summary>
		Dropping
	}

	/// <summary>
	/// This is whatever state we are currently in. Note that this
	/// will get updated in Update as well as through various
	/// function calls.
	/// </summary>
	public State CurrentState {
		get; private set;
	}

	/// <summary>
	/// This is largely used by Update to figure out what state
	/// we're currently in. It'll make its best guess as to what
	/// state we're currently in.
	/// </summary>
	/// <returns>The state which it looks like we're in.</returns>
	private State DeduceCurrentState() {
		if(IsGrounded) {
			if(Mathf.Approximately(moveAmount, 0f)) {
				return State.Idle;
			}
			else {
				return State.Walking;
			}
		}
		else {
			if(canFly) {
				return State.Flying;
			}
			else if(CurrentState == State.Parachuting) {
				return State.Parachuting;
			}
			else {
				return State.Dropping;
			}
		}
	}
	#endregion

    public bool IsGrounded
    {
        get
        {
            return Physics2D.Raycast(
                rb.transform.position,
                Vector2.down,
                groundCastLength,
                groundMask
            );
        }
    }


	#region Inputs
	/// <summary>
	/// Move the character horizontally. Note that if we are in the
	/// air and not flapping, we won't move at all.
	/// 
	/// This should get called every frame.
	/// </summary>
	/// <param name="direction">
	/// -1 is fastest to the left, and 1 is fastest to the right.
	/// </param>
	public void MoveHorizontal(float direction)
    {
        moveAmount = direction;
    }

    /// <summary>
    /// Jumps/flaps, gaining altitude.
    /// </summary>
    /// <param name="nowJumping">
    /// If true, we are going to be jumping/flapping. When false,
    /// we're done gaining altitude.
    /// </param>
    public void SetJumpIntent(bool nowJumping = true)
    {
        isJumpHeld = nowJumping;

        if (nowJumping)
        {
			// If we're in the air, we want to immediately flap.
			// Thus, the next (auto) flap will happen immediately.
			timeOfNextAutoflap = 0;
        }
    }
	#endregion

	//George invation code...

	public void EnableFlight(bool canFly = true)
    {
		Debug.Log("Set fly " + canFly);
        this.canFly = canFly;

		CurrentState = DeduceCurrentState();
    }

	public void DisableFlight() {
		EnableFlight(false);
	}

    public void BeginParachuting()
    {
		if(!IsGrounded && !canFly) {
			CurrentState = State.Parachuting;
		}
    }

    public void Drop()
    {
		if(!IsGrounded && !canFly) {
			CurrentState = State.Dropping;
			//rb.velocity = Vector3.zero;
			//EnableFlight(false);
		}
    }

    public void Faint()
    {
		Debug.Log("Faint");
        //BeginParachuting(false);
        Drop();
        rb.AddForce(Vector2.up * faintImpulseJump);
        hasFainted = true;
    }
    //------------------------

    private void Awake()
    {
        Assert.IsNotNull(rb);
        Assert.IsNotNull(anim);
    }

    private void Update()
    {
		//Vector2 dir = new Vector2(moveAmount, 0);

		//State oldState = CurrentState;
		CurrentState = DeduceCurrentState();

        //More george invasion of the code
        if (CurrentState == State.Parachuting)
        {
            Vector2 v = rb.velocity;
            v.y = Mathf.Lerp(v.y, -parachuteVelocity, parachuteClampLerp);
            rb.velocity = v;
        }
		else if(isJumpHeld && Time.time >= timeOfNextAutoflap)
        {

            if (IsGrounded)
            {
                rb.AddForce(Vector2.up * jumpImpulse);
                timeOfNextAutoflap = Time.time + jumpToAutoFlapDelay;

                //Some more George invasion.
                AudioSource.PlayClipAtPoint(jumpAudioClip, transform.position);
            }
            //canFly is George invasion variable
            else if (canFly)
            {

                Vector2 dir = new Vector2(
                    moveAmount, upwardFlapBias).normalized;

				// Note that the flap force changes if we held the
				// button vs if we are rapidly mashing.
				if(Mathf.Approximately(timeOfNextAutoflap, 0f)) {
					rb.AddForce(dir * flapImpulse);
				}
				else {
					rb.AddForce(dir * autoFlapImpulse);
				}

                timeOfNextAutoflap = Time.time + autoFlapPeriod;

                //Some more George invasion.
                AudioSource.PlayClipAtPoint(jumpAudioClip, transform.position);

                anim.SetTrigger("Flap");
            }
        }
        else if (IsGrounded)
        {
            rb.velocity = new Vector2(
                moveAmount * groundMovementSpeed,
                rb.velocity.y
            );
        }

        anim.SetFloat("Movement", Mathf.Abs(moveAmount));
        anim.SetBool("IsGrounded", IsGrounded);
        anim.SetFloat("Dir", moveAmount);

    }
}
