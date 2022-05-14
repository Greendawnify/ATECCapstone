using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SwarmMovement : MonoBehaviour
{
    public float halfDamage;                        // half the health of the enemy. Triggers swarm going to revive
    public float reviveRate;                        // how fast the swarm revives
    public Transform revivePosition;                // ref to where the swarm revives from
    public GameObject bugModel;                     // ref to the prefab of the bug in the swarm
    public Transform[] bugPositions;                // all the positions in the swarm
    public GameObject reviveParticle;               // particle when the bug is revived

    EnemyHealth health;                             // ref to the health of the enemy
    Waypoint waypointScript;                        // ref to the waypoint script
    NavMeshAgent agent;                             // ref to the navmesh agent
    Animator anim;                                  // animator that moves the swarm
    SwarmShooting shootScript;                      // script that shoots the swarm
    ParticleSystem particle;                        // the particle from the reviveParticle
    AudioSource source;                             // ref to the audiosource 

    bool headingToReviveSpot = false;               // if the swamr is going to reviev spot
    bool courutineStarted = false;                  // if reviveing porcess has started
    bool isTurning = false;                         // if the swarm is turning to face player or revive spot
    bool isSpawning = false;                        // is the swarm is reviving
    bool[] activeBugInPositons;                     // references to which bugs in the swarm are active
    bool revivingProcess = false;                   // the reviving process is starting checks when the swarm has entered the revive spot
    bool revingHasEnded = true;                     // false when headed to the reving spot. True when the process is over
    bool hasAlreadyRevivedOnce = false;

    void Start()
    {
        waypointScript = GetComponent<Waypoint>();
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<EnemyHealth>();
        anim = GetComponent<Animator>();
        shootScript = GetComponent<SwarmShooting>();
        source = GetComponent<AudioSource>();

        // get the revive particle and turn it off to use later
        GameObject newParticle = Instantiate(reviveParticle);
        particle = newParticle.GetComponent<ParticleSystem>();
        particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        

    }

    private void OnEnable()
    {
        // all bugs are active at the beginning
        activeBugInPositons = new bool[bugPositions.Length];
        for (int i = 0; i < activeBugInPositons.Length; i++)
        {
            
            GameObject newBug = bugPositions[i].GetChild(0).gameObject;
            newBug.transform.rotation = bugPositions[i].rotation;
            newBug.SetActive(true);
            newBug.GetComponent<BugHealth>().bugCollider.enabled = true;

            activeBugInPositons[i] = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // checks if there are any bugs left after revivig once
        if (GetAmountOfActiveBugs() <= 0 && hasAlreadyRevivedOnce) {
            StopAllCoroutines();
            CancelInvoke("CreateANewBug");
            revivePosition = null;
            halfDamage = -1f;
            StartCoroutine(KillWholeSwarm());
        }

        // check if the health is at half
        if (health.GetHP() <= halfDamage && !headingToReviveSpot && !revivingProcess) {
            // run away and get health
            headingToReviveSpot = true;
            isTurning = true;
            GoToReviveSpot();
        }

        if (headingToReviveSpot)
        {
            // rotates to the revive spot
            if (isTurning)
            {
                Quaternion targetRotation = Quaternion.LookRotation(revivePosition.position - transform.position, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f);
                transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            }

        }

        // checks if the swarm near the revive spot
        if (Vector3.Distance(transform.position, revivePosition.position) <= 3.5f && headingToReviveSpot && !revingHasEnded ||
            Vector3.Distance(transform.position, revivePosition.position) <= 3.5f && revivingProcess && !revingHasEnded)
        {
            headingToReviveSpot = false;
            revivingProcess = true;

            // spawns a bug one at a time
            if (!isSpawning)
            {
                isSpawning = true;
                Invoke("CreateANewBug", reviveRate);
            }

            if (!courutineStarted)
            {
                courutineStarted = true;
                // stops the reviving process after a certain amount of time
                StartCoroutine(Reviving());
                //stops turning after a second
                StartCoroutine(Turn());
            }
        }
        else
        {
            CancelInvoke("CreateANewBug");
        }

        // kills the swarm when it is out of health
        if (health.GetHP() <= 0f) {
            StopAllCoroutines();
            CancelInvoke("CreateANewBug");
            revivePosition = null;
            StartCoroutine(KillWholeSwarm());
        }

    }

    /// <summary>
    /// go to revive spot. turn off waypoint script
    /// </summary>
    void GoToReviveSpot() {
        waypointScript.ToggleScript(false);
        agent.radius = 0.5f;
        //waypointScript.PauseAgent(true);
        shootScript.SetCanShoot(false);

        agent.isStopped = false;
        agent.SetDestination(revivePosition.position);

        // stops shooting
        shootScript.SetIsReviving(true);

        revingHasEnded = false;
    }

    /// <summary>
    /// reviev a bug, add health, play particles and sounds
    /// </summary>
    void CreateANewBug() {
        // find 1st inactive bug
        int index = -1;
        for (int i = 0; i < activeBugInPositons.Length; i++) {
            if (!activeBugInPositons[i]) {
                index = i;
                break;
            }
        }
        if (index == -1) return;

        AudioManager.Instance.PlayRevive(source);
        particle.gameObject.transform.position = bugPositions[index].position;
        particle.Play(true);

        // reset position of the bug
        GameObject newBug=  bugPositions[index].GetChild(0).gameObject;
        newBug.transform.rotation = bugPositions[index].rotation;
        newBug.SetActive(true);
        newBug.GetComponent<BugHealth>().bugCollider.enabled = true;

        activeBugInPositons[index] = true;

        // make sure the swarm health is the same as the amount of bugs
        health.SetHealth((float)GetAmountOfActiveBugs());

        // if healed up stop the process and reset values
        if (health.GetHP() >= 6f) {
            Debug.Log("Done spawning");
            StopAllCoroutines();
            isSpawning = false;
            headingToReviveSpot = false;
            revingHasEnded = true;
            hasAlreadyRevivedOnce = true;
            halfDamage = -1f;
            //revivingProcess = false;
            courutineStarted = false;
            agent.radius = 1.25f;
            waypointScript.ToggleScript(true);
            shootScript.SetIsReviving(false);
            shootScript.SetCanShoot(true);
            //waypointScript.PauseAgent(false);
            StartCoroutine(WaitToSpawnAgain());
        }
        isSpawning = false;
    }

    public void SetIsBugActive(int index, bool active) {
        activeBugInPositons[index] = active;
    }

    void SetUpBug(int i, GameObject go) {
        BugHealth bHealth = go.GetComponent<BugHealth>();
        bHealth.swarmPosition = i;
        bHealth.swarm = this;

        //shootScript.SetFirePositionIndex(i, bugModel.transform.GetChild(0));
        shootScript.SetFirePositionIndex(true, i, bugModel.transform.GetChild(0));

        go.transform.SetParent(bugPositions[i]);
        go.transform.position = new Vector3(0f, 0f, 0f);

        activeBugInPositons[i] = true;
    }

    int GetAmountOfActiveBugs() {
        int amount = 0;
        /*for (int i = 0; i < activeBugInPositons.Length; i++) {
            if (activeBugInPositons[i]) {
                amount++;
            }
        }*/

        for (int i = 0; i < bugPositions.Length; i++) {
            Transform childTransform = bugPositions[i].GetChild(0);
            if (childTransform.gameObject.activeInHierarchy) {
                amount++;
            }
        }

        return amount;
    }

    public void SetReviveSpot(Transform tran) {
        revivePosition = tran;
    }

    private void OnDisable()
    {
         headingToReviveSpot = false;
         courutineStarted = false; 
         isTurning = false;
         isSpawning = false; 
         revivingProcess = false;
         revingHasEnded = true;
    }

    IEnumerator Reviving() {
        yield return new WaitForSeconds(6f);
        Debug.Log("Done spawning");
        isSpawning = false;
        headingToReviveSpot = false;
        revingHasEnded = true;
        hasAlreadyRevivedOnce = true;
        halfDamage = -1f;
        //revivingProcess = false;
        courutineStarted = false;
        agent.radius = 1.25f;
        waypointScript.ToggleScript(true);
        shootScript.SetIsReviving(false);
        shootScript.SetCanShoot(true);
        //waypointScript.PauseAgent(false);
        StartCoroutine(WaitToSpawnAgain());
    }

    IEnumerator Turn() {
        yield return new WaitForSeconds(2f);
        isTurning = false;
    }

    IEnumerator WaitForParticleToEnd() {
        yield return new WaitForSeconds(1f);
        particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    IEnumerator WaitToSpawnAgain() {
        yield return new WaitForSeconds(5f);
        revivingProcess = false;
    }

    IEnumerator KillWholeSwarm() {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

}
