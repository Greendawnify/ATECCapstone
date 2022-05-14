using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;

public class LookAtARCamera : MonoBehaviour
{

    ARSessionOrigin arOrigin;
    GameObject cam;
    PlayerData playerData;
    // Start is called before the first frame update
    void Start()
    {
        playerData = PlayerData.Instance;
        arOrigin = playerData.arOrigin;
        cam = playerData.playerCam;
    }

    private void OnEnable()
    {
        playerData = PlayerData.Instance;
        arOrigin = playerData.arOrigin;
        cam = playerData.playerCam;
    }
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cam.transform);
    }
}
