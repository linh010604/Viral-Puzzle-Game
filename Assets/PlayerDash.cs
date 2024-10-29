using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    public float dashSpeed = 20f;
    public float dashTime = 0.2f;
    public float dashCooldown = 2f;

    private Rigidbody rb;
    private bool isDashing = false;
    private float dashTimeCounter;
    private float dashCooldownCounter;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Dash();
    }

    void Dash()
    {
        if (dashCooldownCounter > 0)
            dashCooldownCounter -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownCounter <= 0 && !isDashing)
        {
            isDashing = true;
            dashTimeCounter = dashTime;
            dashCooldownCounter = dashCooldown;
        }

        if (isDashing)
        {
            dashTimeCounter -= Time.deltaTime;
            rb.velocity = transform.forward * dashSpeed;

            if (dashTimeCounter <= 0)
                isDashing = false;
        }
    }
}

