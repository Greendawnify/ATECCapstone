using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Interface for all the enemy shooting scripts except for the Swarm Enemy
public interface IEnemyShooting
{
    void StartShootingProcess();        // starts the shooting process
    void EndShootingProcess();          // ends the shooting process. Does not have any implemntation
}
