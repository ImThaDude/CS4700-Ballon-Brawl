using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class PlayerHandlerMVP : MonoBehaviour
{

    [System.Serializable]
    public class PositionEvent : UnityEvent<Vector3> {
    }

    [System.Serializable]
    public class FloatEvent : UnityEvent<float> {
    }

    public ClientNetworkManagerMVP client;

    public Dictionary<string, PlayerClass> playerPool;

    public GameObject NetworkPlayerPrefab;

    public Queue<string> spawningPlayers;

    public PositionEvent OnSelfPositionUpdate;

    void Start()
    {
        playerPool = new Dictionary<string, PlayerClass>();
        spawningPlayers = new Queue<string>();
        tempFlapTrigger = new Dictionary<string, bool>();
        deletePlayerQueue = new Queue<string>();
    }

    void Update()
    {
        if (spawningPlayers.Count > 0)
        {
            while (spawningPlayers.Count > 0)
            {
                var userId = spawningPlayers.Dequeue();
                GameObject p = Instantiate(NetworkPlayerPrefab);
                p.transform.parent = transform;
                p.name = userId;
                playerPool[userId].created = p;
            }
        }

        foreach (var a in playerPool) {
            UpdateReceivePositionFromPlayer(a.Key, a.Value.playerPosition.position, a.Value.playerPosition.velocity);
            var anim = a.Value.animation;
            UpdateReceiveAnimation(a.Key, anim.HP, anim.IsGrounded, anim.Movement, anim.Dir, anim.Flap, anim.PumpProgress);
        }

        if (queuePositionChange) {
            queuePositionChange = false;
            OnSelfPositionUpdate.Invoke(positionChange);
        }

        if (queuePlayerCollision) {
            queuePlayerCollision = false;
            OnPlayerCollision.Invoke(collisionPosition);
        }

        //Delete player queue
        if (deletePlayerQueue.Count > 0) {
            while (deletePlayerQueue.Count > 0) {
                var userId = deletePlayerQueue.Dequeue();
                Destroy(playerPool[userId].created);
                playerPool.Remove(userId);                
            }
        }

        if (queueRedZoneChange) {
            queueRedZoneChange = false;
            OnRedZoneChange.Invoke(redZoneFloat);
        }
    }

    public void OnReceiveFromPlayer(string userId)
    {
        if (!playerPool.ContainsKey(userId) && client.userId != userId)
        {
            playerPool.Add(userId, new PlayerClass());
            spawningPlayers.Enqueue(userId);
        }
    }

    public class PlayerClass
    {
        public PlayerClass()
        {
            playerPosition = new PlayerPosition();
            animation = new AnimationState();
            metadata = new PlayerMetadata();
        }

        public PlayerPosition playerPosition;
        public AnimationState animation;
        public PlayerMetadata metadata;
        public GameObject created;
    }

    public class PlayerPosition
    {
        //All positions will go here
        public Vector3 position;
        public Vector3 velocity;
    }

    public void UpdateReceivePositionFromPlayer(string userId, Vector3 position, Vector3 velocity)
    {
        if (playerPool.ContainsKey(userId))
        {
            if (playerPool[userId].created != null)
            {
                playerPool[userId].created.transform.position = position;
                var phy = playerPool[userId].created.GetComponent<SimulatedPhysics>();
                if (phy != null) {
                    phy.SetVelocity(velocity);
                }
            }
        }
    }

    public bool queuePositionChange = false;
    public Vector3 positionChange;

    public void OnReceivePositionFromPlayer(string userId, Vector3 position, Vector3 velocity)
    {
        OnReceiveFromPlayer(userId);
        if (playerPool.ContainsKey(userId))
        {
            playerPool[userId].playerPosition.position = position;
            playerPool[userId].playerPosition.velocity = velocity;
        }

        if (userId == client.userId) {
            //Self update
            queuePositionChange = true;
            positionChange = position;            
        }
    }

    public class AnimationState
    {
        //All animations will go here and will get updated
        public float HP;
        public bool IsGrounded;
        public float Movement;
        public float Dir;
        public bool Flap;
        public float PumpProgress;
    }

    public Dictionary<string, bool> tempFlapTrigger;

    public void CheckTrigger(string userId, bool curr, Action OnTriggerEvent) {
        if (!tempFlapTrigger.ContainsKey(userId)) {
            tempFlapTrigger.Add(userId, false);
        }
        if (tempFlapTrigger[userId] != curr) {
            if (OnTriggerEvent != null) {
                OnTriggerEvent();
            }
        }
        tempFlapTrigger[userId] = curr;
    }

    public void UpdateReceiveAnimation(string userId, float HP, bool IsGrounded, float Movement, float Dir, bool Flap, float PumpProgress)
    {
        if (playerPool.ContainsKey(userId))
        {
            if (playerPool[userId].created != null)
            {
                var anim = playerPool[userId].created.GetComponent<Animator>();
                anim.SetFloat("HP", HP);
                anim.SetBool("IsGrounded", IsGrounded);
                anim.SetFloat("Movement", Movement);
                anim.SetFloat("Dir", Dir);
                CheckTrigger(userId, Flap, () => {anim.SetTrigger("Flap");});
                anim.SetFloat("PumpProgress", PumpProgress);
            }
        }
    }

    public void OnReceiveAnimation(string userId, float HP, bool IsGrounded, float Movement, float Dir, bool Flap, float PumpProgress)
    {
        OnReceiveFromPlayer(userId);
        if (playerPool.ContainsKey(userId))
        {
            var anim = playerPool[userId].animation;
            anim.HP = HP;
            anim.IsGrounded = IsGrounded;
            anim.Movement = Movement;
            anim.Dir = Dir;
            anim.Flap = Flap;
            anim.PumpProgress = PumpProgress;
        }
    }

    public class PlayerMetadata
    {
        //all metadata will go here
        public int HP;
    }

    public void OnReceiveMetadata(string userId, int HP)
    {

    }

    public PositionEvent OnPlayerCollision;
    public Vector3 collisionPosition;
    public bool queuePlayerCollision;

    public void OnReceiveCollision(string userId, string collidedId, Vector3 collidedPos) {
        if (collidedId == client.userId) {
            //Self update
            queuePlayerCollision = true;
            collisionPosition = collidedPos;
        }
    }

    //Delete player in the queue
    public Queue<string> deletePlayerQueue;

    public void OnDeletePlayer(string userId) {
        Debug.Log("[PlayerHandleMVP]Deleting player... " + userId);
        playerPool = new Dictionary<string, PlayerClass>();
        spawningPlayers = new Queue<string>();
        tempFlapTrigger = new Dictionary<string, bool>();
        if (tempFlapTrigger.ContainsKey(userId))
            tempFlapTrigger.Remove(userId);
        if (playerPool.ContainsKey(userId)) {
            deletePlayerQueue.Enqueue(userId);
        }
    }

    public FloatEvent OnRedZoneChange;
    public float redZoneFloat;
    public bool queueRedZoneChange;

    public void OnReceiveRedZone(float redZoneDistance) {
        redZoneFloat = redZoneDistance;
        queueRedZoneChange = true;
    }

}
