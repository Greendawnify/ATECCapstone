using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    /// <summary>
    /// A class the defines the name, and prefab to be used by a poolof objects that I will spawn at the beginning of the game
    /// </summary>
    [System.Serializable]
    public class Pool {
        public string tag;              // name associated with the objs in this pool
        public GameObject prefab;       // the prefab that the pool spawns
        public int initialSize;         // This pool will be filled by this many objs at the very beginning
    }

    public static ObjectPooler Instance { get; private set; }

    public List<Pool> pools;                                            // all the pools I am using in the game
    public Dictionary<string, Queue<GameObject>> poolDictionary;        // the dictionary that references tags of pools with queues of gameObjects

    private void Awake()
    {
        // making it a singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // initial creation
        for (int i = 0; i < pools.Count; i++) {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            if (pools[i].initialSize > 0)
            {
                for (int j = 0; j < pools[i].initialSize; j++)
                {
                    GameObject obj = Instantiate(pools[i].prefab);
                    obj.SetActive(false);
                    obj.transform.SetParent(transform);
                    objectPool.Enqueue(obj);
                }
            }

            poolDictionary.Add(pools[i].tag, objectPool);
        }

        // then create the single level objects
    }

    /// <summary>
    /// Called when I need an object from a specific pool
    /// </summary>
    /// <param name="tag"></param> the specific pool
    /// <param name="pos"></param> where I will spawn the obj
    /// <param name="rot"></param> How the rotation of the spawned will be set
    /// <returns></returns>
    public GameObject SpawnFromPool(string tag, Vector3 pos, Quaternion rot) {
        if (poolDictionary[tag].Count == 0) {
            AddObjects(tag, 1);
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        
        objectToSpawn.transform.position = pos;
        objectToSpawn.transform.rotation = rot;
        
        // finds the interface on all the pooled objects
        objectToSpawn.GetComponent<IPooledObject>().OnObjectSpawn();

        return objectToSpawn;
    }

    /// <summary>
    /// If I am out of a specific obj then I will Instnatiate new objs and add them to the pool
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="count"></param>
    public void AddObjects(string tag, int count) {
        for (int i = 0; i < pools.Count; i++) {
            if (tag == pools[i].tag) {
                // this is the right pool of objects
                GameObject obj = Instantiate(pools[i].prefab);
                obj.SetActive(false);
                obj.transform.SetParent(transform);

                poolDictionary[tag].Enqueue(obj);
                break;
            }
        }
    }

    /// <summary>
    /// Called when a pooled object is deactivated. Add it back to the pool
    /// </summary>
    /// <param name="tag"></param> tag ref to a pool
    /// <param name="obj"></param> the object added
    public void EnqueObject(string tag, GameObject obj) {
        obj.SetActive(false);
        for (int i = 0; i < pools.Count; i++) {
            if (tag == pools[i].tag) {
                // the correct pool
                poolDictionary[tag].Enqueue(obj);
            }
        }
    }
}
