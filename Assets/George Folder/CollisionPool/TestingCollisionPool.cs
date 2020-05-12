using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingCollisionPool : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public string entityId;
    public Vector2 position;
    public Vector2 scale;
    public int layerMask;
    public LayerMask mask;
    public bool registerObject;
    public bool registerPosition;
    public bool instantiateTestingEnvironment;
    public PlayerCollisionPool pool;

    // Update is called once per frame
    void Update()
    {
        if (registerObject) {
            pool.RegisterEntity(entityId, position, scale, layerMask, mask);
            registerObject = false;
        }

        if (registerPosition) {
            pool.RegisterEntityPositionEvent(entityId, position);
            registerPosition = false;
        }

        if (instantiateTestingEnvironment) {
            TestingEnvironment();
            instantiateTestingEnvironment = false;
        }
    }

    public void LogText(string txt) {
        Debug.Log(txt);
    }

    public void TestingEnvironment() {
        pool.RegisterEntity("test", Vector2.zero, Vector2.one, 10, 0);
        pool.RegisterEntity("test1", Vector2.zero, Vector2.one, 11, 1 << 10);
    }

}
