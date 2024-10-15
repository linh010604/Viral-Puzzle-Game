using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class KeyPad : MonoBehaviour
{
    public PlayerController player;

    [Header("KeyPad Settings")]
    public string currentPassword = "123";
    public string input;
    public TMPro.TMP_Text displayText;
    public AudioSource audioData;
    public GameObject completeText;
    public bool complete = false;

    private bool keypadScreen;
    private float btnClicked = 0;
    private float numofGuesses;

    // Start is called before the first frame update
    void Start()
    {
        btnClicked = 0; // Number of times button was clicked
        numofGuesses = currentPassword.Length ; // Set the password length
    }

    public void ValueEntered(string value)
    {
        switch (value)
        {
            case "#":
                if (input == currentPassword)
                {
                    Debug.Log("Correct Password!");
                    input = "";
                    btnClicked = 0;
                    displayText.text = input.ToString();
                    StartCoroutine(ShowCompleteTextAndDisableSystem());
                }
                else
                {
                    input = "";
                    displayText.text = input.ToString();
                    if (audioData != null)
                    {
                        audioData.Play();
                    }
                    btnClicked = 0;
                }

                break;

            default:
                btnClicked++;
                input += value;
                complete = true;
                displayText.text = input.ToString() ;
                break;
        }
    }

    IEnumerator ShowCompleteTextAndDisableSystem()
    {
        completeText.gameObject.SetActive(true); // Show the popup

        yield return new WaitForSeconds(2);  // Wait for 2 seconds

        completeText.gameObject.SetActive(false); // Hide the popup

        // Optionally, disable the interaction with the match system
        DisableMatchSystem();
    }

    void DisableMatchSystem()
    {
        player.RecoverSpeed();
        this.gameObject.SetActive(false);  // Disable the match block so it doesn't pop up again
    }

    public void OnExitMatchSystem()
    {
        input = "";
        btnClicked = 0;
        displayText.text = input.ToString();
        player.RecoverSpeed();

    }
}
