using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityHolder : MonoBehaviour
{
    public GameObject sheild, heart;                // a sheild, heart, and freeze image to indicate which is in affect
    public Image background;                                // the background image that fills when being selected

    PlayerAbilities playerAbilities;                        // reference to player abilities to call specific functions
    void Start()
    {
        playerAbilities = PlayerAbilities.Instance;

        if (playerAbilities != null) {
            playerAbilities.SetButtonReferences(gameObject, heart, sheild);
        }
    }

    void Update()
    {
        // check for the touch in the right area of the screen
        // fill up the bar
        // when it is all the way full call a function in PlayerAbilites to Invoke the right funtion
        if (!playerAbilities.GetHasUsedAbility())
        {
            if (Input.touchCount > 0)
            {
                CheckPositionOfTouch(Input.GetTouch(0).position);
            }

            if (Input.GetMouseButton(0))
            {
                CheckPositionOfTouch(Input.mousePosition);
            }
        }
        else {
            sheild.SetActive(false);
            heart.SetActive(false);
            background.gameObject.SetActive(false);
            this.enabled = false;
        }

        
    }
    /// <summary>
    /// Check that the touch position is in the right spot and invokes the ability
    /// </summary>
    /// <param name="pos"></param>
    void CheckPositionOfTouch(Vector2 pos)
    {
        if ((pos.x > Screen.width * 0.85f)  &&
            (pos.y < Screen.height * 0.15f) )
        {
            background.fillAmount += 0.65f* Time.deltaTime;
            // once the background image fills up it calls a ability function
            if (background.fillAmount >= 1f) {
                playerAbilities.InvokeAbilityMethod();
                background.fillAmount = 0f;
            }
        }
        else {
            background.fillAmount = 0f;
        }
    }
}
