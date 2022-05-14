using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectToEnemyHealth : MonoBehaviour
{
    public EnemyHealth healthScript;                    // the enemy heatl script in the parent

    Vector3 offset;                                     // a reference to a value in the enemy health
    // Start is called before the first frame update
    void Start()
    {
        offset = healthScript.particleCreationOffset;
    }

    /// <summary>
    /// Called when the hover enemy dies to offcially kill the enemy
    /// </summary>
    public void CallCreateDeathParticle() {
        healthScript.CreateDeathParticle(offset);
    }
}
