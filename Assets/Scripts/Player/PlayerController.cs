using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int speed;
    private int initialSpeed;
    private bool canMoveCamera = true;
    private InputSystem_Actions controller;
    public Transform cam;
    private Rigidbody rb;
    private Vector3 movement;
    
    private Vector3 lastPosition;  // Store the player's last position
    public float turnSmoothTime = 0.1f;
    public CinemachineFreeLook freeLookCamera; // Reference to the Cinemachine FreeLook camera
    private float turnSmoothVelocity;
    // Track whether the player is moving forward
    public bool isMovingForward = false;

    [SerializeField] private DialogueUI dialogueUI;
    

    public DialogueUI DialogueUI => dialogueUI;
    
    public IInteractable Interactable { get; set; }

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
        if (canMoveCamera)
        {
            EnableFreeLookCameraControl(true); // Ensure camera control is enabled when allowed
        }
        else
        {
            EnableFreeLookCameraControl(false); // Disable camera control when UI is open
        }
        
        float x = controller.Player.Move.ReadValue<Vector2>().x;
        float y = controller.Player.Move.ReadValue<Vector2>().y;

        // Store the direction the player is trying to move in
        movement = new Vector3(x, 0, y).normalized;
    }

private void FixedUpdate()
{
    if (dialogueUI.IsOpen) return;

    // If there is no movement input, don't change rotation or movement
    if (movement.magnitude >= 0.1f)
    {
        float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(rb.transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        rb.rotation = Quaternion.Euler(0f, angle, 0f);
        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        
        // Apply movement using rb.velocity instead of MovePosition
        rb.velocity = moveDir * speed ;
    }
    else
    {
        // If there's no movement input, stop horizontal movement, but keep gravity intact
        rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
    }

    // Check if the player's position has changed 
    if (Vector3.Distance(transform.position, lastPosition) > 0.01f)
    {
        isMovingForward = true;
    }
    else
    {
        isMovingForward = false;
    }

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
        DisableCameraControl();

        speed = 0;
    }

    public void RecoverSpeed()
    {
        EnableCameraControl();
        speed = initialSpeed;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IInteractable interactable))
        {
            Interactable?.Interact(this);
        }
    }
        // Method to disable camera control
    public void DisableCameraControl()
    {
        canMoveCamera = false;
        Debug.Log("Camera control disabled");   
    }

    // Method to enable camera control
    public void EnableCameraControl()
    {
        canMoveCamera = true;
    }

    // Enable or disable the camera's input axes based on state
    private void EnableFreeLookCameraControl(bool isEnabled)
    {
        // If using Cinemachine FreeLook, control its input by setting the axis names to null when disabled
        if (freeLookCamera != null)
        {
            if (isEnabled)
            {
                // Enable the axis input for camera control
                freeLookCamera.m_XAxis.m_MaxSpeed = 450f;
                freeLookCamera.m_YAxis.m_MaxSpeed = 0f;
            }
            else
            {
                // Set the X and Y axes speed to 0 to disable camera movement
                freeLookCamera.m_XAxis.m_MaxSpeed = 0f;
                freeLookCamera.m_YAxis.m_MaxSpeed = 0f;
            }
        }
    }
}

