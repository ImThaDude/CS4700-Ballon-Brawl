using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonBrawlMenuTrigger : MonoBehaviour
{
    public bool onBalloonBrawl = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        onBalloonBrawl = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        onBalloonBrawl = false;
    }

    public bool getOnBalloonBrawl()
    {
        return onBalloonBrawl;
    }
}
