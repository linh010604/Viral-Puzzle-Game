﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class will make the player a child of whatever it is attached to
/// It is meant to be used with the moving platform to keep the player moving along with the platform
/// </summary>
public class PlayerChilder : MonoBehaviour
{
    /// <summary>
    /// Description:
    /// Built-in Unity function that is called whenever a trigger collider is entered by another collider
    /// Input:
    /// Collider2D collision
    /// return:
    /// void (no return)
    /// </summary>
    /// <param name="collision">The collider that entered the trigger</param>
    private void OnTriggerEnter(Collider collision)
    {
        MakeAChildOfAttachedTransform(collision);
    }

    /// <summary>
    /// Description:
    /// Built-in Unity function that is called whenever a trigger collider is entered by another collider
    /// Input:
    /// Collider2D collision
    /// return:
    /// void (no return)
    /// </summary>
    /// <param name="collision">The collider that exited the trigger</param>
    private void OnTriggerExit(Collider collision)
    {
        DeChild(collision);
    }

    /// <summary>
    /// Description:
    /// Makes the parent of the passed collider a child of the transform that this script is attached to
    /// Works only for the player character but could be expanded to work for non-player characters
    /// Input:
    /// Collider2D collision
    /// Return:
    /// void (no return)
    /// </summary>
    /// <param name="collision">The collision to make no longer a child</param>
    private void DeChild(Collider collision)
    {
        if (collision.tag == "Player")
        {
            collision.gameObject.transform.parent.transform.SetParent(null);
        }
    }


    /// <summary>
    /// Description:
    /// Makes the parent of the passed collider a child of the transform that this script is attached to
    /// Input:
    /// Collider2D collision
    /// Return:
    /// void (no return)
    /// </summary>
    /// <param name="collision">The collision whos transform will be childed</param>
    private void MakeAChildOfAttachedTransform(Collider collision)
    {
        if (collision.tag == "Player")
        {
            collision.gameObject.transform.parent.transform.SetParent(transform);
        }
    }
}
