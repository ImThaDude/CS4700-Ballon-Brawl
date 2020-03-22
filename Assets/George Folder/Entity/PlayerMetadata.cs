﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMetadata : MonoBehaviour
{

    public int _Health = 3;
    public int Health
    {
        set
        {
            _Health = value;
            if (Body != null && anim != null)
            {
                anim.SetFloat("HP", Health);

                if (Health < -0.1) {
                    Destroy(transform);
                }

                if (Health < 0.5)
                {
                    Body.SetFly(false);
                }
                else
                {
                    Body.SetFly(true);
                }
            }
        }
        get {
            return _Health;
        }
    }
    //Experimental
    public float SpeedBuff = 1;

    public Animator anim;

    public BalloonFighterBody Body;

    public void Damage() {
        Health--;
    }
}