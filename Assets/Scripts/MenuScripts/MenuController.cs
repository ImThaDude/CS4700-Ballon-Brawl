using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Text balloonTripText;
    public Text balloonBrawlText;
    public Text quitText;

    public BalloonTripMenuTrigger bTTrigger;
    public BalloonBrawlMenuTrigger bBTrigger;
    public QuitMenuTrigger qTrigger;

    private float timer = 0;
    private bool timerReset = false;


    void Update()
    {
        timer -= 1 * Time.deltaTime;

        //Logic for Balloon Trip Menu PLatform
        if (bTTrigger.getOnBalloonTrip())
        {
            if (!timerReset)
            {
                timer = 3;
                timerReset = true;
            }
    
            balloonTripText.GetComponent<Text>().color = Color.green;
            balloonTripText.GetComponent<Text>().text = "Balloon Trip (" + Mathf.CeilToInt(timer) + ")";

            if (timer <= 0)
            {
                Debug.Log("Load Scene 1");
                SceneManager.LoadScene(1);
            }
        } 

        //Logic for Balloon Brawl Menu PLatform
        else if (bBTrigger.getOnBalloonBrawl())
        {
            if (!timerReset)
            {
                timer = 3;
                timerReset = true;
            }

            balloonBrawlText.GetComponent<Text>().color = Color.green;
            balloonBrawlText.GetComponent<Text>().text = "Balloon Brawl (" + Mathf.CeilToInt(timer) + ")";

            if (timer <= 0)
            {
                Debug.Log("Load Scene 2");
                SceneManager.LoadScene(1);
            }
        } 

        //Logic for Quit Menu Platform
        else if (qTrigger.getOnQuit())
        {
            if (!timerReset)
            {
                timer = 3;
                timerReset = true;
            }

            quitText.GetComponent<Text>().color = Color.green;
            quitText.GetComponent<Text>().text = "Quit (" + Mathf.CeilToInt(timer) + ")";

            if (timer <= 0)
            {
                Debug.Log("Quit Game");
                Application.Quit();
            }
        }
        else
        {
            balloonTripText.GetComponent<Text>().color = Color.white;
            balloonTripText.GetComponent<Text>().text = "Balloon Trip";

            balloonBrawlText.GetComponent<Text>().color = Color.white;
            balloonBrawlText.GetComponent<Text>().text = "Balloon Brawl";

            quitText.GetComponent<Text>().color = Color.white;
            quitText.GetComponent<Text>().text = "Quit";

            timerReset = false;
        }
    }

}
