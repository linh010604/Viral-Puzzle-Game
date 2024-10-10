using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryManager : MonoBehaviour
{
    public PlayerController player; // plyer
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void DisableCanvas()
    {
            player.RecoverSpeed();
            this.gameObject.SetActive(false);  // Disable the match block so it doesn't pop up again
    }
}
