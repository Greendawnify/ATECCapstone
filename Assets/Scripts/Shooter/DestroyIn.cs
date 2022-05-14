using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyIn : MonoBehaviour
{
    public float time;                      // when the object gets deactivated
    public bool actualyDestroy = false;     // whether when time runs out to actually destroy the object or just deactivate it

    void OnEnable()
    {
        if (actualyDestroy)
        {
            Destroy(gameObject, time);
        }
        else
        {
            Invoke("Disable", time);
        }
    }

    void Disable() {
        gameObject.SetActive(false);
    }

}
