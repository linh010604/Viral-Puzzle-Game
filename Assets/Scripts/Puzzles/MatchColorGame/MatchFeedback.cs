using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class MatchFeedback : MonoBehaviour
{
    public Color matchColor;
    public Color misMatchColor;

   // private Renderer renderer;
    private Image image;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    public void ChangeColorWithMatch(bool match)
    {
        if (match)
        {
            image.color = matchColor;
        }
        else
        {
            image.color = misMatchColor;
        }
    }
}
