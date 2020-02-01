using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Match : MonoBehaviour
{
    const float INITIAL_TIME = 1;  // match time in seconds
    const float INITIAL_TIME_FREZE = 0f;  // freeze time in seconds
    
    public Text countdownText;
    public GameObject gameOverCanvas;

    private float timeLeft = 0f;
    private int timeLeftToShow = 0;

    // Start is called before the first frame update
    void Start()
    { 
        SpawnPlayers();
        StartCountdown();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeLeft >= 0)
        {
            timeLeft -= Time.deltaTime;
            countdownText.text = ((int)timeLeft).ToString();
        }
        else {
            GameOver();
            Destroy(this);
        }
    }

    // Starts the countdown of the match
    void StartCountdown()
    {
        timeLeft = INITIAL_TIME + INITIAL_TIME_FREZE;
    }

    // Spawn the players on the map
    void SpawnPlayers()
    {

    }

    // End the game
    void GameOver() 
    {
        gameOverCanvas.SetActive(true);
        Time.timeScale = 0f;
    }
}
