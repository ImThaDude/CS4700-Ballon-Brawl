using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GetAwayFromRedController : AIController
{

    public class AIButton
    {
        public float cd;
        public Action callFunction;
        public float currCd;

        public AIButton(float cd, Action callFunction)
        {
            this.cd = cd;
            this.callFunction = callFunction;
        }

        public void PressFunction()
        {
            if (currCd < 0)
            {
                if (callFunction != null)
                {
                    callFunction();
                }
                currCd = cd;
            }
        }

        public void UpdateTime(float time)
        {
            currCd -= time;
        }
    }

    public RedzoneDamage redzone;
    public float fastestClickSpeed = 0.1f;
    public BalloonFighterBody body;
    public Dictionary<string, AIButton> aiButtons;
    public bool ActivateAI = false;

    public PlayerMetadata mind;
    public float getAwayFromRedDistance = 10;
    public float getAwayFromRedModifier = 100;

    public override float Evaluate() {
        return (Vector3.Distance(transform.position, Vector3.zero) - redzone.currentRedZone) * getAwayFromRedModifier  + (getAwayFromRedDistance * (1 - (mind.Health / 2)));
    }

    // Start is called before the first frame update
    void Start()
    {
        aiButtons = new Dictionary<string, AIButton>();
        aiButtons.Add("jump", new AIButton(fastestClickSpeed, () =>
        {
            body.SetJump();
        }
        ));
    }

    bool disengagedStage = false;

    // Update is called once per frame
    void Update()
    {
        if (ActivateAI)
        {
            foreach (var aiButton in aiButtons.Values)
                aiButton.UpdateTime(Time.deltaTime);
            if (Vector3.Distance(transform.position, Vector3.zero) > redzone.currentRedZone)
            {
                disengagedStage = true;
                if (transform.position.y < 0) {
                    aiButtons["jump"].PressFunction();
                } else {
                    body.SetJump(false);
                }

                if (transform.position.x < 0)
                {
                    body.MoveHorizontal(1f);
                }
                else
                {
                    body.MoveHorizontal(-1f);
                }
            } else {
                if (disengagedStage) {
                    body.MoveHorizontal(0);
                    body.SetJump(false);
                    disengagedStage = false;
                    gameObject.SetActive(false);
                }
            }
        }
    }

    void OnDisable()
    {
        body.MoveHorizontal(0);
        body.SetJump(false);
        disengagedStage = false;
    }
}
