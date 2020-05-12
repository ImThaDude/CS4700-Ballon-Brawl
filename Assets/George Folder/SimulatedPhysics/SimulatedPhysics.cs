using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulatedPhysics : MonoBehaviour
{

    public Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        velocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(velocity * Time.deltaTime);
    }

    public void SetVelocity(Vector3 vel) {
        velocity = vel;
    }
    
}
