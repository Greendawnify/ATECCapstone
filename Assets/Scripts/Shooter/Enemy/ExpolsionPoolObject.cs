using UnityEngine;

public class ExpolsionPoolObject : MonoBehaviour, IPooledObject
{
    public string poolTag;                          // the tag ref to the pool this object is apart of
    public ParticleSystem particle;                 // if the explossion if a particle this is its reference

    bool poolerIsReady = false;                     // toggles if this object has been spawned by the ObjectPooler

    public void OnObjectSpawn() {
        if(particle)
            particle.Play();

        poolerIsReady = true;
    }

    /// <summary>
    /// When turned off add the explosion back into the pool
    /// </summary>
    void OnDisable() {
        if (poolerIsReady)
        {
            if(particle)
                particle.Stop();

            ObjectPooler.Instance.EnqueObject(poolTag, gameObject);
        }
    }
}
