using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertMeter : MonoBehaviour
{
    public float maxSensitive = 100.0f; // Max sensitivity value
    public float sensitive = 0.0f; // Current sensitivity
    public float minNeedlePointerAngle = 12.64f; // Min angle of the needle
    public float maxNeedlePointerAngle = -153.3f;  // Max angle of the needle
    public RectTransform pointerHolder; 
    public TMPro.TMP_Text speedLabel;
    public PlayerController playerController; // Reference to the PlayerController

    public float increaseRate = 10.0f;
    public float decreaseRate = 25.0f; 

    void Update()
    {
        if (playerController.isMovingForward)
        {
            sensitive = Mathf.Clamp(sensitive + increaseRate * Time.deltaTime, 0, maxSensitive);
        }
        else
        {
            sensitive = Mathf.Clamp(sensitive - decreaseRate * Time.deltaTime, 0, maxSensitive);
        }

        pointerHolder.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(minNeedlePointerAngle, maxNeedlePointerAngle, sensitive / maxSensitive));

        UpdateAlertStage();
    }

    void UpdateAlertStage()
    {
        // Update the speed label to cooling down
        if (!playerController.isMovingForward && sensitive != 0)
        {
            speedLabel.text = "Cooling";
            speedLabel.alignment = TMPro.TextAlignmentOptions.Center;
            speedLabel.color = Color.white;
            return;
        }
        if (sensitive < maxSensitive * 0.30f)
        {
            speedLabel.text = "Normal";
            speedLabel.alignment = TMPro.TextAlignmentOptions.Center;
            speedLabel.color = Color.green;
        }
        else if (sensitive < maxSensitive * 0.55f)
        {
            speedLabel.text = "Hmmm";
            speedLabel.alignment = TMPro.TextAlignmentOptions.Center;
            speedLabel.color = Color.yellow;
        }
        else if (sensitive < maxSensitive * 0.90f)
        {
            speedLabel.text = "Relax";
            speedLabel.alignment = TMPro.TextAlignmentOptions.Center;
            speedLabel.color = new Color(1.0f, 0.5f, 0.0f);
        }
        else
        {
            speedLabel.text = "RUN !!";
            speedLabel.alignment = TMPro.TextAlignmentOptions.Center;
            speedLabel.color = Color.red;
        }
    }
}
