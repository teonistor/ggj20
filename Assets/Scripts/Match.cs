using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Match : MonoBehaviour
{
    const float INITIAL_TIME = 100;  // match time in seconds
    const float INITIAL_TIME_FREZE = 6f;  // freeze time in seconds
    
    public Text countdownText;
    public GameObject gameOverCanvas;
    public GameObject prematchCanvas;
    public Text prematchText;

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
        Time.timeScale = 0f;
        Prematch prematch = prematchCanvas.AddComponent<Prematch>();
        prematch.timeFreze = INITIAL_TIME_FREZE;
        prematch.prematchCanvas = prematchCanvas;
        prematch.prematchText = prematchText;

        timeLeft = INITIAL_TIME;
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

    public class Prematch : MonoBehaviour 
    {
        public float timeFreze = 0f;
        public float timeFrezeLeft = 0f;
        public int lastShown = 0;
        public GameObject prematchCanvas;
        public Text prematchText;

        void Update()
        {
            timeFrezeLeft = timeFreze - Time.unscaledTime;
            UpdateNumber();
        }

        void UpdateNumber() {
            if (timeFrezeLeft < -0.7) {
                Time.timeScale = 1f;
                prematchCanvas.SetActive(false);
                Destroy(this);
            }
            if(lastShown != (int)timeFrezeLeft)
            {
                lastShown = (int)timeFrezeLeft;
                prematchText.text = lastShown.ToString();
                int lastShownCopy = (int)timeFreze - lastShown;
                while (lastShownCopy-- >= 0) prematchText.text += "!";
                prematchText.fontSize = 14 + ((int)Time.unscaledTime);
                if (lastShown == 0) {
                    prematchText.color = Color.red;
                }
            }
        }

    }
}
