using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInteraction : MonoBehaviour
{
    public string tag;                                  // the id for each trigger that has this script
    public Animator menuBoardAnim;                      // the main menu baord that goes away when triggered
    public Animator otherBoardAnimator;                 // the other board that comes in when this is triggered

    LevelSelectMenuInteraction menu;
    ScoreScreenMenu scoreMenu;


    private void Start()
    {
        menu = FindObjectOfType<LevelSelectMenuInteraction>();
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet")) {
            menuBoardAnim.SetTrigger("exit");
            MenuAction();
            
        }
    }

    /// <summary>
    /// the action for when the trigger is hit by a bullet
    /// </summary>
    void MenuAction() {
        switch (tag) {
            case "level":
                Debug.Log("level selct trigger");

                if(menu==null)
                    menu = FindObjectOfType<LevelSelectMenuInteraction>();
                menu.UpdateLevelPersistentData();
                otherBoardAnimator.SetTrigger("enter");
                break;
            case "setting":
                Debug.Log("settings trigger");
                otherBoardAnimator.SetTrigger("enter");
                break;
            case "score":
                Debug.Log("score trigger");
                if (scoreMenu == null)
                    scoreMenu = FindObjectOfType<ScoreScreenMenu>();

                scoreMenu.UpdateDeathScorePersistenData();
                scoreMenu.UpdateScorePersistentData();
                otherBoardAnimator.SetTrigger("enter");

                break;
            case "exit":
                Debug.Log("exit trigger");
                Application.Quit();
                break;
        }
    }
}
