using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RunAwayController : AIController
{

    public float targetRefresh = 5;
    public float detectionRadious = 9;
    public float currTargetRefresh = 5;
    public LayerMask playerMask;

    public GameObject[] detectedEnemies;

    public float fastestClickSpeed = 0.1f;
    public BalloonFighterBody body;
    public Dictionary<string, AIButton> aiButtons;

    public float runawayDesireRate = 9f;
    public float closenessModifier = 100f;

    public float hurryEscape = 4f;
    //Eventually read health of enemy...
    public PlayerHandlerMVP poolMVP;

    bool disengagedStage = false;

    public float enemyBallonHeightModifier = 0.1f;

    public PlayerMetadata mind;

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

    // Update is called once per frame
    void Update()
    {
        foreach (var aiButton in aiButtons.Values)
            aiButton.UpdateTime(Time.deltaTime);
        if (detectedEnemies.Length > 0)
        {
            if (Vector3.Distance(transform.position, detectedEnemies[0].transform.position) < detectionRadious)
            {
                disengagedStage = true;
                if (Vector3.Distance(transform.position, detectedEnemies[0].transform.position) < hurryEscape)
                {
                    aiButtons["jump"].PressFunction();
                }
                else
                {
                    if (transform.position.y < detectedEnemies[0].transform.position.y) {
                        body.SetJump(false);
                    } else {
                        aiButtons["jump"].PressFunction();
                    }
                }

                if (transform.position.x > detectedEnemies[0].transform.position.x)
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
                if (disengagedStage)
                {
                    body.MoveHorizontal(0);
                    body.SetJump(false);
                    disengagedStage = false;
                    gameObject.SetActive(false);
                }
            }
        }

        if (currTargetRefresh < 0)
        {
            currTargetRefresh = targetRefresh;
            body.MoveHorizontal(0);
            body.SetJump(false);
            disengagedStage = false;
            gameObject.SetActive(false);
        }
        else
        {
            currTargetRefresh -= Time.deltaTime;
        }
    }

    public override float Evaluate()
    {
        float closest;
        int count;
        DetectEnemy(out closest, out count);
        return (count <= 0) ? -100 : (1 - (runawayDesireRate - closest) / runawayDesireRate) * (closenessModifier * 2 * (2 - mind._Health) / 2);
    }

    public void DetectEnemy(out float closest, out int count)
    {
        currTargetRefresh = targetRefresh;
        List<GameObject> targets = new List<GameObject>();
        var dets = Physics2D.OverlapCircleAll(transform.position, detectionRadious, playerMask);
        foreach (var d in dets)
        {
            if (d.gameObject != transform.parent.parent.gameObject)
            {
                targets.Add(d.gameObject);
            }
        }
        detectedEnemies = targets.ToArray();
        count = targets.Count;

        float tempClosest = 100;
        foreach (var t in targets)
        {
            if (Vector3.Distance(t.transform.position, transform.position) < tempClosest)
            {
                tempClosest = Vector3.Distance(t.transform.position, transform.position);
                Debug.Log(t.name);
            }
        }
        closest = tempClosest;
    }

    void OnDisable()
    {
        body.MoveHorizontal(0);
        body.SetJump(false);
        disengagedStage = false;
    }

}
