using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertMeter : MonoBehaviour
{
    public float maxSensitive = 100.0f;  // Max sensitivity value
    public float sensitive = 0.0f;       // Current sensitivity
    public float increaseRate = 5.0f;   // Sensitivity increase rate
    public float decreaseRate = 35.0f;   // Sensitivity decrease rate

    public int numSections = 11;          // Number of sections in the alert meter
    public Color[] sectionColors;        // Colors for each section of the alert meter
    public Vector2 sectionSize = new Vector2(65, 20); // Size of each rectangle (width, height)
    public float sectionSpacing = 10f;    // Space between each rectangle

    public Sprite borderSprite;           // The 9-slice sprite for the border
    public PlayerController playerController; // Reference to the PlayerController for movement
    private RectTransform[] alertSections;    // Temp array to store the alert sections

    private Vector2 initialSize;

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
    }

    void Update()
    {
        if (playerController.isMovingForward)
        {
            sensitive = Mathf.Clamp(sensitive + increaseRate * Time.deltaTime, 0, maxSensitive);
        }
        else
        {
            sensitive = Mathf.Clamp(sensitive - decreaseRate * Time.deltaTime, 0, maxSensitive);
        }

        UpdateAlertMeter();
    }

    void UpdateAlertMeter()
    {
        float sensitivityPercentage = sensitive / maxSensitive;

        int sectionsToShow = Mathf.CeilToInt(sensitivityPercentage * numSections);

        for (int i = 0; i < numSections; i++)
        {
            if (i < sectionsToShow)
            {
                // Enable the rectangle (set to its original size)
                alertSections[i].sizeDelta = new Vector2(initialSize.x, initialSize.y);
            }
            else
            {
                // Hide the section (set size to zero)
                alertSections[i].sizeDelta = Vector2.zero;
            }
        }
    }
}
