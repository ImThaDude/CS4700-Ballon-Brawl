using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class PlayerCollisionPool : MonoBehaviour
{
    //This is for when players start colliding.
    [System.Serializable]
    public class CollisionEvent : UnityEvent<string, string, RaycastHit2D>
    {
    }

    //This is for when players stop colliding each other.
    [System.Serializable]
    public class PVPEvent : UnityEvent<string, string>
    {
    }

    [System.Serializable]
    public class EntityGameObjectEvent : UnityEvent<string, int, GameObject>
    {
    }

    public enum CollisionLayers
    {
        none = 0, balloon = 1, player = 2
    }

    public class CurrentPlayerCollision : PlayerCollision
    {

        List<string> collidingMatrix;

        public CurrentPlayerCollision(string entityId, Vector2 position, Vector2 scale, int layer, LayerMask collisionLayers) : base(entityId, position, scale, layer, collisionLayers)
        {
            collidingMatrix = new List<string>();
        }

        public int CollisionCount()
        {
            return collidingMatrix.Count;
        }

        public List<string> CollidingArray()
        {
            return collidingMatrix;
        }

        public void ClearCollidingArray()
        {
            collidingMatrix.Clear();
        }

        public bool RemoveCollision(string entityId)
        {
            if (!collidingMatrix.Contains(entityId))
            {
                return false;
            }
            collidingMatrix.Remove(entityId);
            return true;
        }

        public bool IsCollidingWith(string entityId)
        {
            if (collidingMatrix.Contains(entityId))
            {
                return true;
            }
            return false;
        }

        public bool StartCollisionWith(string entityId)
        {
            if (!collidingMatrix.Contains(entityId))
            {
                collidingMatrix.Add(entityId);
                return true;
            }
            return false;
        }

        public bool EndCollisionWith(string entityId)
        {
            if (collidingMatrix.Contains(entityId))
            {
                collidingMatrix.Remove(entityId);
                return true;
            }
            return false;
        }

    }

    [System.Serializable]
    public class CollisionPoolEvents
    {
        public CollisionEvent OnPlayerEnterCollision;

        public CollisionEvent OnPlayerStayCollision;

        public PVPEvent OnPlayerExitCollision;
    }

    [System.Serializable]
    public class CollisionEntityEvent
    {
        public EntityGameObjectEvent OnEntityGameObjectCreated;
    }

    public class PlayerCollision
    {
        public string entityId;
        public Vector2 position;
        public Vector2 scale;
        public int layer;
        public LayerMask collisionLayers;

        public PlayerCollision(string entityId, Vector2 position, Vector2 scale, int layer, LayerMask collisionLayers)
        {
            this.entityId = entityId;
            this.position = position;
            this.scale = scale;
            this.layer = layer;
            this.collisionLayers = collisionLayers;
        }
    }

    public CollisionPoolEvents OnEvents;
    public CollisionEntityEvent CollisionPoolDelegates;

    Dictionary<string, CurrentPlayerCollision> _collisionPool;
    Dictionary<string, GameObject> _objectPool;

    Dictionary<string, CurrentPlayerCollision> collisionPool
    {
        get
        {
            if (_collisionPool == null)
            {
                _collisionPool = new Dictionary<string, CurrentPlayerCollision>();
            }
            return _collisionPool;
        }
    }
    Dictionary<string, GameObject> objectPool
    {
        get
        {
            if (_objectPool == null)
            {
                _objectPool = new Dictionary<string, GameObject>();
            }
            return _objectPool;
        }
    }

    public bool RegisterEntity(string entityId, Vector2 position, Vector2 scale, int layer, LayerMask collisionLayers)
    {
        if (collisionPool.ContainsKey(entityId))
        {
            return false;
        }

        collisionPool.Add(entityId, new CurrentPlayerCollision(entityId, position, scale, layer, collisionLayers));

        if (!objectPool.ContainsKey(entityId))
        {
            GameObject collisionObject = new GameObject(entityId);
            collisionObject.transform.position = position;
            collisionObject.transform.localScale = scale;
            collisionObject.layer = layer;
            CollisionPoolDelegates.OnEntityGameObjectCreated.Invoke(entityId, layer, collisionObject);
            objectPool.Add(entityId, collisionObject);
        }
        return true;
    }

    //This is only for things that want to trigger not for things that are touching.
    public bool RegisterEntityPositionEvent(string entityId)
    {
        return RegisterEntityPositionEvent(entityId, collisionPool[entityId].position);
    }

    public bool RegisterEntityPositionEvent(string entityId, Vector2 position)
    {
        return RegisterEntityPositionEvent(entityId, position, collisionPool[entityId].scale, collisionPool[entityId].layer, collisionPool[entityId].collisionLayers);
    }

    public bool RegisterEntityPositionEvent(string entityId, Vector2 position, Vector2 scale, int layer, LayerMask collisionLayers)
    {
        if (collisionPool.ContainsKey(entityId))
        {
            //Check if it will hit anything.
            RaycastHit2D[] cols;
            if (BoxCastAllForEntity(entityId, position, scale, collisionLayers, out cols))
            {
                //Debug.Log("Collision detected.");
                //Debug.Log(cols[0].transform.name);
                foreach (var col in cols)
                {
                    var hitEntityId = col.collider.gameObject.name;
                    if (collisionPool.ContainsKey(hitEntityId))
                    {
                        if (collisionPool[entityId].StartCollisionWith(hitEntityId))
                        {
                            OnEvents.OnPlayerEnterCollision.Invoke(entityId, hitEntityId, col);
                        }
                        else
                        {
                            OnEvents.OnPlayerStayCollision.Invoke(entityId, hitEntityId, col);
                        }
                    }
                }
            }
            //Move the object in position
            setObjectPosition(entityId, position);
            return true;
        }
        return false;
    }

    bool setObjectPosition(string entityId, Vector2 position)
    {
        if (!collisionPool.ContainsKey(entityId))
        {
            return false;
        }
        collisionPool[entityId].position = position;
        objectPool[entityId].transform.position = position;
        return true;
    }

    bool BoxCastAllForEntity(string entityId, Vector2 position, out RaycastHit2D[] collisions)
    {
        return BoxCastAllForEntity(entityId, position, collisionPool[entityId].scale, collisionPool[entityId].collisionLayers, out collisions);
    }

    bool BoxCastAllForEntity(string entityId, Vector2 position, Vector2 scale, LayerMask collisionLayer, out RaycastHit2D[] collisions)
    {
        collisions = Physics2D.BoxCastAll(position, scale, 0, Vector2.zero, 0, collisionLayer);
        if (collisions.Length > 0)
        {
            //Debug.Log("Collision detected.");
            //Debug.Log(entityId + " to " + collisions.Length + " objects.");
            collisions = collisions;
            return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCollisionsForAllCollisionPool();
    }

    void UpdateCollisionsForAllCollisionPool()
    {
        RegisterEventForAllCollisionPool();
        RemoveExtraCollisionsForAllCollisionPool();
    }

    void RegisterEventForAllCollisionPool()
    {
        foreach (var pCol in collisionPool)
        {
            var cPCol = pCol.Value;
            RegisterEntityPositionEvent(cPCol.entityId);
        }
    }

    void RemoveExtraCollisionsForAllCollisionPool()
    {
        foreach (var pCol in collisionPool)
        {
            var cPCol = pCol.Value;
            //Remove if the cast is no longer valid.
            List<string> savedValues = new List<string>();
            if (cPCol.CollisionCount() > 0)
            {
                RaycastHit2D[] currentPlayerCollisions;
                if (BoxCastAllForEntity(cPCol.entityId, cPCol.position, out currentPlayerCollisions))
                {
                    //For each name in the colliding array check which ones are still revelant
                    var previousPlayerCollisionArray = cPCol.CollidingArray();
                    foreach (var c in currentPlayerCollisions)
                    {
                        //Debug.Log(c.collider.gameObject.name);
                        if (previousPlayerCollisionArray.Contains(c.collider.gameObject.name))
                        {
                            savedValues.Add(c.collider.gameObject.name);
                        }
                    }

                    //Check which ones are to be deleted
                    List<string> removingValues = new List<string>();
                    for (int i = 0; i < previousPlayerCollisionArray.Count; i++)
                    {
                        if (!savedValues.Contains(previousPlayerCollisionArray[i]))
                        {
                            removingValues.Add(previousPlayerCollisionArray[i]);
                        }
                    }

                    //Remove the ones queued to be deleted
                    foreach (var r in removingValues)
                    {
                        //Debug.Log("Test1...");
                        OnEvents.OnPlayerExitCollision.Invoke(cPCol.entityId, r);
                        cPCol.RemoveCollision(r);
                    }
                }
                else
                {
                    //Debug.Log("Test...");
                    foreach (var r in cPCol.CollidingArray())
                    {
                        OnEvents.OnPlayerExitCollision.Invoke(cPCol.entityId, r);
                    }
                    cPCol.ClearCollidingArray();
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        //Draw colliderPool.
        foreach (var cp in collisionPool)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(cp.Value.position, cp.Value.scale);
        }

        //Draw objectPool.
        foreach (var op in objectPool)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(op.Value.transform.position, op.Value.transform.localScale);
        }
    }

}
