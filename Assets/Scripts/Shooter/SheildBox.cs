using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheildBox : MonoBehaviour
{
    public GameObject[] shield;             // sheilds the shield box "powers"

    EnemyHealth health;

    private void Start()
    {
        health = GetComponent<EnemyHealth>();
    }


    /// <summary>
    /// When this sheild box is destroyed it will Destroy the shields it was "powering"
    /// </summary>
    public void OnDisable()
    {
        for (int i = 0; i < shield.Length; i++) {
            if (shield[i] != null)
            {
                shield[i].GetComponent<Forcefield>().TurnOff();
                shield[i].SetActive(false);
            }
        }

        
    }
}
