using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;

[RequireComponent(typeof(AudioSource))]
public class SoliderShooting : MonoBehaviour, IEnemyShooting
{

    public float attackRate;                    // how many times per second the enemy attacks
    public GameObject bullet;                   // the bullet prefab that is shot
    public Transform[] firingPosition;          // the positions that the bullet prefabs will be instantiated at
    public float aimWait = 1f;                  // the time the enemy will wait to shoot. Gives playe time to react
    public string spawnFromPoolTag;             // tag of pool this obj will spawn from when shooting
    public LayerMask blockRaycast;
    public Transform modelRef;

    float nextAttackTime = 0f;                  // keeps track of when the enemy attacks again
    bool doLookAt = true;                       // if the enemy should look at the player
    bool courotineStarted = false;              // if the enemy has started attakcing
    bool operate = false;                       // if the script can start operating in full. toggled after startAttackOffset is done
    float savedAttackRate;                      // saved value for if time is slowed
    float startAttackOffset = 0f;               // initial time to wait until enemy starts operating
    bool enable = false;                        // if the obj is ready to countdown the startAttackOffset

    ARSessionOrigin arOrigin;                   // ref to the ar session origin
    GameObject camera;                          // ref to the player cam
    AudioSource source;                         // ref to the audiosource
    ObjectPooler pooler;                        // ref to the ObjectPooler
    PlayerData playerData;

    void Start()
    {
        // get ar refs
        if (playerData == null)
            playerData = PlayerData.Instance;

        arOrigin = playerData.arOrigin;
        camera = playerData.playerCam;

        source = GetComponent<AudioSource>();
        pooler = ObjectPooler.Instance;
        savedAttackRate = attackRate;
    }

    public void StartShootingProcess() {
        // reset values for starting the obj over when spawned from a pool
        if (playerData == null)
            playerData = PlayerData.Instance;

        arOrigin = playerData.arOrigin;
        camera = playerData.playerCam;
        modelRef.localRotation = Quaternion.Euler(0f, 0f, 0f);

        startAttackOffset = Random.Range(0.25f, 1.5f);
        nextAttackTime = 0f;
        courotineStarted = false;
        operate = false;
        doLookAt = true;
        enable = true;
    }

    public void EndShootingProcess() { }

    // Update is called once per frame
    void Update()
    {
        if (!enable)
        {
            return;
        }
        else {
            // count down the startAttackOffset time to toggle operate
            startAttackOffset -= Time.deltaTime;
            if (startAttackOffset <= 0f) {
                operate = true;
            }
        }

        if (operate)
        {
            // while it aims, it looks at the camera target
            if (doLookAt)
            {
                transform.LookAt(camera.transform);
            }

            // if it is time to attack it checks if the courutine has already been started if not it calls aim
            if (Time.time >= nextAttackTime)
            {
                if (!courotineStarted)
                {
                    Aim();
                }
            }
        }
    }
    /// <summary>
    /// stop looking at player and start aiming process
    /// </summary>
    void Aim() {
        // don't look at the target any longer
        doLookAt = false;

        // start a courutine 
        StartCoroutine(AimWait());
    }

    void Attack() {
        // if can see player shoot
        if (SightCheck()) { 
            GameObject newObj = null;
            // go through each firing position and create a bullet that aims at the camera
            for (int i = 0; i < firingPosition.Length; i++) {
                // spawn object from pool
                newObj = pooler.SpawnFromPool(spawnFromPoolTag, firingPosition[i].position, firingPosition[i].rotation);
                newObj.transform.LookAt(camera.transform);
                newObj.SetActive(true);
            }
            if (source)
            {
                AudioManager.Instance.PlayEnemyShoot(source);
            }
        }

        // can look at the camera again
        doLookAt = true;

        // reset attack timing
        nextAttackTime = Time.time + 1f / attackRate;
    }

    // checks if enemy can see the player
     bool SightCheck() {
        Vector3 direction = camera.transform.position - transform.position;
        Ray ray = new Ray(transform.position, direction);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 2f);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, blockRaycast, QueryTriggerInteraction.Collide)) {
            Debug.Log(hit.collider.name);
            if (hit.collider.CompareTag("MainCamera")) {
                // can see the player
                return true;
            }
        }
        
        return false;
    }

    IEnumerator AimWait() {
        courotineStarted = true;
        // wait for the aim to happen
        yield return new WaitForSeconds(aimWait);
        //solider shoots
        Attack();
        courotineStarted = false;
    }

    public void SetOperate(bool toggle) {
        enable = toggle;
    }
}
