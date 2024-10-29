using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryPuzzlePickup : Pickup
{
    public GameObject memoryPuzzle; // Reference to the pop-up UI
    private TileGenerator tileGenerator; // Reference to the TileGenerator script
    public PlayerController player; // plyer
    public int lowerAmount = 3; // Amount to lower the hit threshold
    public CursorManager cursorManager;
    private bool playerInRange = false;
    private bool activated = false;
    public WallBreaker wallBreaker; // Reference to the WallBreaker script
    // Start is called before the first frame update
    void Start()
    {
        tileGenerator = memoryPuzzle.GetComponent<TileGenerator>();
    }

    // Update is called once per frame
    void Update()
    {

        if (playerInRange && !activated && tileGenerator != null && tileGenerator.IsCompleted() )
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
            cursorManager.Puzzle();
            memoryPuzzle.SetActive(true);
            playerInRange = true; // Player is in range
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false; // Player left the trigger area
            player.RecoverSpeed();
        }
    }
    void ActivatePuzzle()
    {
        // Lower the hit threshold of the wall
        if (wallBreaker != null)
        {
            wallBreaker.LowerHitThreshold(lowerAmount);
        }
    }
}
