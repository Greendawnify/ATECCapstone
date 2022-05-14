using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreScreenInteraction : MonoBehaviour
{

    public string tag;                                  // the id for each trigger that has this script
    public Animator menuBoardAnim;                      // the main menu baord that goes away when triggered
    public Animator otherBoardAnimator;                 // the other board that comes in when this is triggered
                                                        // Start is called before the first frame update
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
            case "exit":
                Debug.Log("exit trigger");
                menuBoardAnim.SetTrigger("exit");
                otherBoardAnimator.SetTrigger("enter");
                StartCoroutine(wait());
                break;
        }
    }

    IEnumerator wait() {
        Debug.Log("wait called");
        yield return new WaitForSeconds(1f);
        otherBoardAnimator.SetTrigger("enter");
    }
}
