using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Match : MonoBehaviour
{
    const float INITIAL_TIME = 60;  // match time in seconds
    const float INITIAL_TIME_FREZE = 6f;  // freeze time in seconds

    public Player bluePlayer;
    public Player redPlayer;

    public Text bluePlayerScoreText;
    public Text redPlayerScoreText;

    public Text countdownText;

    public GameObject gameOverCanvas;
    public Text gameOverText;

    public GameObject prematchCanvas;
    public Text prematchText;

    public GameObject notBirp;
    public Button replayButton;

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
            bluePlayerScoreText.text = bluePlayer.score.ToString();
            redPlayerScoreText.text = redPlayer.score.ToString();
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
        replayButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(1);
        });
        gameOverCanvas.SetActive(true);
        if (bluePlayer.score > redPlayer.score)
            gameOverText.text = "Blue Player Won!";
        else if (redPlayer.score > bluePlayer.score)
            gameOverText.text = "Red Player Won!";
        else
        {
            gameOverText.text = "Tie!!";
            notBirp.SetActive(true);
        }

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
