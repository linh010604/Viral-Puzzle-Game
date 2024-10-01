using UnityEngine;

public class SamplePuzzle : MonoBehaviour
{
    public GameObject door;  // Reference to the door or object that will be activated
    public bool isPressed = false;
    public TMPro.TMP_Text pressText; 
    private bool playerInRange = false; 
    public int lowerAmount = 3; // Amount to lower the hit threshold
    public WallBreaker wallBreaker; // Reference to the WallBreaker script
    void Update()
    {
        // Check if the player is in range and e is pressed
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !isPressed)
        {
            Debug.Log("Button pressed!");
            isPressed = true;
            ActivatePuzzle();  // Call the puzzle activation function
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPressed)
        {
            pressText.text = "Press 'E' to interact!";
            Debug.Log("Player near button. Press 'E' to interact.");
            playerInRange = true; // Player is in range
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")  && !isPressed )
        {
            pressText.text = "Something awesome!";
            Debug.Log("Player left the button.");
            playerInRange = false; // Player left the trigger area
        }
    }

    void ActivatePuzzle()
    {
        if (door != null)
        {
            door.SetActive(false);  // Deactivate the door to simulate opening it
            pressText.text = "Door opened!";
            Debug.Log("Door opened!");
        }
        else
        {
            Debug.Log("Puzzle activated!");
        }
        // Lower the hit threshold of the wall
        if (wallBreaker != null)
        {
            wallBreaker.LowerHitThreshold(lowerAmount);  
        }
    }
}
