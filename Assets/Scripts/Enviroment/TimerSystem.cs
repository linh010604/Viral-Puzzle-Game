using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TimerSystem : MonoBehaviour
{
    public float TimeLeft = 180;
    public bool TimerOn = true;

    public TextMeshProUGUI TimerTxt;

    public Color fontColor;

    public bool showMilliseconds;

    private float currentSeconds;
    private int timerDefault;
    [Header("Loser UI Panel")]
    public GameObject LoserPage;   // Reference to the Loser UI Panel

    private bool gameEnded = false;

    void Start()
    {
        // timerText.color = fontColor;
        timerDefault = 0;
        timerDefault += (seconds + (minutes * 60) + (hours * 60 * 60));
        currentSeconds = timerDefault;
    }

    void Update()
    {
        if((currentSeconds -= Time.deltaTime) <= 0)
        {
            TimeUp();
            EndGame();
        }
        else
        {
            if(showMilliseconds)
                timerText.text = TimeSpan.FromSeconds(currentSeconds).ToString(@"hh\:mm\:ss\:fff");
            else
                timerText.text = TimeSpan.FromSeconds(currentSeconds).ToString(@"hh\:mm\:ss");
        }
    }

    private void TimeUp()
    {
        if (showMilliseconds)
            timerText.text = "00:00:00:000";
        else
            timerText.text = "00:00:00";
    }

    void EndGame()
    {
        gameEnded = true;

        // Display the Loser UI
        if (LoserPage != null)
        {
            LoserPage.SetActive(true);  // Activate the Loser UI when time runs out
        }

        // Optional: You can pause the game by setting Time.timeScale to 0
        Time.timeScale = 0f;  // Freeze the game
    }
}
