using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour, IPooledObject
{
    public float speed = 5f;                    // the speed the bullet is fired at
    public float damage = 1f;                   // the damage the bullet does
    public AudioClip clip;                      // the sound the bullet makes
    public string poolTag;                      // the tag ref to the pool this object belongs to

    Rigidbody rb;                               // reference rigidbody of the bullet
    float usedSpeed;                            // value that is used in the code for calculations
    bool poolerIsReady = false;                 // toggled when OnObjectSpawn is called to let the script know if this instance was pooled


    void FixedUpdate() {
        // move the bullet
        if(speed > 0f)
        rb.MovePosition(transform.position + transform.forward *usedSpeed* Time.fixedDeltaTime);
    }

    public void OnObjectSpawn() {
        // replace start method
        usedSpeed = speed;
        if(rb== null)
            rb = GetComponent<Rigidbody>();
        poolerIsReady = true;
    }

    /// <summary>
    /// When turned off put it back in the pool
    /// </summary>
    void OnDisable() {
        if(poolerIsReady)
            ObjectPooler.Instance.EnqueObject(poolTag, gameObject);
    }
}
