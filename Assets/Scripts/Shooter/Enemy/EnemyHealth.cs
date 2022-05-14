using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour, IPooledObject
{

    public float startHealth;                           // the starting health of the enemy
    public Image healthbar;                             // reference to the health bar image 
    public GameObject healthCanvas;                     // reference to the canvas for the enemy
    public AudioSource source;                          // ref to the audiosource that will produce the death sounds
    public Animator anim;                               // the animator for the enemy
    public Vector3 particleCreationOffset;              // the offset for the creatingof the explosion particles when the enemy dies
    public float waitTimeToDie = 0.5f;                  // how long to wait till the enemy is turned off
    public string explosionPoolTag;                     // tag of the explosion that the enemy spawns from a pool
    public string poolTag;                              // the tag of the pool that this enemy is from

    float health;                                       // the health value that decreases when the enemy is damaged
    bool firstHit = false;                              // check if the enemy has been hit yet
    int damageAnimValue = 0;                            // some enmies have "hurt" animations and this keeps track of that
    ObjectPooler pooler;                                // ref to ObjectPooler that has all the pools

    void Start()
    {
        health = startHealth;
        pooler = ObjectPooler.Instance;

    }

    void OnEnable() {
        health = startHealth;
        try
        {
            healthbar.fillAmount = health / startHealth;
        }
        catch { }
    }

    public void OnObjectSpawn() {
        pooler = ObjectPooler.Instance;

        // reset the health and start the shooting process for all the enemies
        health = startHealth;
        healthbar.fillAmount = health / startHealth;
        // get access to the IEnenmyShooting type script and turn them on
        gameObject.GetComponentInChildren<IEnemyShooting>().StartShootingProcess();

        if (anim)
        {
            anim.SetInteger("Hurt", 0);
            anim.SetTrigger("Revive");
        }
        healthCanvas.SetActive(false);
        firstHit = false;
        damageAnimValue = 0;
    }

    /// <summary>
    /// called when hit by a player bullet
    /// </summary>
    /// <param name="amount"></param> the amount of damage
    public virtual void LoseHealth(float amount) {

        if (healthCanvas)
        {
            // if this is the first time the enemy has been hit turn on the enmies canvas
            if (!firstHit)
            {
                healthCanvas.SetActive(true);
                firstHit = true;
                
            }
        }

        // lose health
        health -= amount;

        if (anim != null)
        {
            // apply hurt animations if they are applicable
            damageAnimValue++;
            anim.SetInteger("Hurt", damageAnimValue);
            anim.SetTrigger("Hit");
        }  

        if (healthbar != null)
        {
            // apply the lose health to the healthbar
            healthbar.fillAmount = health / startHealth;
        }

        // if the health is below zero destroy the object
        if (health <= 0) {
            health = 0;
            // die
            if (source)
            {
                AudioManager.Instance.PlayEnemyDeath(source);
            }
            // death animation
            if (anim != null)
            {
                anim.SetTrigger("Death");
                StartCoroutine(Die(waitTimeToDie));
            }
            else {
                if (explosionPoolTag != "") // should only apply to the swarm enemy
                {
                    CreateDeathParticle(particleCreationOffset);
                }

                // if there is a pool tag add obj back into that pool
                if (poolTag != "" || poolTag != null)
                {
                    pooler.EnqueObject(poolTag, gameObject);
                }
                else
                {
                    //Destroy(gameObject);
                    //gameObject.SetActive(false);
                    StartCoroutine(DiableSwarm());
                }
            }

        }
    }
    /// <summary>
    ///  called when health if added back to the enemy. Only applies to the swarm enemy
    /// </summary>
    /// <param name="hp"></param> amount of health regained
    public void AddHealth(float hp) {
        health += hp;

        if (health > startHealth) {
            health = startHealth;
        }

        // reset the hurt animation index
        damageAnimValue--;
        if (damageAnimValue < 0) {
            damageAnimValue = 0;
        }

        if (healthbar != null)
        {
            // apply the lose health to the healthbar
            healthbar.fillAmount = health / startHealth;
        }


    }

    public void CreateDeathParticle(Vector3 offset) {
        GameObject newObj = pooler.SpawnFromPool(explosionPoolTag, transform.position, transform.rotation);
        newObj.SetActive(true);
    }

    public void SetHealth(float amount) {
        health = amount;
        healthbar.fillAmount = health / startHealth;
    }

    public IEnumerator Die(float wait) {
        yield return new WaitForSeconds(wait);
        CreateDeathParticle(particleCreationOffset);
        // if obj is apart of a pool. Put it back in the pool 
        if (poolTag != "")
        {
            pooler.EnqueObject(poolTag, gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    IEnumerator DiableSwarm() {
        yield return new WaitForSeconds(.75f);
        gameObject.SetActive(false);
    }

    public float GetHP() {
        return health;
    }
}
