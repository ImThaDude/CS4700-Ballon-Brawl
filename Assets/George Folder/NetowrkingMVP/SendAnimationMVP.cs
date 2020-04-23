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

    void Start()
    {
        anim = GetComponent<Animator>();
        state = new AnimationState();
    }

    public void UpdateHP(float Health)
    {
        if (state != null)
        {
            anim.SetFloat("HP", Health);
            state.HP = Health;
        }
    }

    public void UpdateIsGrounded(bool IsGrounded)
    {
        if (state != null)
        {
            anim.SetBool("IsGrounded", IsGrounded);
            state.IsGrounded = IsGrounded;
        }
    }

    public void UpdateMovement(float moveAmount)
    {
        if (state != null)
        {
            anim.SetFloat("Movement", Mathf.Abs(moveAmount));
            state.Movement = moveAmount;
        }
    }

    public void UpdateDir(float moveAmount)
    {
        if (state != null)
        {
            anim.SetFloat("Dir", moveAmount);
            state.Dir = moveAmount;
        }
    }

    public void UpdateFlap(bool Flap)
    {
        if (state != null)
        {
            anim.SetTrigger("Flap");
            state.Flap = !state.Flap;
        }
    }

    public void UpdatePumpProgress(float PumpProgress)
    {
        if (state != null)
        {
            anim.SetFloat("PumpProgress", PumpProgress);
            state.PumpProgress = PumpProgress;
        }
    }

    void Update()
    {
        if (state != null)
        {
            client.SendAnimation(state.HP, state.IsGrounded, state.Movement, state.Dir, state.Flap, state.PumpProgress);
        }
    }

}
