using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonTripMenuTrigger : MonoBehaviour
{
    public bool onBalloonTrip = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        onBalloonTrip = true;
        Debug.Log("Entered");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        onBalloonTrip = false;
        Debug.Log("Exited");
      
    }

    public bool getOnBalloonTrip()
    {
        return onBalloonTrip;
    }
}
