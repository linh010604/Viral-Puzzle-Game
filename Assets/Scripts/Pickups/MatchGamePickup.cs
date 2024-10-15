using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePickup : Pickup
{
    public GameObject matchWirePopUp; // Reference to the pop-up UI

    public MatchSystemManager matchSystem; // Game system

    public CursorManager cursorManager;

    public PlayerController player; // plyer

    public HintText dialogText; // Reference to hint dialog text

    public int lowerAmount = 3; // Amount to lower the hit threshold

    public WallBreaker wallBreaker; // Reference to the WallBreaker script

    private bool playerInRange = false;

    private bool activated = false;

    /// <summary>
    /// Description:
    /// When picked up, marks the goal as complete
    /// Inputs: Collider collision
    /// Outputs: N/A
    /// </summary>
    /// <param name="collision">The collider that caused this to be picked up</param>
    void Update()
    {
        // Check if the player is in range and e is pressed
        if (matchSystem.AllPaired() && playerInRange && !activated)
        {
            //Debug.Log("Button pressed!");
            activated = true;
            ActivatePuzzle();  // Call the puzzle activation function
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !activated)
        {
            player.SetSpeed();
            // Start the match when the player touches the trigger
            cursorManager.Puzzle();
            matchSystem.Start();
            matchWirePopUp.SetActive(true);
            //Debug.Log("Player near button. Press 'E' to interact.");
            playerInRange = true; // Player is in range
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !matchSystem.AllPaired())
        {
            playerInRange = false; // Player left the trigger area
        }
    }

    void ActivatePuzzle()
    {

        
        Debug.Log("Door opened!");

        // Lower the hit threshold of the wall
        if (wallBreaker != null)
        {
            if (wallBreaker.LowerHitThreshold(lowerAmount))
                dialogText.enterText = "Door opened. Let spread !";
            else
                dialogText.enterText = "Door almost opened. Hit it!";
        }
    }
}
