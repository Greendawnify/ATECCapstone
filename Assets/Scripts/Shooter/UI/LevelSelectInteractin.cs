using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectInteractin : MonoBehaviour
{
    public string tag;                                  // the id for each trigger that has this script
    public Animator menuBoardAnim;                      // the main menu baord that goes away when triggered
    public Animator otherBoardAnimator;                 // the other board that comes in when this is triggered
    public GameObject downArrow, otherDownArrow;

    PlayerAbilities ability;
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            MenuAction();
        }
    }

    void MenuAction()
    {
        switch (tag)
        {
            case "left":
                Debug.Log("settings trigger");
                break;
            case "right":
                Debug.Log("score trigger");
                break;
            case "exit":
                Debug.Log("exit trigger");
                menuBoardAnim.SetTrigger("exit");
                StartCoroutine(wait());
                break;
            case "heart":
                downArrow.SetActive(true);
                otherDownArrow.SetActive(false);
                ability = PlayerAbilities.Instance;
                ability.SetAbility(1);
                break;
            case "shield":
                downArrow.SetActive(true);
                otherDownArrow.SetActive(false);
                ability = PlayerAbilities.Instance;
                ability.SetAbility(0);
                break;
        }
    }

    IEnumerator wait() {
        yield return new WaitForSeconds(1f);
        otherBoardAnimator.SetTrigger("enter");
    }
}
