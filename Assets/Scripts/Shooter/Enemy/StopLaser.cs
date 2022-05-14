using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopLaser : MonoBehaviour
{
    LaserShooting laserShooting;
    // Start is called before the first frame update
    void Start()
    {
        laserShooting = GetComponentInParent<LaserShooting>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StopLaser_ANIM() {
        laserShooting.StopShooting();
    }
}
