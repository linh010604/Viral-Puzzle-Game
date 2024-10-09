using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.InputSystem.XR;

public class MatchEntity : MonoBehaviour
{

    public MatchFeedback feedBack;
    public MovablePair movablePair;
    public Image fixedPairRender;
    public MatchSystemManager matchSystemManager;

    private bool matched;

    public Vector3 GetMovablePairPosition()
    {
        return movablePair.GetPosition();
    }

    public void SetMovablePairPosition(Vector3 newPosition)
    {
        movablePair.SetInitialPosition(newPosition);
    }


    public void Reset()
    {
        movablePair.ResetPosition();
        feedBack.ChangeColorWithMatch(false);
    }

    public void SetColorToPairs(Color pairColor)
    {
        movablePair.GetComponent<Image>().color = pairColor;
        fixedPairRender.GetComponent<Image>().color = pairColor;
    }

    public void PairObjectInteraction( bool IsEnter, MovablePair movable, Port port)
    {
        if (IsEnter && !matched)
        {
            matched = (movable == movablePair);
            if (matched)
            {
                movable.Collide(port);
                matchSystemManager.NewMatchRecord(matched);
                feedBack.ChangeColorWithMatch(matched);
            }
        }
        else if (!IsEnter && matched)
        {
            matched = !(movable == movablePair);
            if (!matched)
            {
                //movable.;
                matchSystemManager.NewMatchRecord(false);
                feedBack.ChangeColorWithMatch(false);
            }
            
        }

    }
}
