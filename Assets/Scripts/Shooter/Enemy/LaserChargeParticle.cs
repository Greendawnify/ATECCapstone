using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserChargeParticle : MonoBehaviour
{
    public float lowerScaleVale = 0.075f;           // the values that the obj shrinks by over time

    public void Start()
    {
        gameObject.SetActive(false);
    }


    public void OnEnable()
    {
        // reset the scale
        transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
    }

    private void Update()
    {
        // shrink the scale over time
        transform.localScale -= new Vector3(lowerScaleVale, lowerScaleVale, lowerScaleVale) * Time.deltaTime;

        // rotate the object over time
        Quaternion lastRotation = transform.rotation;
        Vector3 rotationChange = lastRotation.eulerAngles + new Vector3(0f, 0f, 5f);
        transform.rotation = Quaternion.Euler(rotationChange);
    }
}
