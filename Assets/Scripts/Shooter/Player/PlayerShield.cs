using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    AudioManager audioManager;
    AudioSource source;

    private void Start()
    {
        audioManager = AudioManager.Instance;
        source = GetComponent<AudioSource>();
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("EnemyBullet")) {
            // player shield hit by enemy bullet
            other.gameObject.SetActive(false);
            audioManager.PlayShieldDeflect(source);
        }
    }
}
