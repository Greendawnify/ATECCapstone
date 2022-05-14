using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Ability { HEART, SHIELD, COUNT};


public class PlayerAbilities : MonoBehaviour
{
    public static PlayerAbilities Instance { get; private set; }

    public Ability ability = Ability.COUNT;                             // what ability the player has active
    public float playerShieldDuration;                                  // diration of shield ability

    [Header("For testing")]
    public bool useHealAbility;
    public bool useShieldAbility;

    Button abilityButton;                                               // the button that has to be pressed to call the ability
    GameObject image;                                                   // ability image indicator

    string methodName = "";                                             // name of the method of the ability
    bool hasUsedAbility = false;                                        // if the player has used the ability
    PlayerHealth healthScript;                                          // reference to the player health



    private void Awake()
    {
        // singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (useHealAbility) {
            HeartAbility();
            useHealAbility = false;
        }

        if (useShieldAbility) {
            ShieldAbility();
            useShieldAbility = false;
        }

    }

    // invoke the method asscoiated with the ability
    public void InvokeAbilityMethod() {
        if (!hasUsedAbility)
        {
            Invoke(methodName, 0f);
            hasUsedAbility = true;
        }
    }
    /// <summary>
    /// Heal ability
    /// </summary>
    void HeartAbility() {
        Debug.Log("Heart Ability");
        image.SetActive(false);
        healthScript = FindObjectOfType<PlayerHealth>();
        healthScript.RegainFullHealth();
    }

    /// <summary>
    /// Shield ability
    /// </summary>
    void ShieldAbility() {
        Debug.Log("Shield Ability");
        if(image)
            image.SetActive(false);
        healthScript = FindObjectOfType<PlayerHealth>();
        healthScript.TurnOnShield(playerShieldDuration);
    }

    // associates the method with the ability the player has selected
    public void SetButtonReferences(GameObject button, GameObject heart, GameObject shield) {
        abilityButton = button.GetComponent<Button>();

        switch (ability) {
            case Ability.HEART:
                image = heart;
                image.SetActive(true);
                methodName = "HeartAbility";
                break;
            case Ability.SHIELD:
                image = shield;
                image.SetActive(true);
                methodName = "ShieldAbility";
                break;
            case Ability.COUNT:
                button.SetActive(false);
                abilityButton = null;
                break;
        }
    }

    public void SetAbility(int ab) {
        if (ab == 1)
            ability = Ability.HEART;
        else if (ab == 0)
            ability = Ability.SHIELD;
    }


    public void SelectingAbility(GameObject button) {
        StartCoroutine(WaitFor(button.GetComponent<AbilityButton>()));
    }

    public bool GetHasUsedAbility() {
        return hasUsedAbility;
    }

    public void SetHasUsedAbility(bool use) {
        hasUsedAbility = use;
    }

    IEnumerator WaitFor(AbilityButton butt) {
        yield return null;
        if (butt.GetIsSelected())
        {
            ability = butt.ability;
        }
        else {
            ability = Ability.COUNT;
        }
    }
}
