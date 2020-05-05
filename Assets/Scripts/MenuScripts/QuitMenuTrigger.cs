using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitMenuTrigger : MonoBehaviour
{
    public bool onQuit = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        onQuit = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        onQuit = false;
    }

    public bool getOnQuit()
    {
        return onQuit;
    }
}
