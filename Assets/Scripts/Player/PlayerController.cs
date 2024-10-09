using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int speed;
    private int initialSpeed;

    private InputSystem_Actions controller;
    private Rigidbody rb;
    private Vector3 movement;
    private Vector3 lastPosition;  // Store the player's last position

    // Track whether the player is moving forward
    public bool isMovingForward = false;

    private void Awake()
    {
        controller = new InputSystem_Actions();
        initialSpeed = speed;
    }

    private void OnEnable()
    {
        controller.Enable();
    }

    void OnDisable()
    {
        // Disable the player input actions when the object is inactive
        controller.Disable();
    }

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        lastPosition = transform.position;
        initialSpeed = speed;
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

    // Clean up the input system
    private void OnDestroy()
    {
        // Make sure to disable the input system when this object is destroyed
        controller.Disable();
    }

    public void SetSpeed()
    {
        Debug.Log("stupid");
        speed = 0;
    }

    public void RecoverSpeed()
    {
        speed = initialSpeed;
    }
}
