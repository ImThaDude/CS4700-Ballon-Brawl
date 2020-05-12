using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControllerManager : MonoBehaviour
{
    AIController[] aiList;
    public float updateAIList = 5;
    public float currUpdateAIList;

    public float AIReactionTime = 0.75f;
    public float currReactionTime = 0;
    public GameObject currentState;

    public bool AIEnabled = false;

    void Start()
    {
        currUpdateAIList = updateAIList;
        currReactionTime = AIReactionTime;
        if (AIEnabled)
        {
            GetAllAIControllers();
            DisableAllAIControllers();
            EnableHighestEvaluatedController();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (AIEnabled)
        {
            currUpdateAIList -= Time.deltaTime;
            if (currUpdateAIList < 0)
            {
                currUpdateAIList = updateAIList;
            }

            currReactionTime -= Time.deltaTime;
            if (currReactionTime < 0 || !currentState.activeInHierarchy)
            {
                currReactionTime = AIReactionTime;
                EnableHighestEvaluatedController();
            }
        }
    }

    void GetAllAIControllers()
    {
        aiList = GetComponentsInChildren<AIController>(true);
        Debug.Log("Obtained " + aiList.Length + "controllers.");
    }

    void DisableAllAIControllers() {
        if (aiList != null) {
            foreach (var a in aiList) {
                a.gameObject.SetActive(false);
            }
        }
    }

    void EnableHighestEvaluatedController()
    {
        GameObject highestObject = null;
        float highestValue = -1000000;
        foreach (var ai in aiList)
        {
            var e = ai.Evaluate();
            if (e > highestValue)
            {
                highestValue = e;
                highestObject = ai.gameObject;
            }
        }

        if (currentState != null)
        {
            currentState.SetActive(false);
        }
        currentState = highestObject;
        if (highestObject != null)
        {
            Debug.Log("Started state..." + highestObject.name);
            currentState.SetActive(true);
        }
        else
        {
            Debug.Log("No state set...");
        }
    }
}
