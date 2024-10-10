using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryPuzzlePickup : Pickup
{
    public GameObject memoryPuzzle; // Reference to the pop-up UI

    public PlayerController player; // plyer


    public CursorManager cursorManager;


    private bool playerInRange = false;
    private bool activated = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !activated)
        {
            // Start the match when the player touches the trigger
            player.SetSpeed();
            cursorManager.Puzzle();
            memoryPuzzle.SetActive(true);
            //Debug.Log("Player near button. Press 'E' to interact.");
            playerInRange = true; // Player is in range
        }
    }
}
