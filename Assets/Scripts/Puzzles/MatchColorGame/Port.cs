using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Port : MonoBehaviour, IDropHandler, IPointerExitHandler
{
    public MatchEntity ownerMatchEntity;

    // This method is called when an item is dropped on the UI element
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            MovablePair collidedMoveable = eventData.pointerDrag.GetComponent<MovablePair>();
            if (collidedMoveable != null)
            {
                ownerMatchEntity.PairObjectInteraction(true, collidedMoveable,this);
                Debug.Log("Ok");
            }
        }
    }

    // You can use this method to handle when an item is dragged out of the port
    public void OnPointerExit(PointerEventData eventData)
    {
        /*if (eventData.pointerDrag != null)
        {
            MovablePair collidedMoveable = eventData.pointerDrag.GetComponent<MovablePair>();
            ownerMatchEntity.PairObjectInteraction(false, collidedMoveable,this);
        }
        Debug.Log("Hate");*/
    }

    public void Reset(MovablePair movablePair)
    {
        ownerMatchEntity.PairObjectInteraction(false, movablePair, this);
    }
}
