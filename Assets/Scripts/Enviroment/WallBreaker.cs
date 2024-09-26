using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBreaker : MonoBehaviour
{
    public int hitThreshold = 3; // Number of hít needed to break the wall
    private int hitCount = 0; // Track the number of hits

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            hitCount++;
            Debug.Log("Wall hit " + hitCount + " times.");

            if (hitCount >= hitThreshold)
            {
                BreakWall();
            }
        }    
    }

    private void BreakWall()
    {
        Debug.Log("Wall is breaking!");

        Destroy(gameObject);
    }    
}
