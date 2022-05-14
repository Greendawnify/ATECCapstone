using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dampen : MonoBehaviour
{
    public float smoothTime = 0.3f;

    Vector3 velocity = Vector3.zero;
    Vector3 pos;

    void Start()
    {
        pos = transform.position;
    }

    void Update()
    {
        CallDampen();
    }

    void CallDampen()
    {
        transform.position = Vector3.SmoothDamp(transform.position, pos, ref velocity, smoothTime);
    }
}
