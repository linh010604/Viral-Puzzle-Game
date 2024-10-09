﻿using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System;

public class TimerSystem : MonoBehaviour
{

    public TextMeshProUGUI timerText;

    [Header("Time Values")]
    [Range(0, 60)]
    public int seconds;
    [Range(0, 60)]
    public int minutes;
    [Range(0, 60)]
    public int hours;

    public Color fontColor;

    public bool showMilliseconds;

    private float currentSeconds;
    private int timerDefault;


    private bool gameEnded = false;

    void Start()
    {
        timerDefault = 0;
        timerDefault += (seconds + (minutes * 60) + (hours * 60 * 60));
        currentSeconds = timerDefault;

    }

    void Update()
    {
        if ((currentSeconds -= Time.deltaTime) <= 0)
        {
            TimeUp();
            EndGame();
        }
        else
        {
            if (showMilliseconds)
            {
                if (currentSeconds <= 10f)
                {
                    if (fontColor == Color.white)
                        fontColor = Color.red;  // Flash red when 10 seconds are left
                    else
                    {
                        fontColor = Color.white;  // Normal color
                    }
                }
                timerText.text = TimeSpan.FromSeconds(currentSeconds).ToString(@"hh\:mm\:ss\:fff");
            }
                
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

    private void EndGame()
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
