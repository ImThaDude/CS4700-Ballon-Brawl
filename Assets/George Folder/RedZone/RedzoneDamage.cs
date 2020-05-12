using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedzoneDamage : MonoBehaviour
{
    public float redStartZoneDistance = 63;
    public Transform redZoneCircle;
    public float redZoneMinimalDistance = 10f;
    public float currentRedZone = 63;
    public GameObject redZoneParticle;
    public float redZoneTime = 4.5f;
    public float currRedZoneTime = 0;
    public PlayerMetadata metadata;

    public void ChangeCurrentRedZone(float value)
    {
        currentRedZone = value;
    }

    void Start() {
        currentRedZone = redStartZoneDistance;
        currRedZoneTime = redZoneTime;
    }

    // Update is called once per frame
    void Update()
    {
        var dist = Vector3.Distance(Vector3.zero, transform.position);
        
        if (dist > currentRedZone)
        {
            if (redZoneParticle != null)
                redZoneParticle.SetActive(true);
            if (currRedZoneTime > 0) {
                currRedZoneTime -= Time.deltaTime;
            } else {
                currRedZoneTime = redZoneTime;
                metadata.Damage();
            }
        } else {
            if (redZoneParticle != null)
                redZoneParticle.SetActive(false);
            currRedZoneTime = redZoneTime;
        }

        redZoneCircle.localScale = new Vector3(currentRedZone / redStartZoneDistance, currentRedZone / redStartZoneDistance, 1);
    }
}
