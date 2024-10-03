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

    private bool gameEnded = false;

    void Start()
    {
        TimerOn = true;
    }

    void Update()
    {
        if (TimerOn)
        {
            if (TimeLeft > 0)
            {
                TimeLeft -= Time.deltaTime;
                updateTimer(TimeLeft);
            }
            else
            {
                Debug.Log("Time is UP!");
                TimeLeft = 0;
                TimerOn = false;
                EndGame();
            }
        }
    }

    void updateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        TimerTxt.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (currentTime <= 10f)
        {
            TimerTxt.color = Color.red;  // Flash red when 10 seconds are left
        }
        else
        {
            TimerTxt.color = Color.white;  // Normal color
        }
    }

    void EndGame()
    {
        gameEnded = true;

        if (GameManager.instance != null)
        {
            GameManager.instance.GameOver();
        }

        // Optional: You can pause the game by setting Time.timeScale to 0
        Time.timeScale = 0f;  // Freeze the game
    }

}
