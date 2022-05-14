using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using UnityEngine.EventSystems;



public class DragArObject : MonoBehaviour
{
    public GameObject readyBUtton;
    public GameObject prefabBox;

    ARSessionOrigin arOrigin;
    ARPlaneManager planeManager;
    ARPlane plane;

    public GameObject placedObj;
    bool hasNotSpawnedBox = true, canSwipe =false;
    int swiping = -1;
    Vector2 touchPosition;
    float timer = 2f;
    // Start is called before the first frame update
    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        planeManager = arOrigin.GetComponent<ARPlaneManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasNotSpawnedBox)
        {
            var planes = planeManager.trackables;

            if (planes.count > 0)
            {
                // there is a plane
                foreach (var p in planes)
                {
                    placedObj = Instantiate(prefabBox, p.center, p.transform.rotation);
                    hasNotSpawnedBox = false;
                    readyBUtton.SetActive(true);
                }
            }
        }

        if (canSwipe) {
            //TakeSwipeInput();
            TakeMouseInput();
        }
    }

    public void StartSwiping() {
        canSwipe = true;
    }

    void TakeSwipeInput() {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // start of the swipe
            swiping = 1;
            touchPosition = Input.GetTouch(0).position;
            Debug.Log("Start finger positon " + touchPosition);
        }
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) {
            // finger in new position
            Debug.Log("New finger position " + Input.GetTouch(0).position);
            Vector2 newVector = Input.GetTouch(0).position - touchPosition;

            Debug.Log("Direction " + newVector);

            float xValue = newVector.normalized.x;
            float yValue = newVector.normalized.y;


            if (Mathf.Abs(xValue) > Mathf.Abs(yValue))
            {
                placedObj.transform.position += (xValue >= 0f) ? new Vector3(5f, 0f, 0f) : new Vector3(-5f, 0f, 0f);

            }
            else
            {
                placedObj.transform.position += (yValue >= 0f) ? new Vector3(0f, 5f, 0f) : new Vector3(0f, -5f, 0f);
            }
        }


    }

    void TakeMouseInput() {
        if (Input.GetMouseButtonDown(0))
        {
            // start of the swipe
            swiping = 1;
            touchPosition = Input.mousePosition;
            Debug.Log("Start finger positon " + touchPosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // finger in new position
            Debug.Log("New finger position " + Input.mousePosition);
            Vector2 newVector = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - touchPosition;

            Debug.Log("Direction " + newVector.normalized);


            float xValue = newVector.normalized.x;
            float yValue = newVector.normalized.y;

            if (Mathf.Abs(xValue) > Mathf.Abs(yValue))
            {
                //moving either left or right
                if (xValue >= 0f)
                {
                    // moving right
                    placedObj.transform.position += new Vector3(5f, 0f, 0f);
                }
                else {
                    // moving left
                    placedObj.transform.position += new Vector3(-5f, 0f, 0f);
                }
            }
            else {
                // moving up or down
                if (yValue >= 0f)
                {
                    //moving up
                    placedObj.transform.position += new Vector3(0f, 5f, 0f);
                }
                else {
                    // moving down
                    placedObj.transform.position += new Vector3(0f, -5f, 0f);
                }
            }
        }
    }
}
