using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBreaker : MonoBehaviour
{
    public int hitThreshold = 3; // Number of hits needed to break the wall
    private int hitCount = 0; // Track the number of hits

    public float sensitivityIncreaseAmount = 10f; // Amount to increase sensitivity per hit
    public float cooldownTime = 2f; // Time before sensitivity starts decreasing after no hits
    private bool isCooldownActive = false; // Cooldown flag

    private AlertMeter alertMeter; // Reference to the AlertMeter script
    private Coroutine cooldownCoroutine; // Store the cooldown coroutine

    private void Start()
    {
        // Find the AlertMeter by tag
        GameObject alertObject = GameObject.FindWithTag("AlertMeter");

        if (alertObject != null)
        {
            alertMeter = alertObject.GetComponent<AlertMeter>();
        }
    }

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
            else if (alertMeter != null) // No need to check cooldown, always increase sensitivity on hit
            {
                IncreaseSensitivity();
                
                // Restart cooldown timer each time the player hits the wall
                if (cooldownCoroutine != null)
                {
                    StopCoroutine(cooldownCoroutine); // Stop the existing cooldown if a hit happens before it ends
                }
                cooldownCoroutine = StartCoroutine(SensitivityCooldown());
            }
        }
    }

    private void BreakWall()
    {
        Debug.Log("Wall is breaking!");
        Destroy(gameObject);
    }

    private void IncreaseSensitivity()
    {
        if (alertMeter != null)
        {
            alertMeter.sensitive = Mathf.Clamp(alertMeter.sensitive + sensitivityIncreaseAmount, 0, alertMeter.maxSensitive);
            Debug.Log("increased to: " + alertMeter.sensitive);
        }
    }

    private IEnumerator SensitivityCooldown()
    {
        isCooldownActive = true;
        Debug.Log("Starting cooldown...");

        // Wait for the cooldown period to finish
        yield return new WaitForSeconds(cooldownTime);

        isCooldownActive = false;
        Debug.Log("Cooldown ended.");
    }
}
