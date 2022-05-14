using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugHealth : EnemyHealth
{
    public int swarmPosition;                                   // ref position in the swarm
    public SwarmMovement swarm;                                 // ref to the swarm movement script
    public Collider bugCollider;

    /// <summary>
    /// Called when hit by a player bullet
    /// </summary>
    /// <param name="amount"></param> the amount of damage
    public override void LoseHealth(float amount) {
        Debug.Log("A bug is loosing hwalth");
        bugCollider.enabled = false;

        swarm.SetIsBugActive(swarmPosition, false);

        if (anim) {
            anim.SetTrigger("Death");
            StartCoroutine(BugDeath(waitTimeToDie));
        }

        if (source) {
            AudioManager.Instance.PlayEnemyDeath(source);
        }
    }

    IEnumerator BugDeath(float wait) {
        yield return new WaitForSeconds(wait);
        CreateDeathParticle(particleCreationOffset);
        gameObject.SetActive(false);
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }
}
