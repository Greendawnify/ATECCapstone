using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityButton : MonoBehaviour
{
    public Ability ability;                         // enum for which ability this button is for (heal,sheild, slow)
    public GameObject highlight;                    // object that highlights the buttong

    public AbilityButton[] otherButtons;            // the other ability buttons

    bool isSelected = false;                        // if this button has been selected


    /// <summary>
    /// when this button is clicked
    /// </summary>
    public void Click() {
        if (!isSelected)
        {
            isSelected = true;
            highlight.SetActive(true);

            // deactivate this button
            for (int i = 0; i < otherButtons.Length; i++) {
                otherButtons[i].Deactivate();
            }
        }
        else {
            isSelected = false;
            highlight.SetActive(false);
        }
    }

    // we deactivated
    public void Deactivate() {
        isSelected = false;
        highlight.SetActive(false);
    }

    

    public bool GetIsSelected() {
        return isSelected;
    }

}
