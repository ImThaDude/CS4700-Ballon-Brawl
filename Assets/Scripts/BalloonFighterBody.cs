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

    [Header("Jump tuning")]
    [Tooltip("Used when jump is hit while grounded.")]
    public float jumpImpulse = 4f;
    [Tooltip("If we are holding jump, this is number of seconds " +
             "before autoflap begins.")]
    public float jumpToAutoFlapDelay = 1.0f;

    [Header("Flap tuning")]
    [Tooltip("Used when jump is hit while airborne.")]
    public float flapImpulse = 10f;
    [Tooltip("Time between flaps (in seconds) while jump is held.")]
    [Range(0f, 2f)] public float autoFlapPeriod = 0.5f;
    [Tooltip("This controls how much the player can influence the " +
             "left/right movement. Higher numbers makes it feel " +
             "more stiff, but low numbers make it so that the " +
             "player will fall more when trying to move and flap.")]
    [Range(0.01f, 5f)] public float upwardFlapBias = 1f;

    [Header("Standing tuning")]
    [Tooltip("Maximum speed while on the ground.")]
    public float groundMovementSpeed = 4f;
    [Space(10)]
    public float groundCastLength = 0.5f;
    public LayerMask groundMask;

    private float moveAmount;
    private bool isJumping;
    private float timeOfNextAutoflap = 0f;

    //George invasion variables.........
    private bool canFly = true;
    public bool isFloating = false;
    public float floatVelocity = 2f;
    public float floatClampLerp = 0.1f;
    public bool hasFainted = false;
    public bool isIdle = false;
    public AudioClip jumpAudioClip;

    public SendAnimationMVP animMVP;

    public Transform[] GroundPositionRays;
    //-----------------------------------

    public bool IsGrounded
    {
        get
        {
            foreach (var a in GroundPositionRays)
            {
                if (Physics2D.Raycast(a.position, Vector2.down, groundCastLength, groundMask)) {
                    return true;
                }
            }
            return false;
        }
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
    public void SetJump(bool nowJumping = true)
    {
        isJumping = nowJumping;

        if (isJumping)
        {
            // If we're in the air, we want to immediately flap.
            // Thus, the next (auto) flap will happen immediately.
            timeOfNextAutoflap = Time.time;
        }
    }

    //George invation code...
    public float faintImpulseJump = 100f;

    public void SetFly(bool canFly)
    {
        this.canFly = canFly;
    }

    public void SetFloat(bool isFloating)
    {
        this.isFloating = isFloating;
    }

    public void Drop()
    {
        rb.velocity = Vector3.zero;
        SetFly(false);
    }

    public void Faint()
    {
        SetFloat(false);
        Drop();
        rb.AddForce(Vector2.up * faintImpulseJump);
        hasFainted = true;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
    //------------------------

    private void Awake()
    {
        Assert.IsNotNull(rb);
        Assert.IsNotNull(anim);
    }

    private void OnEnable()
    {
        ResetControlVars();
    }

    private void Update()
    {
        //Vector2 dir = new Vector2(moveAmount, 0);

        //More george invasion of the code
        if (isFloating)
        {
            Vector2 v = rb.velocity;
            v = Vector3.Lerp(v, Vector2.down * floatVelocity, floatClampLerp);
            rb.velocity = v;
        }

        if (IsGrounded)
        {
            isFloating = false;

            if (hasFainted)
            {
                //Destroy(gameObject);
                hasFainted = false;
            }

            if (moveAmount == 0)
            {
                isIdle = true;
            }
            else
            {
                isIdle = false;
            }
        }
        else
        {
            isIdle = false;
        }
        //-------------------------------

        /*
		if(IsGrounded) {

			if(isJumping && Time.time >= timeOfNextAutoflap) {
				rb.AddForce(Vector2.up * jumpImpulse);

				timeOfNextAutoflap = Time.time + jumpToAutoFlapDelay;
			}
			else {
				rb.velocity = new Vector2(
					moveAmount * groundMovementSpeed,
					rb.velocity.y
				);
			}
		}
		else if(isJumping && Time.time >= timeOfNextAutoflap) {

			Vector2 dir = new Vector2(moveAmount, 1).normalized;
			rb.AddForce(dir * flapImpulse);

			anim.SetTrigger("Flap");

			timeOfNextAutoflap = Time.time + autoFlapPeriod;
		}
		*/


        if (isJumping && Time.time >= timeOfNextAutoflap)
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
                rb.AddForce(dir * flapImpulse);
                timeOfNextAutoflap = Time.time + autoFlapPeriod;

                //Some more George invasion.
                AudioSource.PlayClipAtPoint(jumpAudioClip, transform.position);

                //anim.SetTrigger("Flap");
                animMVP.UpdateFlap(true);
            }
        }
        else if (IsGrounded)
        {
            rb.velocity = new Vector2(
                moveAmount * groundMovementSpeed,
                rb.velocity.y
            );
        }

        //anim.SetFloat("Movement", Mathf.Abs(moveAmount));
        animMVP.UpdateMovement(Mathf.Abs(moveAmount));
        //anim.SetBool("IsGrounded", IsGrounded);
        animMVP.UpdateIsGrounded(IsGrounded);
        //anim.SetFloat("Dir", moveAmount);
        animMVP.UpdateDir(moveAmount);

        ResetControlVars();
    }

    /// <summary>
    /// Reverts whatever control inputs were given.
    /// </summary>
    private void ResetControlVars()
    {
        //dir = Vector2.zero;
        //isJumping = false;
    }
}
