using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMetadata : MonoBehaviour
{

    public int _Health = 2;
    public int Health
    {
        set
        {
            RenderHealth(value);
        }
        get
        {
            return _Health;
        }
    }
    //Experimental
    public float SpeedBuff = 1;

    public Animator anim;

    public BalloonFighterBody Body;

    public AudioClip BalloonPop;
    public AudioClip BalloonRecover;

    public float OneBalloonRecoveryTime = 15f;
    float currBallonRecoveryTime = 0;

    public void Damage()
    {
        if (BalloonPop != null) {
            AudioSource.PlayClipAtPoint(BalloonPop, transform.position);
        } else {
            CSLogger.L("BalloonPop clip as not been assigned.");
        }
        Health--;
    }

    public void RestoreBalloon() {
        if (BalloonRecover != null) {
            AudioSource.PlayClipAtPoint(BalloonRecover, transform.position);
        } else {
            CSLogger.L("BalloonRecover clip as not been assigned.");
        }
        Health++;
    }

    void RenderHealth(int hp)
    {
        _Health = hp;
        if (Body != null && anim != null)
        {
            anim.SetFloat("HP", Health);

            if (Health < -0.1)
            {
                //Destroy(transform.gameObject);
                Body.Faint();
            } else if (Health < 0.5)
            {
                Body.Drop();
                Body.SetFloat(true);
            }
            else
            {
                Body.SetFly(true);
                Body.SetFloat(false);
            }
        }
        else
        {
            CSLogger.L("Cannot find BalloonFighterBody or Animator");
        }
    }

    public bool DamageTrigger = false;

    void Start()
    {
        RenderHealth(_Health);
    }

    void Update()
    {
        if (DamageTrigger)
        {
            Damage();
            DamageTrigger = false;
        }

        if (Body.isIdle) {
            if (Health < 0.5f) {
                
                //Counter for idle time
                currBallonRecoveryTime += Time.deltaTime;
                
                //If fulfilled add a balloon
                if (currBallonRecoveryTime > OneBalloonRecoveryTime) {
                    RestoreBalloon();
                    currBallonRecoveryTime = 0;
                }

                
            } else {
                currBallonRecoveryTime = 0;
            }
        } else {
            currBallonRecoveryTime = 0;
        }

        //Show on animation
        anim.SetFloat("PumpProgress", (currBallonRecoveryTime / OneBalloonRecoveryTime) * 100);
        
    }
}
