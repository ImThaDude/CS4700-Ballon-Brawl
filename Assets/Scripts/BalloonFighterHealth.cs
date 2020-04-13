using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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

    public BalloonFighterBody body;

    public AudioClip balloonPop;
    public AudioClip balloonRecover;

    public float oneBalloonRecoveryTime = 15f;
    float currBallonRecoveryTime = 0;

	[Header("Debugging")]
    public bool damageTrigger = false;

    public void Damage()
    {
        if (balloonPop != null) {
            AudioSource.PlayClipAtPoint(balloonPop, transform.position);
        } else {
            CSLogger.L("BalloonPop clip as not been assigned.");
        }
        Health--;
    }

    public void RestoreBalloon() {
        if (balloonRecover != null) {
            AudioSource.PlayClipAtPoint(balloonRecover, transform.position);
        } else {
            CSLogger.L("BalloonRecover clip as not been assigned.");
        }
        Health++;
    }

	private void OnHealthChanged() 
    {
        if (body != null && anim != null)
        {
            anim.SetFloat("HP", Health);

            if (Health < 0)
            {
                //Destroy(transform.gameObject);
                body.Faint();
            }
			else if (Health == 0)
            {
				body.DisableFlight();
                body.BeginParachuting();
            }
            else
            {
                body.EnableFlight();
            }
        }
    }


	private void Awake() {
		Assert.IsNotNull(body);
		Assert.IsNotNull(anim);
	}

    private void Start()
    {
		OnHealthChanged();
    }

    private void Update()
    {
        if (damageTrigger)
        {
            Damage();
            damageTrigger = false;
        }

		// Control the balloon refilling while idle
        if (body.CurrentState == BalloonFighterBody.State.Idle && Health == 0) {
                
			//Counter for idle time
			currBallonRecoveryTime += Time.deltaTime;
			
			//If fulfilled add a balloon
			if (currBallonRecoveryTime > oneBalloonRecoveryTime) {
				RestoreBalloon();
				currBallonRecoveryTime = 0;
			}
			
        } else {
            currBallonRecoveryTime = 0;
        }

        //Show on animation
        anim.SetFloat("PumpProgress", (currBallonRecoveryTime / oneBalloonRecoveryTime) * 100);
        
    }
}
