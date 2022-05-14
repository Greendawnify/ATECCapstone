using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance { get; private set; }

    public  ARSessionOrigin arOrigin;                                     // a single reference to the player cam for use by other scripts
    public  GameObject playerCam;                                         // a ref to the player cam for use for other scripts
    public  PlayerHealth playerHealth;                                    //a ref to the health of the player

    private void Awake()
    {
        // making it a singleton
        if (Instance == null)
        {
            Instance = this;
            arOrigin = FindObjectOfType<ARSessionOrigin>();
            playerCam = arOrigin.camera.gameObject;
            playerHealth = playerCam.GetComponent<PlayerHealth>();
        }
        else {
            Destroy(gameObject);
        }
    }

}
