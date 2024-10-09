using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovableUIImage : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Vector3 initialPosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialPosition = rectTransform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Optionally do something when drag starts
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move the UI element according to the mouse position
        rectTransform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Optionally, reset position or handle logic after drag ends
        rectTransform.position = initialPosition; // Reset to initial position
    }
}

