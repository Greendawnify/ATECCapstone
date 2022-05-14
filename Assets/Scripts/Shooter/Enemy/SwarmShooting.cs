using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;

public class SwarmShooting : MonoBehaviour
{
    public GameObject bullet;                                           // the prefab bullet the swarm shoots
    public List<Transform> firePositions = new List<Transform>();       // all the fire positions that the bullet can shoot from
    public Transform[] bugs;                                            // refs to all the positions of where the bugs in the swarm are
    public Transform sightCheckPos;                                     // where the sight raycast is shot from
    public float timeTillShoot = 2f;                                    // the time between each shot
    public string spawnFromPoolTag;                                     // the pool tag of the bullets that will be spawned

    ARSessionOrigin arOrigin;                                           // ar session origin ref
    GameObject playerCam;                                               // ref to the player cam
    ObjectPooler pooler;                                                // ref to ObjectPooler
    PlayerData playerData;

    Animator anim;                                                      // animator for moving the swarm
    float savedTimeValue;                                               // saved value of the shoot time. when time is slowed

    [SerializeField] bool canShoot = false;                             // if the swarm can shoot
    bool isReviving = false;                                            // if the swarm is reviving
    float timer;                                                        // used to count down the time till the swarm shoots again

    void Start()
    {
        playerData = PlayerData.Instance;
        arOrigin = playerData.arOrigin;
        playerCam = playerData.playerCam;
        pooler = ObjectPooler.Instance;

        anim = GetComponent<Animator>();
        
    }

    private void OnEnable()
    {
        timer = timeTillShoot;
        savedTimeValue = timeTillShoot;
    }

    void Update()
    {

        // if not reviving have all the bugs look at the player cam
        if (!isReviving) {
            for (int i = 0; i < bugs.Length; i++) {
                bugs[i].LookAt(playerCam.transform);
            }
        
        }

        // can shoot countdown till shooting
        if (canShoot) {
            timer -= Time.deltaTime;
            if (timer <= 0f) {
                SwarmShoot();
                timer = timeTillShoot;
            }
        }
    }

    /// <summary>
    /// fires 3 shots from specific bugs aslongas they are active
    /// </summary>
    public void SwarmShoot() {

        int firstShot = Random.Range(0, firePositions.Count);
        int secondShot = Random.Range(0, firePositions.Count);
        int thirdShot = Random.Range(0, firePositions.Count);

        if (firstShot == secondShot) firstShot = -1;
        if (firstShot == thirdShot) firstShot = -1;
        if (secondShot == thirdShot) secondShot = -1;
        if (firstShot == thirdShot && firstShot == secondShot) {
            firstShot = -1;
            secondShot = -1;
        }

        if (SightCheck())
        {
            if (firstShot != -1 && firePositions[firstShot].gameObject.activeInHierarchy) Attack(firstShot);
            if (secondShot != -1 && firePositions[secondShot].gameObject.activeInHierarchy) Attack(secondShot);
            if (thirdShot != -1 && firePositions[thirdShot].gameObject.activeInHierarchy) Attack(thirdShot);
        }
    }

    void Attack(int pos)
    {
        //GameObject newObj = Instantiate(bullet, firePositions[pos].position, firePositions[pos].rotation);
        GameObject newObj = pooler.SpawnFromPool(spawnFromPoolTag, firePositions[pos].position, firePositions[pos].rotation);
        newObj.transform.LookAt(playerCam.transform);
        newObj.SetActive(true);
        

        AudioSource source = bugs[pos].GetComponentInChildren<AudioSource>();
        if (source)
        {
            AudioManager.Instance.PlayEnemyShoot(source);
        }
    }

    bool SightCheck() {

        Vector3 direction = playerCam.transform.position - sightCheckPos.position;
        Ray ray = new Ray(sightCheckPos.position, direction);
        //int mask = (1 << 8);
        int mask = 1; // need to figure out how to make sure it will shoot through other enemies but not buildings
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 2f);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask, QueryTriggerInteraction.Collide))
        {
            Debug.Log(hit.collider.name);
            if (hit.collider.CompareTag("MainCamera"))
            {
                // can see the player
                return true;
            }
        }

        return false;
    }

    public void SetCanShoot(bool can) {
        canShoot = can;
    }

    public void SetIsReviving(bool toggle) {
        isReviving = toggle;
    }

    public void SetFirePositionIndex(int index, Transform pos) {
        firePositions[index] = pos;
    }

    /// <summary>
    /// If true then add a new bug back into the fire position list. If false remove them from the list
    /// </summary>
    /// <param name="adding"></param> to add or remove
    /// <param name="index"></param> where to remove the bug
    /// <param name="pos"></param> the ref to the location to add a fire position
    public void SetFirePositionIndex(bool adding, int index, Transform pos) {
        if (adding)
        {
            firePositions.Add(pos);

        }
        else {
            firePositions.RemoveAt(index);
        }
    }

    private void OnDisable()
    {
        canShoot = false;
        isReviving = false;
    }

    public IEnumerator WaitToSetCanShoot(bool can, float wait) {
        yield return new WaitForSeconds(wait);
        canShoot = can;
    }
}
