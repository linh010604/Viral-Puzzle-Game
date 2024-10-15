using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintText : MonoBehaviour
{
    public TMPro.TMP_Text dialogText; // Reference to hint dialog text
    public string enterText; // Reference to hint dialog text
    public string exitText; // Reference to hint dialog text

    // Start is called before the first frame update
    void Start()
    {
        dialogText.text = exitText;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
          
            dialogText.text = enterText;
            //Debug.Log("Player near button. Press 'E' to interact.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            dialogText.text = exitText;
            //Debug.Log("Player near button. Press 'E' to interact.");
        }
    }
}
