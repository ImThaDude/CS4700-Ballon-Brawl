using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendAnimationMVP : MonoBehaviour
{
    public class AnimationState
    {
        //All animations will go here and will get updated
        public float HP;
        public bool IsGrounded;
        public float Movement;
        public float Dir;
        public bool Flap;
        public float PumpProgress;
    }

    public Animator anim;
    public AnimationState state;
    public ClientNetworkManagerMVP client;
    public bool Initialized = false;
    public bool UpdateQueued = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        state = new AnimationState();
    }

    public void UpdateHP(float Health)
    {
        if (state != null)
        {
            if (state.HP != Health)
            {
                anim.SetFloat("HP", Health);
                state.HP = Health;
                UpdateQueued = true;
            }
        }
    }

    public void UpdateIsGrounded(bool IsGrounded)
    {
        if (state != null)
        {
            if (state.IsGrounded != IsGrounded)
            {
                anim.SetBool("IsGrounded", IsGrounded);
                state.IsGrounded = IsGrounded;
                UpdateQueued = true;
            }
        }
    }

    public void UpdateMovement(float moveAmount)
    {
        if (state != null)
        {
            if (state.Movement != moveAmount)
            {
                anim.SetFloat("Movement", Mathf.Abs(moveAmount));
                state.Movement = moveAmount;
                UpdateQueued = true;
            }
        }
    }

    public void UpdateDir(float moveAmount)
    {
        if (state != null)
        {
            if (state.Dir != moveAmount)
            {
                anim.SetFloat("Dir", moveAmount);
                state.Dir = moveAmount;
                UpdateQueued = true;
            }
        }
    }

    public void UpdateFlap(bool Flap)
    {
        if (state != null)
        {
            anim.SetTrigger("Flap");
            state.Flap = !state.Flap;
            UpdateQueued = true;
        }
    }

    public void UpdatePumpProgress(float PumpProgress)
    {
        if (state != null)
        {
            if (state.PumpProgress != PumpProgress) {
                anim.SetFloat("PumpProgress", PumpProgress);
                state.PumpProgress = PumpProgress;
                UpdateQueued = true;
            }
        }
    }

    void Update()
    {
        if (state != null)
        {
            if (UpdateQueued || !Initialized) {
                client.SendAnimation(state.HP, state.IsGrounded, state.Movement, state.Dir, state.Flap, state.PumpProgress);
                UpdateQueued = false;
                Initialized = true;
            }
        }
    }

}
