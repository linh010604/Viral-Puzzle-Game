using UnityEngine;
using TMPro;  

public class TimerSystem : MonoBehaviour
{
    public float TimeLeft = 180;
    public bool TimerOn = true;

    public TextMeshProUGUI TimerTxt;

    public GameObject LoserPage;   // Reference to the Loser UI Panel

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
