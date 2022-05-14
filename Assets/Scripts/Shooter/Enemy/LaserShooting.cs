using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;

[RequireComponent(typeof(AudioSource))]
public class LaserShooting : MonoBehaviour, IEnemyShooting
{
    public float attackRate;                            // how many times per second the enemy attacks
    public Transform[] firingPosition;                  // The positions that bullets are shot from
    public float laserAttackDuration = 1f;              // how long the laser attack is
    public GameObject laserChargeUpParticle;            // the charge up object
    public GameObject laser, miniLaser;                 // the 2 laser objects that are used by the laser enemy
    public float rotationSpeed = 1f;                    // the speed the laser enemy roates when shooting the laser
    [Range(0.01f, 0.5f)]
    public float laserDamage;                           // the damage of the laser over time
    public Transform modelRef;

    ARSessionOrigin arOrigin;                           //ref to the ar session origin
    GameObject camera;                                  // ref to the player camera
    AudioSource source;                                 // audiosource for this object
    AudioManager audioManager;
    PlayerData playerData;

    float nextAttackTime = 0f;                          // keeps track of when thelaser will attack again
    [SerializeField] bool enable = false;               // if this object is on or not
    float startAttackOffset = 0f;                       // initial start attack time offset. So enemy does not attack instantly
    [SerializeField] bool operate = false;              // toggled when startAttackOffset is over. Starts process of firing
    bool doLookAt = true;                               // if toggled will look at the player camera
    bool courotineStarted = false;                      // if true then the laser is attakcing
    bool slowRotate = false;                            // if true laser is slow rotating instead of just looking at the player
    PlayerHealth playerHealth;                          // ref to the player health
    Transform aimSpot;                                  // the position that the laser shoots at.

    void Start()
    {
        // get the ar objects
        if(playerData == null)
            playerData = PlayerData.Instance;

        arOrigin = playerData.arOrigin;
        camera = playerData.playerCam;

        source = GetComponent<AudioSource>();
        audioManager = AudioManager.Instance;

        // get the aim position
        playerHealth = camera.gameObject.GetComponent<PlayerHealth>();
        aimSpot = playerHealth.laserAimPoint;
    }

    public void StartShootingProcess() {
        if(playerData== null)
            playerData = PlayerData.Instance;

        arOrigin = playerData.arOrigin;
        camera = playerData.playerCam;
        playerHealth = playerData.playerHealth;
        aimSpot = playerHealth.laserAimPoint;

        startAttackOffset = Random.Range(0.25f, 3f);
        nextAttackTime = 0f;
        laser.SetActive(false);
        miniLaser.SetActive(false);
        modelRef.localRotation = Quaternion.Euler(0f, 0f, 0f);
        

        operate = false;
        doLookAt = true;
        courotineStarted = false;
        slowRotate = false;
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
        else
        {
            // once enabled countdown the start attackOffset
            startAttackOffset -= Time.deltaTime;
            if (startAttackOffset <= 0f && !operate)
            {
                operate = true;
                laserChargeUpParticle.SetActive(true);
            }
        }

        if (operate) {
            if (doLookAt) {
                transform.LookAt(aimSpot);
            }

            // check if it is time to attack again
            if (Time.time >= nextAttackTime)
            {
                Aim();
            }

            // when attakcing instead of lookAt just rotate slowly
            if (slowRotate) {
                Quaternion targetRotation = Quaternion.LookRotation(aimSpot.position - transform.position);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
    /// <summary>
    /// Stop looking at and start Attack process
    /// </summary>
    void Aim()
    {
        doLookAt = false;

        Attack();
        if (!courotineStarted)
        {
            StartCoroutine(LaserAttack());
        }
    }

    /// <summary>
    /// Does all the logic for what the laser is hitting and what to do when hitting certain objs
    /// </summary>
    void Attack()
    {
        RaycastHit hit;
        // make sure I am hitting the right layer mask
        Debug.DrawRay(firingPosition[0].position, firingPosition[0].forward * 50f, Color.cyan);

        if (Physics.Raycast(firingPosition[0].position, firingPosition[0].forward, out hit, 50f, 11, QueryTriggerInteraction.Collide))
        {
            // checks if the laser is going to hit wall
            if (hit.collider.CompareTag("Building") || hit.collider.CompareTag("Enemy"))
            {
                // has hit somthing that should break up the laser
                Debug.Log("Hitting a building");
                laser.SetActive(false);
                miniLaser.SetActive(true);

                // scale the mini laser obj between the laser enemy and the obj it is hitting
                miniLaser.transform.localScale = new Vector3(1f, 1f, Vector3.Distance(hit.point, firingPosition[0].position) / 2f);

                // turn off the ui that indicates the player is being hit by the laser
                playerHealth.ToggleLaserHitScreen(false);
            }
            else if (hit.collider.CompareTag("MainCamera"))
            {
                // hit the player
                Debug.Log("Lasered the player");
                miniLaser.SetActive(false);
                laser.SetActive(true);
                // turn on the ui that inidcates the player is being hit by the laser
                playerHealth.ToggleLaserHitScreen(true);
            }
            else
            {
                Debug.Log("Let me know ifI get called");

            }
        }
        else {
            // hit nothing
            Debug.Log("Not hitting anything");
            miniLaser.SetActive(false);
            laser.SetActive(true);
            playerHealth.ToggleLaserHitScreen(false);
        }
    }

    IEnumerator LaserAttack() {
        // switch the audio tracks
        audioManager.PlayChargeUpLaser(source, false);
        audioManager.PlayShootLaser(source, true);

        courotineStarted = true;
        slowRotate = true;

        laserChargeUpParticle.SetActive(false);
        

        yield return new WaitForSeconds(laserAttackDuration);

        // reset all the objects and values back to when the laser enemy was not attacking
        laserChargeUpParticle.SetActive(true);
        laser.SetActive(false);
        miniLaser.SetActive(false);
        playerHealth.ToggleLaserHitScreen(false);
        courotineStarted = false;
        doLookAt = true;
        slowRotate = false;

        // stop calling Attack function
        nextAttackTime = Time.time + 1f / attackRate;
        
        // reset the audio tracks
        audioManager.PlayShootLaser(source, false);
        audioManager.PlayChargeUpLaser(source, true);

    }

    /// <summary>
    /// Stops the laser in the middle of the attack and resets all the values and objs
    /// </summary>
    public void StopShooting() {
        StopAllCoroutines();

        laserChargeUpParticle.SetActive(true);
        //laserChargeUpParticle.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        courotineStarted = false;
        doLookAt = true;
        slowRotate = false;
        // stop calling Attack function
        nextAttackTime = Time.time + 1f / attackRate;
    }
}
