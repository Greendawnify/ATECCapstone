using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Waypoint : MonoBehaviour
{
    public Transform[] waypoints;                           // the waypoints on the map the solider can go to
    public float distanceCheck = 0.1f;                      // distnace the sloider has to be from the waypoint to be considered at destination
    public float waitTime = 2f;                             // wait time unitl the solider is sent to another destination


    List<Transform> whereIveBeen = new List<Transform>();   // list of where the solider has already been
    bool lookingForNewLocation = true;                      // if the solider is looking for a new destination
    NavMeshAgent agent;                                     // the nav mesh agent component of the solider
    int choice = -1;                                        // reference to the specific wapoint in the the array


    public void StartWaypoint(Transform[] list) {
        if(agent == null)
            agent = GetComponent<NavMeshAgent>();

        waypoints = list;

        agent.Warp(transform.position);
        lookingForNewLocation = true;
    }

    void Update()
    {

        // if looking for a location call Choose Destination. Happens after waiting
        if (lookingForNewLocation) {
            ChooseDestination();
        }

        // If not lpoking for a new location. Happens if solider has not arrived at location yet
        if (!lookingForNewLocation && choice != -1)
        {
            // checks if solider is at location
            if (Vector3.Distance(transform.position, waypoints[choice].position) <= distanceCheck) {
                // solider is at location
                choice = -1;
                // stop moving
                agent.isStopped = true;
                // start waiting
                StartCoroutine(Wait());
            }
        }
    }

    /// <summary>
    /// Goes through the list and picks a waypoint that has not been visted
    /// </summary>
    void ChooseDestination() {
        if (waypoints.Length == 0)
            return;

        // choose random waypoint
        choice = Random.Range(0, waypoints.Length);

        // if waypoint is in this list recall the function until a waypoint is chosen that is not in this list
        if (whereIveBeen.Contains(waypoints[choice])) {
            // have been there
            choice = -1;
            ChooseDestination();
            
        }
        // set the destination and stop looking for a new destination
        agent.SetDestination(waypoints[choice].position);
        lookingForNewLocation = false;

        // add the waypoint to where the solider has been
        whereIveBeen.Add(waypoints[choice]);

        // if the list of where the solider has been is getting large clear the list
        if (whereIveBeen.Count >= waypoints.Length-1) {
            whereIveBeen.Clear();
        }

    }

    public void PauseAgent(bool stop) {
        agent.isStopped = stop;
    }

    /// <summary>
    /// turns off the script or turns on the script
    /// </summary>
    /// <param name="toggle"></param>
    public void ToggleScript(bool toggle) {
        if (toggle)
        {
            lookingForNewLocation = true;
            choice = 0;
            agent.isStopped = false;
        }
        else {
            StopAllCoroutines();
            agent.isStopped = true;
            whereIveBeen.Clear();
            lookingForNewLocation = false;
            choice = -1;
        }
    }

    IEnumerator Wait() {
        yield return new WaitForSeconds(waitTime);
        // wait is over, is now able to move
        agent.isStopped = false;

        // start process of looking for next destination
        lookingForNewLocation = true;
    }
}
