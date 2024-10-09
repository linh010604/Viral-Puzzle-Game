using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class MatchSystemManager : MonoBehaviour
{
    public List<Color> colorMaterials;
    public PlayerController player;
    public GameObject completeText;

    private List<MatchEntity> matchEntities;
    private int targetMatchCount;
    private int currentMatchCount = 0;
    

    // Start is called before the first frame update
    public void Start()
    {
        
        matchEntities = transform.GetComponentsInChildren<MatchEntity>().ToList();
        targetMatchCount = matchEntities.Count;
        Debug.Log("target match count = " + targetMatchCount);
        SetEntityColors();
        RandomizeMovablePairPlacement();
    }

    public bool AllPaired()
    {
        if (currentMatchCount == targetMatchCount)
        {
            return true;
        }
        return false;
    }

    void SetEntityColors()
    {
        Shuffle(colorMaterials);

        for(int i = 1; i < matchEntities.Count; i++)
        {
            matchEntities[i].SetColorToPairs(colorMaterials[i]);
        }
    }

    void RandomizeMovablePairPlacement()
    {
        List<Vector3> movablePairPositions = new List<Vector3>();

        for (int i = 0; i < matchEntities.Count; i++)
        {
            movablePairPositions.Add(matchEntities[i].GetMovablePairPosition());
        }

        Shuffle(movablePairPositions);

        for (int i = 0; i < matchEntities.Count; i++)
        {
            matchEntities[i].SetMovablePairPosition(movablePairPositions[i]);
        }
    }

    public void NewMatchRecord(bool MatchConnected)
    {
        if (MatchConnected)
        {
            currentMatchCount++;
        }
        else
        {
            currentMatchCount--;
        }

        Debug.Log("Currently, there are " + currentMatchCount + " matches");

        if (currentMatchCount == targetMatchCount)
        {
            Debug.Log("All Paired!");

            StartCoroutine(ShowCompleteTextAndDisableSystem());

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
    }

    void ResetAllMovablePairs()
    {
        for (int i = 0; i < matchEntities.Count; i++)
        {
            matchEntities[i].Reset();
        }
    }

    public void OnExitMatchSystem()
    {
        player.RecoverSpeed();
        ResetAllMovablePairs(); // Call this when the match system UI is closed
        currentMatchCount = 0;
        matchEntities = null;

    }

    #region StaticMatchCounter
    #endregion

    public static void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while(n > 1)
        {
            n--;
            int k = Random.Range(0, n);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    } 
}
