using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonFighterHealth : MonoBehaviour
{

    [SerializeField] private int _health = 2;
    public int Health
    {
        set
        {
			if(_health != value) {
				_health = value;
				OnHealthChanged();
			}
        }
        get
        {
            return _health;
        }
    }

    //Experimental
    //public float SpeedBuff = 1;

    public Animator anim;

    public BalloonFighterBody Body;

    public AudioClip BalloonPop;
    public AudioClip BalloonRecover;

    public float OneBalloonRecoveryTime = 15f;
    float currBallonRecoveryTime = 0;
    public bool DamageTrigger = false;

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

	private void OnHealthChanged() 
    {
        if (Body != null && anim != null)
        {
            anim.SetFloat("HP", Health);

            if (Health < 0)
            {
                //Destroy(transform.gameObject);
                Body.Faint();
            }
			else if (Health == 0)
            {
				Body.DisableFlight();
                Body.BeginParachuting();
            }
            else
            {
                Body.EnableFlight();
            }
        }
    }


    private void Start()
    {
		OnHealthChanged();
    }

    private void Update()
    {
        if (DamageTrigger)
        {
            Damage();
            DamageTrigger = false;
        }

        if (Body.CurrentState == BalloonFighterBody.State.Idle && Health == 0) {
                
			//Counter for idle time
			currBallonRecoveryTime += Time.deltaTime;
			Debug.Log(currBallonRecoveryTime);
			
			//If fulfilled add a balloon
			if (currBallonRecoveryTime > OneBalloonRecoveryTime) {
				RestoreBalloon();
				currBallonRecoveryTime = 0;
			}
			
        } else {
            currBallonRecoveryTime = 0;
        }

        //Show on animation
        anim.SetFloat("PumpProgress", (currBallonRecoveryTime / OneBalloonRecoveryTime) * 100);
        
    }
}
