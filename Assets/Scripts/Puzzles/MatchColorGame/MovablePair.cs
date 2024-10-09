using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovablePair : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Port connectedPort;
    private Vector3 initialPosition;
    public RectTransform rectTransform;
    private bool connected;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        //initialPosition = rectTransform.anchoredPosition;

        // Ensure the sprite has a BoxCollider for interaction
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Optionally do something when drag starts
        //rectTransform.position = Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {

        if (!connected)
        {
            //rectTransform.position = Input.mousePosition;
            rectTransform.anchoredPosition += eventData.delta;
        }
        else if (Vector3.Distance(rectTransform.position, Input.mousePosition) > 2)
        {
            connected = false;
        }

        // Check for collision with any Port UI elements
        //CheckCollision();

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!connected)
        {
            ResetPosition();
        }
    }

    public Vector3 GetPosition()
    {
        return rectTransform.position;//transform.position;
    }

    public void SetInitialPosition(Vector3 newPosition)
    {
        initialPosition = newPosition;
        rectTransform.position = initialPosition;
    }

    public void ResetPosition()
    {
        //transform.position = initialPosition;
        rectTransform.position = initialPosition; // Reset to initial position
        if (connectedPort)
        {
            connectedPort.gameObject.SetActive(true);
            connectedPort.Reset(this);
            connectedPort = null;
        }
        connected = false;
    }

    public void Collide(Port collidedPort)
    {
        connected = true; // Mark as connected if overlapping
        Debug.Log($"{gameObject.name} is colliding with {collidedPort.name}.");

        // Snap to port position or handle matching logic here
        rectTransform.anchoredPosition = collidedPort.GetComponent<RectTransform>().anchoredPosition; // Snap to port position
        connectedPort = collidedPort;
        collidedPort.gameObject.SetActive(false);
        //break; // Exit the loop if a collision is detected
    }


}
