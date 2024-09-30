using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int speed;

    private InputSystem_Actions controller;
    private Rigidbody rb;
    private Vector3 movement;
    private Vector3 lastPosition;  // Store the player's last position

    // Track whether the player is moving forward
    public bool isMovingForward = false;

    private void Awake()
    {
        controller = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        controller.Enable();
    }

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        lastPosition = transform.position;  
    }

    void Update()
    {
        float x = controller.Player.Move.ReadValue<Vector2>().x;
        float z = controller.Player.Move.ReadValue<Vector2>().y;

        // Store the direction the player is trying to move in
        movement = new Vector3(x, 0, z).normalized;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + movement * speed * Time.fixedDeltaTime);

        // Check if the player's position has changed 
        if (Vector3.Distance(transform.position, lastPosition) > 0.01f)  // A small amount to detect movement
        {
            isMovingForward = true;
        }
        else
        {
            isMovingForward = false;
        }

        // Update lastPosition to the current position
        lastPosition = transform.position;
    }
}
