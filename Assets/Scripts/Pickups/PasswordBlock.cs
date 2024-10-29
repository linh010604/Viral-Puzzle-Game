using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasswordBlock : MonoBehaviour
{
    public GameObject passwordPage; // Reference to the pop-up UI

    public CursorManager cursorManager;

    public PlayerController player; // plyer

    public int lowerAmount = 3; // Amount to lower the hit threshold

    public WallBreaker wallBreaker; // Reference to the WallBreaker script

    public KeyPad keyPad;

    private bool playerinRange = false;
    private bool activated = false;

    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player") )
        {
           
            playerinRange = true;

        }
    }

    void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Player") )
        {
            player.RecoverSpeed();
            playerinRange = false;

        }
    }

    void Update()
    {
        if (playerinRange && Input.GetKeyDown(KeyCode.P) && !keyPad.complete)
        {
            player.SetSpeed();
            // Start the match when the player touches the trigger
            cursorManager.Puzzle();
            passwordPage.SetActive(true);
        }
        else if (keyPad.complete & !activated)
        {
            activated = true;
            ActivatePuzzle();
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
