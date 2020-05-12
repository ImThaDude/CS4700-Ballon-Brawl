using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealController : AIController
{

    public PlayerMetadata mind;
    public float idleModifier = 50f;

    public override float Evaluate() {
        return (mind._Health == 0) ? idleModifier : -100f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
