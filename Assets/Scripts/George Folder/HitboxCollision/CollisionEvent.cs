using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CollisionEvent : UnityEvent<Collider2D>
{
    
}

[System.Serializable]
public class CollisionsEvent : UnityEvent<RaycastHit2D[]> 
{

}
