using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Camera cam;
    public Transform followEntity;
    public Rigidbody2D followRigidBody;
    public float followRate = 0.75f;
    public float startZoom = 5f;
    public float velocityZoomRate = 1f;
    public float zoomRate = 0.75f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (followEntity == null) {
            followEntity = GameObject.FindGameObjectWithTag("Player").transform;
        }
        
        if (followEntity != null) {
            var vt = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            var ve = new Vector3(followEntity.position.x, followEntity.position.y, transform.position.z);
            //transform.position = Vector3.Lerp(vt, ve, followRate);
            
            var rb = followEntity.GetComponent<Rigidbody2D>();
            if (rb != null) {
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, rb.velocity.magnitude * velocityZoomRate + startZoom, zoomRate);
            } else {
                cam.orthographicSize = startZoom;
            }
        }
    }
}
