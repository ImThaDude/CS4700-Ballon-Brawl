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

    public GameObject[] HitBoxes;

    public SendAnimationMVP animMVP;

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
            //anim.SetFloat("HP", Health);
            animMVP.UpdateHP(Health);

            if (Health < -0.1)
            {
                //Destroy(transform.gameObject);
                Body.Faint();

                foreach (var box in HitBoxes) {
                    box.SetActive(false);
                }

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

    public float faintTimer = 5f;

    void Start()
    {
        RenderHealth(_Health);
    }

    void Update()
    {
        RenderHealth(Health);

        if (DamageTrigger)
        {
            Damage();
            DamageTrigger = false;
        }

        if (Body.isIdle) {
            if (Health < 0.5f && Health > -0.5f) {
                
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

        if (Health < -0.5f) {
            faintTimer -= Time.deltaTime;
            if (faintTimer < 0) {
                Destroy(gameObject);
            }
        }

        //Show on animation
        //anim.SetFloat("PumpProgress", (currBallonRecoveryTime / OneBalloonRecoveryTime) * 100);
        animMVP.UpdatePumpProgress((currBallonRecoveryTime / OneBalloonRecoveryTime) * 100);
        
    }
}
