using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private InputSystem_Actions controller;

    private void Awake() 
    {
        controller = new InputSystem_Actions();
    }

    private void OnEnable() 
    {
        controller.Enble();
    }

    // Update is called once per frame
    void Update()
    {
        float x = controller.Player.Move.ReadValue<Vector2>().x;
    }
}
