using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IdleController : AIController
{

    public Vector3 goalIdlePosition;

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

    public float fastestClickSpeed = 0.3f;
    public BalloonFighterBody body;
    public Dictionary<string, AIButton> aiButtons;

    public float acceptanceDistance = 0.5f;

    public float refreshTime = 5f;
    public float currRefreshTime = 0f;

    public RedzoneDamage redzone;

    // Start is called before the first frame update
    void Start()
    {
        currRefreshTime = refreshTime;
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

        foreach (var aiButton in aiButtons.Values)
            aiButton.UpdateTime(Time.deltaTime);
        var temp = goalIdlePosition;
        temp.z = 0;
        if (Vector3.Distance(transform.position, temp) > acceptanceDistance)
        {
            disengagedStage = true;

            if (transform.position.y < goalIdlePosition.y)
            {
                aiButtons["jump"].PressFunction();
            }
            else
            {
                body.SetJump(false);
            }

            if (transform.position.x < goalIdlePosition.x)
            {
                body.MoveHorizontal(1f);
            }
            else
            {
                body.MoveHorizontal(-1f);
            }
        }
        else
        {
            GetNewDestionation();
            body.MoveHorizontal(0);
            body.SetJump(false);
            disengagedStage = false;
        }

        currRefreshTime -= Time.deltaTime;

        if (currRefreshTime < 0)
        {
            GetNewDestionation();
            body.MoveHorizontal(0);
            body.SetJump(false);
            disengagedStage = false;
            currRefreshTime = refreshTime;
        }
    }

    void OnEnable()
    {
        GetNewDestionation();
        body.MoveHorizontal(0);
        body.SetJump(false);
        disengagedStage = false;
        currRefreshTime = refreshTime;
    }

    void OnDisable()
    {
        body.MoveHorizontal(0);
        body.SetJump(false);
        disengagedStage = false;
    }

    void GetNewDestionation()
    {
        Vector3 rand = UnityEngine.Random.insideUnitSphere;
        rand.Normalize();
        goalIdlePosition = transform.position + rand * redzone.currentRedZone / 2;
    }
}
