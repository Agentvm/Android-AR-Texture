using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class InputModule : MonoBehaviour
{
    // Static instance of InputModule which allows it to be accessed by any other script.
    public static InputModule Instance = null;

    // Delegate for Touch Events
    public delegate void TouchDelegate (RaycastHit rayCastHit );
    private List<TouchDelegate> functionsExectuedOnTouch = new List<TouchDelegate> ();

    // Input Variables 
    private bool touchInputActive = false;

    // Configuration
    [Range(0f, 30f)][SerializeField] private float tapThreshold = 15f;
   
    // Touch Variables
    private Vector2 touchStartPosition;
    private List<int> knownTouchIds = new List<int> ();
    private float timeTouchEnded;
    float timeNoTouchSince = 0f;
    float touchTimeDelay = 1.2f;

    // Mouse Point and Click
    private Vector3 touchPoint = new Vector3 (10f, 0f, 0f); // last click / touch position

    // Mouse Timing
    float lastClickTime = 0f;
    float clickDelay = 0.3f;

    // Raycasting
    Camera mainCamera;
    Ray ray;

    // Properties
    public bool TouchInputActive { get => touchInputActive; }
    public Vector3 TouchPoint { get => touchPoint; }

    // Singleton
    void Awake ()
    {
        // check that there is only one instance of this and that it is not destroyed on load
        if ( Instance == null )
            Instance = this;
        else if ( Instance != this )
            Destroy (gameObject);
        DontDestroyOnLoad (gameObject);
    }

    // initiate
    private void Start ()
    {
        // Get Scene Camera
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // Touch input
        if ( Input.touchCount > 0 )
        {
            // Get touch information
            touchInputActive = true;
            timeNoTouchSince = Time.time;
            touchAndTouch ();
        }
        // Mouse Input Active
        else if (Time.time > timeNoTouchSince + touchTimeDelay )
        {
            // Disable Touch
            touchInputActive = false;

            // Mouse Behaviour
            mousePointAndClick ();
        }
    }

    // Takes a function and adds it to the list of functions that gets executed every time a touch is registered
    public void SubscribeToTouch (TouchDelegate callbackFunction )
    {
        functionsExectuedOnTouch.Add (callbackFunction);
    }

    // execute all functions that subscribe to a touch
    void executeCallbackFunctions (RaycastHit raycastHit )
    {
        if ( functionsExectuedOnTouch.Count == 0 ) return;

        // Iterate through all functions stored
        foreach ( TouchDelegate delegateFunction in functionsExectuedOnTouch )
        {
            // Execute the stored function
            delegateFunction (raycastHit);
        }
    }

    // Handle touch input
    void touchAndTouch ()
    {
        List<int> currentTouchIds = new List<int> ();

        // Iterate through all touches
        foreach (Touch touch in Input.touches/*InputHelper.GetTouches ()*/ )
        {
            currentTouchIds.Add (touch.fingerId);

            // First Touch
            if ( knownTouchIds.Count == 0 || (knownTouchIds.Count > 0 && touch.fingerId == knownTouchIds[0]) )
            {

                // On first sensing the touch
                if ( touch.phase == TouchPhase.Began )
                    touchStartPosition = touch.position;

                // When the touch contact is ending
                else if ( touch.phase == TouchPhase.Ended )
                {
                    // Check if tapped
                    if ( (touchStartPosition - touch.position).magnitude < tapThreshold )
                    {
                        // Shoot a ray from the camera through the mouse position into the scene and store the hit info
                        RaycastHit raycastHit = Raycast (touch.position);

                        // call all functions that subscribed to the touch
                        if ( raycastHit.transform )
                            executeCallbackFunctions (raycastHit);
                    }

                    // At the end of touch, remove it's ID from the list of known ID's
                    knownTouchIds.Remove (touch.fingerId);
                    timeTouchEnded = Time.time;

                }

                    // At the end of touch, remove it's ID from the list of known ID's
                    knownTouchIds.Remove (touch.fingerId);

            }

            // add unknown touch id to the list of known ids
            if ( !knownTouchIds.Contains (touch.fingerId) )
                knownTouchIds.Add (touch.fingerId);

        }

        // after all touches have been treated, delete finger touch id's that are no longer viable
        for ( int i = 0; i < knownTouchIds.Count; i++ )
        {
            if ( !currentTouchIds.Contains (knownTouchIds[i] ))
            {
                knownTouchIds.RemoveAt (i);
                i--;
            }
        }


    }

    // Use the mouse to test in Editor
    void mousePointAndClick ()
    {
        // Mouse clicked?
        if ( Input.GetMouseButton (0) && Time.time > lastClickTime + clickDelay )
        {
            lastClickTime = Time.time;

            // Shoot a ray from the camera through the mouse position into the scene and store the hit info
            RaycastHit raycastHit = Raycast (Input.mousePosition);

            // call all functions that subscribed to the touch
            if ( raycastHit.transform )
                executeCallbackFunctions (raycastHit );
        }
    }

    // Do a raycast from the main camera to the specified position, set the mouse position and return the object hit
    RaycastHit Raycast ( Vector3 screen_point )
    {
        // prepare raycast
        RaycastHit raycastHit = new RaycastHit ();
        ray = mainCamera.ScreenPointToRay (screen_point);

        // check ray integrity
        if ( !CheckRaycast (screen_point) ) return raycastHit;

        // cast ray
        Physics.Raycast (ray, out raycastHit);
        return raycastHit;
    }

    // validates Raycast point
    bool CheckRaycast (Vector3 screenPoint )
    {
        // is main camera missing?
        if ( !mainCamera ) return false;

        // is point in front of camera?
        Vector3 viewport_point = mainCamera.ScreenToViewportPoint(screenPoint);
        if ( viewport_point.x > 1 || viewport_point.x < 0 ||
            viewport_point.y > 1 || viewport_point.y < 0 ||
            viewport_point.z < 0 )
        {
            return false;
        }

        if ( EventSystem.current && EventSystem.current.IsPointerOverGameObject () ) return false;

        // everything is fine
        return true;
    }

}
