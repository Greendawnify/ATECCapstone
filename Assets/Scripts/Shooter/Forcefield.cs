using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Forcefield : MonoBehaviour
{
    public string enemyType;                                // the pool tag type of enemy that the shield can protect

    AudioSource source;                                     // ref to the audio source
    List<Collider> colliders = new List<Collider>();        // all the colliders that are underneath the sheild and have the enemyType pool tag
    AudioManager audioManager;

    private void Start()
    {
        if(GetComponent<AudioSource>())
            source = GetComponent<AudioSource>();

        audioManager = AudioManager.Instance;
    }

    /// <summary>
    /// when the shiled is destroyed go through the colliders and turn them back on
    /// </summary>
    public void OnDestroy()
    {
        for (int i = 0; i < colliders.Count; i++) {
            colliders[i].enabled = true;
        }
    }

    public void TurnOff() {
        for (int i = 0; i < colliders.Count; i++)
        {
            colliders[i].enabled = true;
        }
        colliders.Clear();
    }

    public void OnTriggerEnter(Collider other)
    {
        // deactivate player bullets that hit the shield

        if (other.CompareTag("PlayerBullet"))
        {
            if (source) {
                audioManager.PlayShieldDeflect(source);
            }
            other.gameObject.SetActive(false);
        }

        if (other.CompareTag("Enemy"))
        {
            EnemyHealth health = other.GetComponent<EnemyHealth>();
            if (health.poolTag == enemyType)
            {
                // a turret has entered the shield
                colliders.Add(other);
                other.enabled = false;
            }
        }
    }


    /*
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("PlayerBullet")) {
            ContactPoint point = collision.GetContact(0);
            Vector3 newForward = Vector3.Reflect(collision.transform.forward, point.normal);

            Rigidbody rb = collision.collider.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.AddForce(newForward * push, ForceMode.VelocityChange);

           // AudioManager.Instance.PlayShieldDeflect(source);
        }
    }

    */
}
