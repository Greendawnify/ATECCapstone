using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float speed;
    public bool rotateX, rotateY, rotateZ;

    Vector3 rotationChange;

    void Update()
    {
        Quaternion lastRotation = transform.rotation;
        

        if (rotateX) {
            rotationChange += new Vector3(speed, 0f, 0f);
        }

        if (rotateY) {
            rotationChange += new Vector3(0f, speed, 0f);
        }

        if (rotateZ) {
            rotationChange += new Vector3(0f, 0f, speed);
        }

        transform.rotation = Quaternion.Euler(rotationChange);
    }
}
