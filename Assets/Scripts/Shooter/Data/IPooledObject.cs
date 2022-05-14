using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface that I can find on all pooled objects and call OnObjectSpawn()
/// </summary>
public interface IPooledObject 
{

    void OnObjectSpawn();

    //void OnObjectDisable();
}
