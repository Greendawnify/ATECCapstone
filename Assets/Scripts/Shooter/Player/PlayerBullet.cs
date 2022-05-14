using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerBullet : MonoBehaviour, IPooledObject
{
    public float speed;                                         // the speed of the bullet
    public float damage;                                        // damage of the bullet
    public GameObject particle;                                 // particle of the bullet
    public string poolTag;                                      // tag  of pool this bullet is associated with

    Rigidbody rb;                                               // rigidbody of the player bullet
    bool poolerIsReady = false;

    public void OnObjectSpawn() {
        rb = GetComponent<Rigidbody>();
    }

    // gets the direection and then sets the velocity of the object
    public void SetBulletShooting(Vector3 dir) {
        rb.velocity = dir * speed;
        poolerIsReady = true;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Building")) {
            // if hitting a building turn off bullet
            if (particle != null)
            {
                Instantiate(particle, transform.position, transform.rotation);
            }

            gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
    }

    /// <summary>
    /// When disabled put back into the pool
    /// </summary>
    void OnDisable() {
        if(poolerIsReady)
            ObjectPooler.Instance.EnqueObject(poolTag, gameObject);
    }
}
