using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using UnityEngine.UI;

public class SetupTest : MonoBehaviour
{
    public GameObject placeObject;                          // the final game board for this level
    public GameObject ghostObject;                          // the box object that shpws the user how big the board will be
    public GameObject setButton;                            // button for setting the ghost object and spawning the final board
    public GameObject finishedButton;                       // button when user is finshed
    public GameObject sliderPanel;                          // panel has sliders and info on it
    public GameObject startGamePanel;                       // panel with info for transitioning to start the game
    public GameObject infoPanel;                            // panel that holds info
    public GameObject InGameUI;                             // the in game UI that appears after setting up the ar elements
    public GameObject firstUXScreenPanel;                   // ux screen for the tutorial level
    public GameObject secondUXScreenPanel;                  // ux screen for the swarm enemy
    public GameObject thirdUXScreenPanel;                   // ux screen for laser enemy
    public GameObject fourthUXScreenPanel;                  // ux screen for shields
    public PlayerShooting shootingScript;                   // ref to the player shooting
    public GameObject restartingArSearchText;               // text to tell the user that the ar search is starting over
    public float baordDragSpeed = 1f;                       // the speed the baord get moved when dragginf on the screen
    public bool firstUxScreen;
    public bool secondUXScreen;
    public bool thirdUXScreen;
    public bool fourthUXScreen;

    ARSessionOrigin arOrigin;                               // ref to the ar session orign
    ARPlaneManager planeManager;                            // ref to the plane manager on the ar orgin
    GameObject playerCam;                                   // the ar player camera ref
    PlayerData playerData;

    List<ARPlane> planeList = new List<ARPlane>();          // list of all the ar planes during this script
    bool checkForPlanes = false;                            // if we are looking for ar planes
    bool readyToAccept = false;                             // if the final board has been placed
    bool canSwipe = false;                                  // if the user can swipe to move the baord
    GameObject acceptedObject;                              // ref to ghost object being placed
    GameObject finalObject;                                 // ref to the final board being placed
    GameBoard board;                                        // ref to the Game Board script on the final baord obj
    bool uxScreenHasBeenShown = true;                       // if the ux screen has been shown already
    MusicManager musicManager;                              // controls the music in the game
    AudioManager audioManager;                              // audio manager that is used to call buttons sounds
    List<GameObject> ghostPlanes = new List<GameObject>();  // all the ghost objects
    float arTimer = 60f;
    float timer;

    Transform boardMoveTransform;                           // ref to where the baord will be moved from
    Vector2 touchPosition;                                  // position that the swipes are being made from the phone

    // Start is called before the first frame update
    void Start()
    {
        playerData = PlayerData.Instance;
        arOrigin = playerData.arOrigin;
        planeManager = arOrigin.GetComponent<ARPlaneManager>();
        playerCam = playerData.playerCam;
        musicManager = MusicManager.Instance;
        audioManager = AudioManager.Instance;
        timer = arTimer;

        if (firstUxScreen || secondUXScreen || thirdUXScreen || fourthUXScreen) {
            uxScreenHasBeenShown = false;
        }


        try
        {
            musicManager.PlayStabByUI();
        }
        catch { }
    }



    // Update is called once per frame
    void Update()
    {
        if (readyToAccept) {
            finishedButton.SetActive(true);
            readyToAccept = false;
        }

        if (checkForPlanes) {
            var planes = planeManager.trackables;

            // if there are some planes check if they are unique ornot
            if (planes.count > 0 && planes.count <=5) {
                ARPlane plam;
                foreach (var p in planes) {

                    if (planes.TryGetTrackable(p.trackableId, out plam))
                    {
                        bool isNotInList = true;
                        for (int i = 0; i < planeList.Count; i++) {
                            if (plam.Equals(planeList[i])) {
                                isNotInList = false;
                                break;
                            }
                            
                        }

                        if (!isNotInList)
                        {
                            // not a unique plane
                            // is in the list. We dont need to do anythi else
                            break;
                        }
                        else {
                            // new plane. a unique plane
                            // add it to the list of planes found and add the ghostobject to each one
                            planeList.Add(plam);
                            infoPanel.SetActive(false);
                            GameObject ghost = Instantiate(ghostObject, p.center, p.transform.rotation);
                            ghostPlanes.Add(ghost);
                        }
                    }
                    
                }
            }
            if (planeList.Count == 0)
            {
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    // time has expried at the ar plane manager has not found any planes.
                    // Let the player know. and Restart the search
                    checkForPlanes = false;
                    setButton.SetActive(true);
                    infoPanel.SetActive(false);
                    StartCoroutine(RestartArSearch());
                    timer = arTimer;
                }
            }

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
                if (DoRaycast(Input.GetTouch(0).position))
                    checkForPlanes = false;
            }

            
        }

        

        if (canSwipe) {
            if (Input.touchCount > 0)
            {
                TakeSwipeInput(Input.GetTouch(0));
            }// the rest will be for testing
            /*else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                TakeArrowInput(0);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                TakeArrowInput(1);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                TakeArrowInput(2);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                TakeArrowInput(3);
            }*/
        }
    }

    void TakeSwipeInput(Touch touch) {
        if (touch.phase == TouchPhase.Began)
        {
            // start of drag
            touchPosition = touch.position;
            Debug.Log("Start finger positon " + touchPosition);
        }
        else if (touch.phase == TouchPhase.Moved) {
            Debug.Log("New finger position " + touch.position);
            Vector2 newVector = touch.position - touchPosition;

            Debug.Log("Direction " + newVector.normalized);

            float xValue = newVector.normalized.x;
            float yValue = newVector.normalized.y;

            if (Mathf.Abs(xValue) > Mathf.Abs(yValue))
            {
                boardMoveTransform.position += (xValue >= 0f) ? playerCam.transform.right.normalized/10 : -playerCam.transform.right.normalized/10;
            }
            else {
                boardMoveTransform.position += (yValue >= 0f) ? new Vector3(0f, baordDragSpeed, 0f) : new Vector3(0f, -baordDragSpeed, 0f);
            }

            //board.UpdateDampenPosition(boardMoveTransform.position);
        }
    }

    void TakeArrowInput(int dir) {
        switch (dir) {
            case 0: // left arrow
                Vector3 direction = -playerCam.transform.right;
                boardMoveTransform.position += direction;
                break;
            case 1:// right arrow
                Vector3 _direction = playerCam.transform.right;
                boardMoveTransform.position += _direction;

                break;
            case 2:// up arrow
                boardMoveTransform.position += new Vector3(0f, baordDragSpeed, 0f);
                break;
            case 3: // down arrow
                boardMoveTransform.position += new Vector3(0f, -baordDragSpeed, 0f);
                break;
        }
    }

    /// <summary>
    /// turns on the ar search for ar planes
    /// </summary>
    /// <param name="button"></param>
    public void Scan(GameObject button)
    {
        checkForPlanes = true;
        setButton.SetActive(false);
        infoPanel.SetActive(true);
    }

    /// <summary>
    /// When the user accepts the ghost object as the place for the final board
    /// </summary>
    public void AcceptPosition() {
        if (acceptedObject != null)
        {
            ButtonSound();

            checkForPlanes = false;
            finishedButton.SetActive(false);
            PlaceCorrectObject();

            for (int i = 0; i < ghostPlanes.Count; i++)
            {
                if(ghostPlanes[i])
                    ghostPlanes[i].SetActive(false);
            }

            sliderPanel.SetActive(true);
            infoPanel.SetActive(false);
            board = finalObject.GetComponent<GameBoard>();
            boardMoveTransform = board.moveTarget;
        }
    }

    /// <summary>
    /// Called when done with the slider panel. And can now swipe to move the board around
    /// </summary>
    public void DoneWithSliders() {
        sliderPanel.SetActive(false);
        startGamePanel.SetActive(true);
        canSwipe = true;
    }

    void PlaceCorrectObject() {
        finalObject =Instantiate(placeObject, acceptedObject.transform.position, acceptedObject.transform.rotation);
        Destroy(acceptedObject);
        //to get this location when retrying this level. You need to get this ref again.
        //acceptedObject.SetActive(false);
    }

    /// <summary>
    /// If raycast hit the ghost object that object is the accpetedObject
    /// </summary>
    /// <param name="touch"></param> positoin the player touched on the screen
    bool DoRaycast(Vector2 touch) {
        Ray ray = arOrigin.camera.ScreenPointToRay(new Vector3(touch.x, touch.y, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
            if (hit.collider.CompareTag("Respawn")) {
                if (acceptedObject != null) {
                    Renderer otherRend = acceptedObject.GetComponent<Renderer>();
                    otherRend.material.color = Color.white;
                    acceptedObject = null;
                }

                Renderer rend = hit.collider.GetComponent<Renderer>();
                rend.material.color = Color.green;
                acceptedObject = hit.collider.gameObject;
                readyToAccept = true;
                return true;
            }
        }
        return false;
    }

    
    /// /// <summary>
    /// scales the ar world based on the value of the slider
    /// </summary>
    /// <param name="slider"></param>
    public void OnScaleSliderChange(Slider slider)
    {
        arOrigin.transform.localScale = new Vector3(slider.value, slider.value, slider.value);
    }

    /// <summary>
    /// rotates the final baord around based on the value of the slider
    /// </summary>
    /// <param name="slider"></param>
    public void OnRotationSliderChange(Slider slider)
    {
        if (boardMoveTransform != null)
        {
            boardMoveTransform.rotation = Quaternion.Euler(0f, slider.value, 0f);
        }
    }

    /// <summary>
    /// sets up all the objects in the world for the game to offically start
    /// </summary>
    public void StartLevel() {
        ButtonSound();
        if (OptionDataScreen())
        {
            musicManager.PlayGameMusic();
            InGameUI.SetActive(true);
            board.TurnOnPieces();
            shootingScript.enabled = true;
            shootingScript.SetCanShoot(true);
            //Destroy(gameObject);
            gameObject.SetActive(false);
        }
    }

    public void ResetUI() {
        readyToAccept = false;
        canSwipe = false;
        checkForPlanes = false;

        firstUXScreenPanel.SetActive(false);
        secondUXScreenPanel.SetActive(false);
        thirdUXScreenPanel.SetActive(false);
        fourthUXScreenPanel.SetActive(false);

        startGamePanel.SetActive(false);
        sliderPanel.SetActive(true);

        

        // reset the UI to show the Sliders -> swipe movement -> start game
        arOrigin.transform.localScale = new Vector3(5f, 5f, 5f);

        board.gameObject.SetActive(true);
        

    }

    /// <summary>
    /// Figures out if this level is supposed to show off a UX screen
    /// </summary>
    /// <returns></returns>
    bool OptionDataScreen()
    {
        if (uxScreenHasBeenShown) {
            return true;
        }

        OptionData data = SaveScoreSystem.GetOptionData();
        if (data == null)
        {
            Debug.LogError("OPtions data was not found");
            return true;
        }

        if (!data.showPregameInfo) { // player set in the option menu that they dont want to see ux screens
            return true;
        }

        if (firstUxScreen)
        {
            firstUXScreenPanel.SetActive(true);
            uxScreenHasBeenShown = true;
            return false;
        }
        else if (secondUXScreen)
        {
            secondUXScreenPanel.SetActive(true);
            uxScreenHasBeenShown = true;
            return false;

        }
        else if (thirdUXScreen)
        {
            thirdUXScreenPanel.SetActive(true);
            uxScreenHasBeenShown = true;
            return false;

        }
        else if (fourthUXScreen) {
            fourthUXScreenPanel.SetActive(true);
            uxScreenHasBeenShown = true;
            return false;
        }
        Debug.LogError("Error with OPtion Data Screen in Setup Test");
        return true;

    }

    public void ButtonSound() {
        audioManager.PlayButtonSound();
    }

    IEnumerator RestartArSearch() {
        restartingArSearchText.SetActive(true);
        yield return new WaitForSeconds(2f);
        restartingArSearchText.SetActive(false);
    }
}
