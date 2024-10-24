using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFloorColor : MonoBehaviour
{
    private Material floorMaterial;

    public Color triggeredColor = Color.red;

    private Color originalColor;

    public float colorChangeDuration = 0.5f;

    private bool playerOnFloor = false;

    void Start()
    {
        floorMaterial = GetComponent<Renderer>().material;
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (!playerOnFloor)
            {
                playerOnFloor = true;
                StopAllCoroutines(); 
                StartCoroutine(ChangeColor(floorMaterial.GetColor("_Color"), triggeredColor, colorChangeDuration));
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerOnFloor = false;
            StopAllCoroutines();
        }
    }

    IEnumerator ChangeColor(Color startColor, Color endColor, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            floorMaterial.color = Color.Lerp(startColor, endColor, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        floorMaterial.color = endColor;
    }
}

