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

        get { return _health; }
    }

    public Animator anim;
    public BalloonFighterBody body;
	public Logger logger;
	public GameObject deathPrefab;

    public AudioClip balloonPopClip;
    public AudioClip balloonRecoverClip;

    public float oneBalloonRecoveryTime = 15f;
    float currBallonRecoveryTime = 0;

	[Header("Debugging")]
    public bool damageTrigger = false;

    public void Damage()
    {
        if (balloonPopClip != null) {
            AudioSource.PlayClipAtPoint(balloonPopClip, transform.position);
        }
        Health--;
    }

    public void RestoreBalloon() {
		if(balloonRecoverClip != null) {
			AudioSource.PlayClipAtPoint(balloonRecoverClip, transform.position);
		}

        Health++;
    }

	public void Kill() {
		Destroy(gameObject);

		if(deathPrefab != null) {
			Instantiate(deathPrefab, transform.position, deathPrefab.transform.rotation, null);
		}
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
		if(logger == null) {
			logger = Logger.DefaultLogger;
		}

		Assert.IsNotNull(body);
		Assert.IsNotNull(anim);

		if(balloonPopClip == null) {
			logger.Warn("No pop clip given", this);
		}
		if(balloonRecoverClip == null) {
			logger.Warn("No recover clip given", this);
		}
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
