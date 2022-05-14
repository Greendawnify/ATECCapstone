using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleActiveTimer : MonoBehaviour
{
    public float time;

    private void OnEnable()
    {
        StartCoroutine(TurnOff());


    }

    private void OnDisable()
    {
        
    }

    IEnumerator TurnOff() {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
}
