using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroDialog : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Intro());
    }

    IEnumerator Intro()
    {

        yield return new WaitForSeconds(2);  // Wait for 2 seconds

        this.gameObject.SetActive(false); // Show the popup

    }
}
