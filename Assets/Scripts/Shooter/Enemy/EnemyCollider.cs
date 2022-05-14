using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollider : MonoBehaviour
{
    public EnemyHealth[] health;                              // reference to the enemy health script


    public void OnCollisionEnter(Collision collision)
    {
        // when hit by the player bullet
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            // get the damage value
            float dam = collision.collider.GetComponent<PlayerBullet>().damage;

            for (int i = 0; i < health.Length; i++) {
                // apply the damage value
                health[i].LoseHealth(dam);
            }


            // destroy the player bullet
            //Destroy(collision.gameObject);
            collision.gameObject.SetActive(false);
        }
    }
}
