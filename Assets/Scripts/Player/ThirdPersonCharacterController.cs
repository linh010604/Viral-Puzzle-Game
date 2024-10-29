using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI; 

/// <summary>
/// This class handles the control of a third person character controller.
/// 
/// Motion is based on the direction the player(user) expects to move 
/// i.e. left and right are based on the camera view and not the direction the player's character is facing
/// </summary>
public class ThirdPersonCharacterController : MonoBehaviour
{
    // the character controller used for player motion
    private CharacterController characterController;
    // the health used by the player
    private Health playerHealth;

    public bool isFlyingActive = false;

    // Enum to handle the player's state
    public enum PlayerState { Idle, Moving, Jumping, DoubleJumping, Falling, Dead };

    [Header("State Information")]
    [Tooltip("The state the player controller is currently in")]
    public PlayerState playerState = PlayerState.Idle;

    [Header("Input Actions & Controls")]
    [Tooltip("The input action(s) that map to player movement")]
    public InputAction moveInput;
    [Tooltip("The input action(s) that map to jumping")]
    public InputAction jumpInput;

    [Header("Effects settings")]
    [Tooltip("The effect to create when jumping")]
    public GameObject jumpEffect;
    [Tooltip("The effect to create when double jumping")]
    public GameObject doubleJumpEffect;
    [Tooltip("The effec to create when the player lands on the ground")]
    public GameObject landingEffect;

    [Header("Following Game Objects")]
    [Tooltip("The following scripts of game objects that should follow this controller after it is done moving each frame")]
    public List<FollowLikeChild> followers;


    [Header("Dash Settings")]
    [Tooltip("The speed boost applied during dash")]
    public float dashSpeed = 20f;
    [Tooltip("The duration of the dash")]
    public float dashDuration = 0.2f;
    [Tooltip("Cooldown between dashes")]
    public float dashCooldown = 2f;

    private bool isDashing = false;
    private float dashTimeCounter;
    private float dashCooldownCounter;


    [Header("Speed Boost Settings")]
    [Tooltip("The multiplier for the speed during the boost")]
    public float speedBoostMultiplier = 2f; 
    [Tooltip("The duration of the speed boost")]
    public float speedBoostDuration = 5f; 
    [Tooltip("Cooldown between speed boosts")]
    public float speedBoostCooldown = 10f; 

    private bool isSpeedBoosting = false;
    private float speedBoostTimeCounter;
    private float speedBoostCooldownCounter;


    [Header("Fly Settings")]
    public float flySpeed = 10f;           // Movement speed while flying
    public float flyAscendSpeed = 5f;      
    private bool isFlying = false;         // Whether the player is currently flying




    /// <summary>
    /// Standard Unity function called whenever the attached gameobject is enabled
    /// </summary>
    private void OnEnable()
    {
        moveInput.Enable();
        jumpInput.Enable();
    }

    public void EnableFlying()
    {
        isFlyingActive = true;
    }

    /// <summary>
    /// Standard Unity function called whenever the attached gameobject is disabled
    /// </summary>
    private void OnDisable()
    {
        moveInput.Disable();
        jumpInput.Disable();
    }

    /// <summary>
    /// Description:
    /// Standard Unity function that is called before the first frame
    /// Input:
    /// none
    /// Return:
    /// void
    /// </summary>
    void Start()
    {
        InitialSetup();
        
        // Set the slider's max value to the speed boost duration
        if (speedBoostSlider != null)
        {
            speedBoostSlider.maxValue = speedBoostDuration;
            speedBoostSlider.value = speedBoostDuration;  
        }
    }


    /// <summary>
    /// Description:
    /// Checks for and gets the needed refrences for this script to run correctly
    /// Input:
    /// none
    /// Return:
    /// void
    /// </summary>
    void InitialSetup()
    {
        characterController = gameObject.GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("There is no character controller attached to the same gameobject as the ThirdPersonCharacterController. + " +
                "\n It needs one to run correctly");
        }

        if (GetComponent<Health>() == null)
        {
            Debug.LogError("There is no health script attached to the player!\n" + "There needs to be a health script on the same" +
                "object as the Third Person Character Controller");
        }
        else
        {
            playerHealth = GetComponent<Health>();
        }

        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().useGravity = false;
        }
    }
    private void ToggleFlyMode()
    {
        if (!isFlyingActive)
        {
            return;
        }
        isFlying = !isFlying;

        // if (isFlying)
        // {
        //     moveDirection = Vector3.zero;  
        // }
    }

    private void FlyMovement()
    {
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        float horizontalInput = moveInput.ReadValue<Vector2>().x;
        float verticalInput = moveInput.ReadValue<Vector2>().y;

        Vector3 moveDirection = (cameraForward * verticalInput + cameraRight * horizontalInput) * flySpeed;

        // Ascend and descend using space (ascend) and left control (descend)
        if (Input.GetKey(KeyCode.Space))
        {
            moveDirection.y = flyAscendSpeed;  // Ascend
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            moveDirection.y = -flyAscendSpeed;  // Descend
        }
        else
        {
            moveDirection.y = 0;  // Stay at the same height
        }

        // Move the player in flying mode

        characterController.Move(moveDirection * Time.deltaTime);
        foreach (FollowLikeChild follower in followers)
        {
            follower.FollowParent();
        }
        playerState = PlayerState.Falling;
    }


    /// <summary>
    /// Description:
    /// Standard Unity function called once per frame
    /// Inputs:
    /// none
    /// Retuns:
    /// void
    /// </summary>
    void LateUpdate()
    {
        // Don't do anything if the game is paused
        if (Time.timeScale == 0)
        {
            return;
        }

        // Toggle fly mode when pressing 'F'
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleFlyMode();  // Toggle fly mode when the 'F' key is pressed
        }

        MatchCameraYRotation();
        HandleDash();
        HandleSpeedBoost();

        // Check if flying, if true, use fly movement, otherwise, use normal movement
        if (isFlying)
        {
            FlyMovement();  // Handle flying movement
        }
        else
        {
            CentralizedControl(moveInput.ReadValue<Vector2>().x, moveInput.ReadValue<Vector2>().y, jumpInput.triggered);  // Normal movement
        }
    }

    void HandleDash()
    {
        if (dashCooldownCounter > 0)
            dashCooldownCounter -= Time.deltaTime;

        // Check for dash input (Left Shift) and initiate dash if conditions are met
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownCounter <= 0 && !isDashing)
        {
            isDashing = true;
            dashTimeCounter = dashDuration;
            dashCooldownCounter = dashCooldown;
        }

        // If dashing, apply dash speed
        if (isDashing)
        {
            dashTimeCounter -= Time.deltaTime;

            Vector3 dashDirection = transform.forward * dashSpeed;

            characterController.Move(dashDirection * Time.deltaTime);

            // Stop dashing when the duration ends
            if (dashTimeCounter <= 0)
            {
                isDashing = false;
            }
        }
    }
    void HandleSpeedBoost()
    {
        if (isSpeedBoosting)
        {
            speedBoostTimeCounter -= Time.deltaTime;

            if (speedBoostSlider != null)
                speedBoostSlider.value = speedBoostTimeCounter;

            if (speedBoostTimeCounter <= 0)
            {
                isSpeedBoosting = false;
                moveSpeed /= speedBoostMultiplier; // Reset to normal speed

                speedBoostCooldownCounter = speedBoostCooldown;

                // Update the slider's max value to reflect the cooldown period
                if (speedBoostSlider != null)
                {
                    speedBoostSlider.maxValue = speedBoostCooldown; //
                    speedBoostSlider.value = 0; // Start from 0 for cooldown
                }

                // Update text to indicate cooldown
                if (speedBoostText != null)
                    speedBoostText.text = "Speed Boost Cooling Down...";
            }
        }
        else if (!isSpeedBoosting && speedBoostCooldownCounter > 0)
        {
            speedBoostCooldownCounter -= Time.deltaTime;

            if (speedBoostSlider != null)
            {
                speedBoostSlider.value = (speedBoostCooldown - speedBoostCooldownCounter) / speedBoostCooldown * speedBoostSlider.maxValue;
            }

            if (speedBoostCooldownCounter <= 0)
            {
                if (speedBoostText != null)
                    speedBoostText.text = "Speed Boost Ready!";

                // Reset slider to full to show that the boost is available
                if (speedBoostSlider != null)
                    speedBoostSlider.value = speedBoostSlider.maxValue;
            }
        }

        // Check for speed boost input ("B" key) and initiate boost if conditions are met
        if (Input.GetKeyDown(KeyCode.B) && speedBoostCooldownCounter <= 0 && !isSpeedBoosting)
        {
            isSpeedBoosting = true;
            speedBoostTimeCounter = speedBoostDuration;
            moveSpeed *= speedBoostMultiplier; // Apply the speed boost multiplier

            // Set the slider's max value to the boost duration
            if (speedBoostSlider != null)
            {
                speedBoostSlider.maxValue = speedBoostDuration; // Change the slider's max value to the boost duration
                speedBoostSlider.value = speedBoostDuration;    // Start the slider filled during the boost
            }

            // Update text to show the boost is active
            if (speedBoostText != null)
                speedBoostText.text = "Speed Boost Active!";
        }
    }





    [Header("Related Gameobjects / Scripts needed for determining control states")]
    [Tooltip("The player camera gameobject, used to manage the controller's rotations")]
    public GameObject playerCamera;
    [Tooltip("The player character's representation (model)")]
    public PlayerRepresentation playerRepresentation;

    /// <summary>
    /// Description:
    /// Makes the character controller's rotation along the Y match the camera's
    /// Input:
    /// none
    /// Return:
    /// void
    /// </summary>
    void MatchCameraYRotation()
    {
        if (playerCamera != null)
        {
            this.gameObject.transform.rotation = Quaternion.Euler(0, playerCamera.transform.rotation.eulerAngles.y, 0);
        }

    }

    [Header("Speed Control")]
    [Tooltip("The speed at which to move the player")]
    public float moveSpeed = 5f;
    [Tooltip("The strength with which to jump")]
    public float jumpStrength = 8.0f;
    [Tooltip("The strength of gravity on this controller")]
    public float gravity = 20.0f;


    [Header("UI Elements")]
    public Slider speedBoostSlider;  // The slider that indicates speed boost duration
    public Text speedBoostText;
    [Tooltip("The amount of downward movement to register as falling")]
    public float fallAmount = -9.0f;

    // The direction the player is moving in
    private Vector3 moveDirection = Vector3.zero;

    /// <summary>
    /// Description:
    /// Handles swithing between control styles if more than one is coded in
    /// Inputs:
    /// float leftCharacterMovement | float rightCharacterMovement | bool jumpPressed
    /// Returns:
    /// void
    /// </summary>
    /// <param name="leftCharacterMovement">The movement input along the horizontal</param>
    /// <param name="rightCharacterMovement">The movement input along the vertical</param>
    /// <param name="jumpPressed">"Whether or not the jump input has been pressed"</param>
    void CentralizedControl(float leftRightMovementAxis, float forwardBackwardMovementAxis, bool jumpPressed)
    {
        if (playerHealth.currentHealth <= 0)
        {
            DeadControl();
        }
        else
        {
            NormalControl(leftRightMovementAxis, forwardBackwardMovementAxis, jumpPressed);
        }
    }

    // Whether or not the double jump is currently available
    bool doubleJumpAvailable = false;

    [Header("Jump Timing")]
    [Tooltip("How long to be lenient for when the player becomes ungrounded")]
    public float jumpTimeLeniency = 0.1f;
    // When to stop being lenient
    float timeToStopBeingLenient = 0;

    bool landed = false;

    // Horizontal force to apply on bounce
    float xForce = 0f;
    float zForce = 0f;

    /// <summary>
    /// Description:
    /// Handles motion of the player representation under the average or normal use case
    /// Inputs:
    /// float leftCharacterMovement | float rightCharacterMovement | bool jumpPressed
    /// Returns:
    /// void
    /// </summary>
    /// <param name="leftCharacterMovement">The movement input along the horizontal</param>
    /// <param name="rightCharacterMovement">The movement input along the vertical</param>
    /// <param name="jumpPressed">"Wheter or not the jump input has been pressed"</param>
    void NormalControl(float leftRightMovementAxis, float forwardBackwardMovementAxis, bool jumpPressed)
    {
        // The input corresponding to the left and right movement
        float leftRightInput = leftRightMovementAxis;
        // The input corresponding to the forward and backward movement
        float forwardBackwardInput = forwardBackwardMovementAxis;

        // If the controller is grounded
        if (characterController.isGrounded && !bounced)
        {
            // Reset the time to stop being lenient
            timeToStopBeingLenient = Time.time + jumpTimeLeniency;

            // reset horizontal forces
            xForce = 0f;
            zForce = 0f;

            // make the double jump available again
            doubleJumpAvailable = true;

            if (!landed && landingEffect != null && moveDirection.y <= fallAmount)
            {
                landed = true;
                Instantiate(landingEffect, transform.position, transform.rotation, null);
            }

            // set the move direction based on the input
            moveDirection = new Vector3(leftRightInput, 0, forwardBackwardInput);

            // transform the movement direction to be in world space (because we want to move in relation to the world not ourselves)
            moveDirection = transform.TransformDirection(moveDirection);

            // Apply the movement speed to the movement direction
            moveDirection *= moveSpeed;

            // If the player has pressed the jump button, apply to the y movement the jump strength
            if (jumpPressed)
            {
                moveDirection.y = jumpStrength;
                if (jumpEffect != null)
                {
                    Instantiate(jumpEffect, transform.position, transform.rotation, null);
                }
                playerState = PlayerState.Jumping;
            }
            else if (moveDirection == Vector3.zero)
            {
                playerState = PlayerState.Idle;
            }
            else
            {
                playerState = PlayerState.Moving;
            }

        }
        // When we are not grounded...
        else
        {
            // Apply move direction with the input and move speed to the x and z
            // Apply the previous move direction y to this current move direction y
            moveDirection = new Vector3(leftRightInput * moveSpeed + xForce, moveDirection.y, forwardBackwardInput * moveSpeed + zForce);
            // transform the movement direction to be in world space (because we want to move in relation to the world not ourselves)
            moveDirection = transform.TransformDirection(moveDirection);

            // If the jump is pressed and we are still being lenient apply the jump
            if (jumpPressed && Time.time < timeToStopBeingLenient)
            {
                moveDirection.y = jumpStrength;
                if (jumpEffect != null)
                {
                    Instantiate(jumpEffect, transform.position, transform.rotation, null);
                }
                playerState = PlayerState.Jumping;
            }
            // otherwise, check for the double jump..
            else if (jumpPressed && doubleJumpAvailable)
            {
                // Apply the double jump and make it unavailable
                moveDirection.y = jumpStrength;
                doubleJumpAvailable = false;
                if (doubleJumpEffect != null)
                {
                    Instantiate(doubleJumpEffect, transform.position, transform.rotation, null);
                }
                playerState = PlayerState.DoubleJumping;
            }
            else if (moveDirection.y < -5.0f)
            {
                bounced = false;
                landed = false;
                playerState = PlayerState.Falling;
            }
        }
        // Apply gravity with Time.deltaTime (effectively applied again to make it an acceleration)
        moveDirection.y -= gravity * Time.deltaTime;

        // If we are grounded and the y movedirection is negative reset it to something small
        // This avoids building up a large negative motion along the y direction
        if (characterController.isGrounded && moveDirection.y < 0)
        {
            moveDirection.y = -0.3f;
        }

        // Pass the calculated move direction multiplied by the time inbetween freames to the charater controller move function
        characterController.Move(moveDirection * Time.deltaTime);


        // Make all assigned followers do their following of the player now
        foreach (FollowLikeChild follower in followers)
        {
            follower.FollowParent();
        }
    }

    bool bounced = false;
    /// <summary>
    /// Description:
    /// Bounces the player upwards with some multiplier by the jump strength
    /// Input:
    /// float bounceForceMultiplier | float bounceJumpButtonHeldMultiplyer
    /// Output:
    /// void
    /// </summary>
    /// <param name="bounceForceMultiplier">The force to multiply jump strength by when bounce is called</param>
    /// <param name="bounceJumpButtonHeldMultiplyer">The force to multiply jump strength by when bounce is called and the jump button is held down</param>
    public void Bounce(float bounceForceMultiplier, float bounceJumpButtonHeldMultiplyer, bool applyHorizontalForce)
    {
        bounced = true;
        playerState = PlayerState.Jumping;
        if (jumpInput.ReadValue<float>() != 0)
        {
            moveDirection.y = jumpStrength * bounceJumpButtonHeldMultiplyer;
        }
        else
        {
            moveDirection.y = jumpStrength * bounceForceMultiplier;
        }

        if (applyHorizontalForce)
        {
            float horizontalForce = moveDirection.y * 0.5f;
            xForce = Random.Range(-horizontalForce, horizontalForce);
            zForce = Random.Range(-horizontalForce, horizontalForce);
        }
    }

    /// <summary>
    /// Description:
    /// Control when the player is dead
    /// Input:
    /// none
    /// Return:
    /// void
    /// </summary>
    void DeadControl()
    {
        playerState = PlayerState.Dead;
        moveDirection = new Vector3(0, moveDirection.y, 0);
        moveDirection = transform.TransformDirection(moveDirection);

        moveDirection.y -= gravity * Time.deltaTime;

        characterController.Move(moveDirection * Time.deltaTime);

        // Make all assigned followers do their following of the player now
        foreach (FollowLikeChild follower in followers)
        {
            follower.FollowParent();
        }
    }

    /// <summary>
    /// Description:
    /// Resets the double jump of the player
    /// Input:
    /// None
    /// Return:
    /// void (no return)
    /// </summary>
    public void ResetJumps()
    {
        doubleJumpAvailable = true;
    }

    /// <summary>
    /// Description:
    /// Moved the player to a specified position
    /// Input:
    /// Vector3 of the newPosition to move to
    /// Return:
    /// void (no return)
    /// </summary>
    public void MoveToPosition(Vector3 newPosition)
    {
        // must turn off the Character Controller prior to moving, as when on, the Character Controller controls all movement
        characterController.enabled = false;

        // reposition the player
        transform.position = newPosition;

        // turn character controller back on
        characterController.enabled = true;
    }

}
