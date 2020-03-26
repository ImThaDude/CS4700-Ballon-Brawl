using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxFollower : MonoBehaviour
{

    public Transform Followed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Followed.position;
        transform.rotation = Followed.rotation;
    }
}
