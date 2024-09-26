using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int speed;

    private InputSystem_Actions controller;
    private Rigidbody rb;
    private Vector3 movement;

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
    }

    // Update is called once per frame
    void Update()
    {
        float x = controller.Player.Move.ReadValue<Vector2>().x;
        float z = controller.Player.Move.ReadValue<Vector2>().y;

        Debug.Log(x + "," + z);

        movement = new Vector3(x, 0, z).normalized;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + movement * speed * Time.fixedDeltaTime);
    }
}
