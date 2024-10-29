using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertMeter : MonoBehaviour
{
    public float maxSensitive = 100.0f;  // Max sensitivity value
    private float minSensitive = 0.0f;   // Minimum sensitivity (20% of maxSensitive)
    public float sensitive = 0.0f;       // Current sensitivity
    public float increaseRate = 5.0f;    // Sensitivity increase rate
    public float decreaseRate = 35.0f;   // Sensitivity decrease rate
    private bool gameEnded = false;       // Flag to indicate if the game has ended
    public int numSections = 11;         // Number of sections in the alert meter
    public Color[] sectionColors;        // Colors for each section of the alert meter
    public Vector2 sectionSize = new Vector2(65, 20); // Size of each rectangle (width, height)
    public float sectionSpacing = 10f;   // Space between each rectangle

    public Sprite borderSprite;          // The 9-slice sprite for the border
    public PlayerController playerController; // Reference to the PlayerController for movement
    private RectTransform[] alertSections;    // Temp array to store the alert sections

    private Vector2 initialSize;
    private float previousSensitive;      // Store previous sensitivity value

    public Animator alertAnimator;
    string animatorBoolName = "status";
    void Start()
    {
        alertSections = new RectTransform[numSections];

        for (int i = 0; i < numSections; i++)
        {
            GameObject section = new GameObject("AlertBlock" + i);
            section.transform.SetParent(this.transform);  // Set the parent to the current GameObject

            RectTransform rect = section.AddComponent<RectTransform>();
            rect.sizeDelta = sectionSize; 
            rect.anchorMin = new Vector2(0.5f, 0f);   
            rect.anchorMax = new Vector2(0.5f, 0f);   
            rect.pivot = new Vector2(0.5f, -0.5f);        
            float yPos = (sectionSize.y - sectionSpacing) * (i);  // Calculate the y position based on index
            rect.anchoredPosition = new Vector2(0, yPos);  

            // Add Image component and set the 9-slice sprite
            Image img = section.AddComponent<Image>();
            img.sprite = borderSprite;       // Assign the 9-slice sprite with transparent center
            img.type = Image.Type.Sliced;    // Set image type to Sliced for 9-slice behavior
            img.color = sectionColors[i];    // Set color for the border

            alertSections[i] = rect;
        }

        initialSize = sectionSize;
        previousSensitive = sensitive;  // Initialize previousSensitive to the starting value of sensitive
    }
    public void TakeDamage(float damage)
    {
        sensitive += damage;
    }
    void Update()
    {
        if (gameEnded)
        {
            return;
        }
        if (playerController.DialogueUI.IsOpen) 
        {
            // IDLE (STOPPED)
            alertAnimator.speed = 0;
            return;
        }
        alertAnimator.speed = 1;
        float currentSensitive = sensitive;

        if (playerController.isMovingForward)
        {
            sensitive = Mathf.Clamp(sensitive + increaseRate * Time.deltaTime, minSensitive, maxSensitive);
        }
        else
        {
            sensitive = Mathf.Clamp(sensitive - decreaseRate * Time.deltaTime, minSensitive, maxSensitive);
        }

        // Determine the direction of sensitivity change
        if (sensitive > currentSensitive)
        {
            alertAnimator.SetInteger(animatorBoolName, 1);
        }
        else if (sensitive < currentSensitive)
        {
            alertAnimator.SetInteger(animatorBoolName, -1);
        }
        else
        {
            alertAnimator.SetInteger(animatorBoolName, 0);
        }


        float sensitivityThreshold = maxSensitive * 0.2f;
        if (sensitive >= sensitivityThreshold)
        {
            float shakeIntensity = (sensitive - sensitivityThreshold) / sensitivityThreshold;  
            CinemachineCameraShake.Instance.ShakeCamera(shakeIntensity, 1.5f);  
        }
        else
        {
            CinemachineCameraShake.Instance.ShakeCamera(0, 0); 
        }

        UpdateAlertMeter();

        previousSensitive = sensitive;

        if (sensitive >= maxSensitive)
        {
            // Game over
            if (GameManager.instance != null)
            {
                GameManager.instance.GameOver();
                gameEnded = true;
            }
            Time.timeScale = 0f;  // Freeze the game
            Debug.Log("Game Over");
        }
    }

    void UpdateAlertMeter()
    {
        // Adjust sensitivity percentage to account for the minimum sensitivity
        float adjustedSensitivity = (sensitive - minSensitive) / (maxSensitive - minSensitive);

        // Calculate how many sections should be shown based on the adjusted sensitivity percentage
        int sectionsToShow = Mathf.CeilToInt(adjustedSensitivity * numSections);

        // Ensure that the minimum sensitivity always shows at least some sections
        sectionsToShow = Mathf.Max(sectionsToShow, Mathf.CeilToInt((minSensitive / maxSensitive) * numSections));

        for (int i = 0; i < numSections; i++)
        {
            if (i < sectionsToShow)
            {
                // Show the section
                alertSections[i].sizeDelta = new Vector2(initialSize.x, initialSize.y);
            }
            else
            {
                // Hide the section
                alertSections[i].sizeDelta = Vector2.zero;
            }
        }
    }
    public void IncreaseMinSenstive(float newMin)
    {
        minSensitive += newMin;
    }
}
