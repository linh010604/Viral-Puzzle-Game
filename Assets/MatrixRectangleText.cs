using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  // Import TextMeshPro namespace
using UnityEngine.UI;  // For UI elements

public class MatrixRectangleText : MonoBehaviour
{
    public GameObject rectanglePrefab;  // Prefab for the rectangle (Panel) that will expand
    public GameObject textPrefab;       // Prefab for the TMP_Text (0, 1) inside the rectangle
    public RectTransform canvasRect;    // Reference to the Canvas RectTransform

    public float totalTime = 65f;       // Total time for the rectangle to expand
    public float textFallSpeed = 17f;        // Speed of the text falling inside the rectangle
    public float spawnInterval = 0.3f;       // Interval between spawning new text inside the rectangle
    public Vector2 initialSize = new Vector2(350, 100);  // Initial size of the rectangle
    public Vector2 maxSize = new Vector2(350, 500);     // Maximum size of the rectangle as it expands

    public float xOffset = 165f;  // The horizontal offset to apply to the text's X position

    private RectTransform rectangleRectTransform;  // RectTransform for the expanding rectangle
    private List<GameObject> fallingTexts = new List<GameObject>(); // List to hold all falling text
    private float elapsedTime = 0f;

    private void Start()
    {
        // Create the expanding rectangle (Panel)
        CreateExpandingRectangle();
    }

    private void Update()
    {
        if (rectangleRectTransform != null)
        {
            // Expand the height of the rectangle as time progresses
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / totalTime;  // Calculate the lerp factor (0 to 1 over time)
            rectangleRectTransform.sizeDelta = Vector2.Lerp(initialSize, maxSize, t);  // Expand size gradually

            // Move the falling text objects inside the rectangle
            for (int i = fallingTexts.Count - 1; i >= 0; i--)
            {
                GameObject textObj = fallingTexts[i];
                RectTransform textRect = textObj.GetComponent<RectTransform>();

                // Move the text down inside the rectangle
                textRect.anchoredPosition -= new Vector2(0, textFallSpeed * Time.deltaTime);

                // Check if the text has reached the bottom of the rectangle and remove it
                if (textRect.anchoredPosition.y < 0)  // Bottom of the rectangle is at Y=0
                {
                    Destroy(textObj);
                    fallingTexts.RemoveAt(i);
                }
            }
        }
    }

    private void CreateExpandingRectangle()
    {
        // Instantiate the rectangle (Panel) in the canvas
        GameObject rectangle = Instantiate(rectanglePrefab, canvasRect);

        // Get the RectTransform of the rectangle
        rectangleRectTransform = rectangle.GetComponent<RectTransform>();

        // Set its initial position to the top center of the canvas
        rectangleRectTransform.anchoredPosition = new Vector2(0, canvasRect.rect.height / 2);

        // Set its initial size to the defined small size
        rectangleRectTransform.sizeDelta = initialSize;

        // Anchor the panel correctly (top center of the canvas)
        rectangleRectTransform.anchorMin = new Vector2(0.5f, 1f);  // Top center anchor
        rectangleRectTransform.anchorMax = new Vector2(0.5f, 1f);  // Top center anchor
        rectangleRectTransform.pivot = new Vector2(0.5f, 1f);      // Set pivot to the top

        rectangleRectTransform.SetAsLastSibling();
        // Start spawning text inside the rectangle
        StartCoroutine(SpawnTextInsideRectangle(rectangleRectTransform));
    }

    private IEnumerator SpawnTextInsideRectangle(RectTransform parentRect)
    {
        while (rectangleRectTransform != null)
        {
            // Wait for the spawn interval
            yield return new WaitForSeconds(spawnInterval);

            // Create a new text object inside the rectangle
            GameObject newText = Instantiate(textPrefab, parentRect);

            // Set the text value to either "0" or "1"
            TMP_Text textComponent = newText.GetComponent<TMP_Text>();
            textComponent.text = Random.Range(0, 2) == 0 ? "0" : "1";

            // Set the initial position of the text at a random X position at the current top of the rectangle
            RectTransform textRect = newText.GetComponent<RectTransform>();

            // Adjust the X position to be within the bounds of the rectangle with an added offset
            float randomX = Random.Range(-parentRect.rect.width / 2, parentRect.rect.width / 2);
            randomX += xOffset;  // Apply the X offset

            // Adjust the Y position so that the text starts at the current top of the rectangle
            textRect.anchoredPosition = new Vector2(randomX, parentRect.rect.height);

            // Make sure the text is correctly scaled
            textRect.localScale = Vector3.one;

            // Add the text object to the list of falling texts
            fallingTexts.Add(newText);
        }
    }
}
