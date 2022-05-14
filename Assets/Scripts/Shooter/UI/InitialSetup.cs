using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using UnityEngine.UI;

public class InitialSetup : MonoBehaviour
{
    public GameObject placeObject;                                          // the object that gets placed when you touch a plane in ar session space
    public GameObject inGameUI;                                             // the UI that will be on for the rest of the game. Gets turned on when done with placing base
    public PlayerShooting shootingScript;
    public GameObject finishButton;

    ARSessionOrigin arOrigin;                                               // ar session origin
    ARRaycastManager raycastManager;                                        // ar raycast manager, helps with raycasting
    bool shootRaycast = false;
    Camera camera;
    GameObject lastObject;
    GameBoard board;
    Image buttonImage;


    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        raycastManager = arOrigin.GetComponent<ARRaycastManager>();
        camera = arOrigin.GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        if (shootRaycast) {
            // when the user touches the screen
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
                Raycast(Input.GetTouch(0).position);
            }
        }
    }

    void Raycast(Vector2 touch) {
        buttonImage.color = Color.black;
        // transfer the touch position to the game position
        var screenCenter = camera.GetComponent<Camera>().ViewportToScreenPoint(new Vector3(touch.x, touch.y));
        var hits = new List<ARRaycastHit>();

        // do the raycast looking for ar planes
        raycastManager.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);


        // if something was hit
        if (hits.Count > 0) {
            buttonImage.color = Color.yellow;
            // destroy the last object that was created. If there is one
            if (lastObject) {
                Destroy(lastObject);
                board = null;
            }


            buttonImage.color = Color.green;
            // create the object at the postion the raycast hit
            lastObject = Instantiate(placeObject, hits[0].pose.position, placeObject.transform.rotation);

            // set up the board
            board = lastObject.GetComponent<GameBoard>();
            shootRaycast = false;
            finishButton.SetActive(true);
            buttonImage.color = Color.magenta;
        }

        
    }


    // called when the slider to change rotation is called
    public void OnRotationSliderChange(Slider slider) {
        arOrigin.transform.rotation = Quaternion.Euler(0f, slider.value, 0f);
    }

    // called when the slider to change scale is called
    public void OnScaleSliderChange(Slider slider) {
        arOrigin.transform.localScale = new Vector3(slider.value, slider.value, slider.value);
    }

    public void SetPositionButton(GameObject button) {
        shootRaycast = true;
        buttonImage = button.GetComponent<Image>();
        buttonImage.color = Color.red;

    }

    // called when the the user is done setting up the board
    public void FinishedButtonClicked() {
        /*
        // get all the planes that are being tracked
        var planes = arOrigin.GetComponent<ARPlaneManager>().trackables;
        // destroy the manager of the planes
        Destroy(arOrigin.GetComponent<ARPlaneManager>());

        // go through each plane and destroy it
        foreach (var p in planes) {
            Destroy(p.gameObject);
        }
        */
        // trun on in Game Canvas UI
        inGameUI.SetActive(true);

        board.TurnOnPieces();
        shootingScript.enabled = true;
        // destroys the set up ui and end this code.
        Destroy(gameObject);
    }
}
