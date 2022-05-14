using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class GameBoard : MonoBehaviour
{
    [Header("Regular pooled enmey locations")]
    public Transform[] hoverPositions;
    public Transform[] turretPositions;
    public Transform[] laserPositions;         // reference to positions where pooled enemies will be spawned

    [Header("Swarm Info")]
    public Transform[] swarmPosition;
    public GameObject swarmPrefab;
    public List<GameObject> swarms = new List<GameObject>();
    public Transform reviveSpot;

    [Header("Misc objects")]
    public GameObject[] miscObjects;                                    // reference to all the enmies that will not be pooled

    [Header("Board specifics")]
    public Transform moveTarget;                                                // position where the baord can be moved by swipes
    public NavMeshSurface normalSurface;
    public NavMeshSurface swarmSurface;                          // Navmesh for humanoid/swarm movement on the board
    public bool quickStart = false;                                             
    public float smoothTime = 0.3f;                                             // the value for dampening the movement of the the board
    public Transform[] humanoidWaypoints;
    public Transform[] swarmWaypoints;                       //the waypoints for humanoids/swarms
    

    UIController uiController;                                                  // ref to the main UI 
    ObjectPooler pooler;                                                        // ref to ObjectPooler
    Vector3 velocity = Vector3.zero;                                            // a value for smooth dampening
    Vector3 pos;                                                                // pos of this board
    bool levelIsOver = false;                                                   // when the level is over
    List<GameObject> enemies = new List<GameObject>();                          // all the enmies in the level

    private void Start()
    {
        // building the navmesh 
        if(normalSurface)
            normalSurface.BuildNavMesh();

        if(swarmSurface)
            swarmSurface.BuildNavMesh();

        pooler = ObjectPooler.Instance;
        pos = transform.position;
    }
    /// <summary>
    /// Called when the game starts. Places all the enemies in the correct spots and sets them up
    /// </summary>
    public void TurnOnPieces()
    {

        // enables specific scripts for all the persons on the game board
        for (int i = 0; i < hoverPositions.Length; i++)
        {
            GameObject newObj = pooler.SpawnFromPool("Hover", hoverPositions[i].position, hoverPositions[i].rotation);
            newObj.SetActive(true);
            enemies.Add(newObj);

            Waypoint waypointRef = newObj.GetComponent<Waypoint>();
            waypointRef.enabled = true;
            waypointRef.StartWaypoint(humanoidWaypoints);

        }

        // enables specific scripts on all the turrets on the game board
        for (int i = 0; i < turretPositions.Length; i++)
        {
            GameObject newObj = pooler.SpawnFromPool("Turret", turretPositions[i].position, turretPositions[i].rotation);
            newObj.SetActive(true);
            enemies.Add(newObj);
        }

        // enables specific scripts on all the laser enemy on the game board
        for (int i = 0; i < laserPositions.Length; i++) {
            GameObject newObj = pooler.SpawnFromPool("Laser", laserPositions[i].position, laserPositions[i].rotation);
            newObj.SetActive(true);
            enemies.Add(newObj);

            Waypoint waypointRef = newObj.GetComponent<Waypoint>();
            waypointRef.enabled = true;
            waypointRef.StartWaypoint(humanoidWaypoints);
        }

        // enables specific scripts on all the swarms on the game board
        for (int i = 0; i < swarms.Count; i++) {
            swarms[i].transform.position = swarmPosition[i].position;
            swarms[i].SetActive(true);
            enemies.Add(swarms[i]);

            Waypoint waypointRef = swarms[i].GetComponent<Waypoint>();
            waypointRef.enabled = true;
            waypointRef.StartWaypoint(swarmWaypoints);

            SwarmMovement moveScript = swarms[i].GetComponent<SwarmMovement>();
            moveScript.SetReviveSpot(reviveSpot);

            swarms[i].GetComponent<SwarmShooting>().SetCanShoot(true);
            // set up the swarms
        }
        swarms.Clear();

        for (int i = 0; i < miscObjects.Length; i++) {
            miscObjects[i].SetActive(true);
        }


        uiController = FindObjectOfType<UIController>();
    }

    private void Update()
    {
        Dampen();

        if (quickStart) {
            TurnOnPieces();
            quickStart = false;
        }

        if (!levelIsOver && uiController != null)
        {

            // if all the enmies are gone the level is over
            if (enemies.Count > 0)
            {
                for (int i = 0; i < enemies.Count; i++)
                {
                    if (enemies[i].activeInHierarchy)
                    {
                        return;
                    }

                }
            }
            else {
                return;
            }


            levelIsOver = true;
            //all enemies are dead.
            uiController.Win();
            for (int i = 0; i < enemies.Count; i++) {
                enemies[i].SetActive(false);
            }
            
            
        }
    }

    /// <summary>
    /// makes sure the board does not move far away from its starting pos
    /// </summary>
    void Dampen() {
        transform.position = Vector3.SmoothDamp(transform.position, pos, ref velocity, smoothTime);
    }

    public void UpdateDampenPosition(Vector3 p) {
        pos = p;
    }

    public void DisableEnemies() {
        for (int i = 0; i < enemies.Count; i++) {
            enemies[i].SetActive(false);
        }
    }

    public void EnqueEnemies() {
        for (int i = 0; i < enemies.Count; i++) {
            if (enemies[i].GetComponent<IPooledObject>() != null && enemies[i].activeInHierarchy && enemies[i].GetComponent<EnemyHealth>()) {
                EnemyHealth health = enemies[i].GetComponent<EnemyHealth>();

                pooler.EnqueObject(health.poolTag, enemies[i]);
            }
        }
    }

    public void ResetBoard() {
        quickStart = false;
        levelIsOver = false;
        velocity = Vector3.zero;
        enemies.Clear();

        if (swarmPosition.Length > 0) {
            // there are swarms in this level and need to be recreated when restarted
            for (int i = 0; i < swarmPosition.Length; i++) {
                GameObject go = Instantiate(swarmPrefab, swarmPosition[i].position, swarmPosition[i].rotation);
                go.SetActive(false);
                swarms.Add(go);
            }
        }

        pos = transform.position;
    }
}
