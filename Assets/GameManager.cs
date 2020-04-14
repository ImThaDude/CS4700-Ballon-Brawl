using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text gameOverText;
    public Text startText;
    public Text player1Lives;
    public Text player2Lives;
    public Text player3Lives;

    public GameObject player1;
    public GameObject player2;
    public GameObject player3;

    private PlayerMetadata player1MetaData;
    private PlayerMetadata player2MetaData;
    private PlayerMetadata player3MetaData;
    private BalloonFighterBody player1Controller;
    private BalloonFighterBody player2Controller;
    private BalloonFighterBody player3Controller;
    private bool player1IsDead = false;
    private bool player2IsDead = false;
    private bool player3IsDead = false;

    private float startTimer = 3.0f;

    private void Start()
    {
        gameOverText.text = "";
        player1MetaData = player1.GetComponent<PlayerMetadata>();
        player2MetaData = player2.GetComponent<PlayerMetadata>();
        player3MetaData = player3.GetComponent<PlayerMetadata>();

        player1Controller = player1.GetComponent<BalloonFighterBody>();
        player2Controller = player2.GetComponent<BalloonFighterBody>();
        player3Controller = player3.GetComponent<BalloonFighterBody>();
        player1Controller.enabled = false;
        player2Controller.enabled = false;
        player3Controller.enabled = false;
    }

    private void Update()
    {
        //PLayerLives Text Changes
        if(player1MetaData._Health >= 0) {
            player1Lives.text = "P1 Lives: " + player1MetaData._Health;
        } else {
            player1Lives.text = "P1 DEAD";
            player1IsDead = true;
        }

        if (player2MetaData._Health >= 0) {
            player2Lives.text = "P2 Lives: " + player2MetaData._Health;
        }
        else {
            player2Lives.text = "P2 DEAD";
            player2IsDead = true;
        }

        if (player3MetaData._Health >= 0) {
            player3Lives.text = "P3 Lives: " + player3MetaData._Health;
        }
        else {
            player3Lives.text = "P3 DEAD";
            player3IsDead = true;
        }

        //End of Game check and GameOver Screen
        if (player1IsDead && player2IsDead)
        {
            Debug.Log("player 3 wins");
            gameOverText.text = "Game Over\n Player 3 Wins!";
        } else if (player2IsDead && player3IsDead)
        {
            Debug.Log("player 1 wins");
            gameOverText.text = "Game Over\n Player 1 Wins!";
        } else if (player1IsDead && player3IsDead)
        {
            Debug.Log("player 2 wins");
            gameOverText.text = "Game Over\n Player 2 Wins!";
        }

        //Start of Game Countdown
        startTimer -= 1 * Time.deltaTime;
        double count = Math.Ceiling(startTimer);
        if (startTimer > 0)
        {
            startText.text = count.ToString();
        } else
        {
            startText.text = "Start!";
            player1Controller.enabled = true;
            player2Controller.enabled = true;
            player3Controller.enabled = true;
        }
        if (startTimer < -1)
        {
            startText.enabled = false;
        }
    }

}
