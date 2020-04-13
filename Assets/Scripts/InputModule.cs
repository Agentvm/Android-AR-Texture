using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputModule : MonoBehaviour
{
    // Static instance of InputModule which allows it to be accessed by any other script.
    public static InputModule Instance = null;

    // Delegates for Touch Events
    public delegate void PlaneTouchDelegate (Vector3 position );
    private List<PlaneTouchDelegate> functionsOnPlaneTouch = new List<PlaneTouchDelegate> ();
    //public delegate void EmptyTouchDelegate (Vector3 position );
    //public List<EmptyTouchDelegate> functionsOnEmptyTouch = new List<EmptyTouchDelegate> ();

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
    RaycastHit rayHit;
    Ray ray;

    // Properties
    public bool TouchInputActive { get => touchInputActive; }
    public Vector3 MousePoint { get => touchPoint; }

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
        if ( Input.touchCount > 0/* || InputHelper.GetTouches ().Count > 0*/)
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

    // Takes a function and adds it to the list of functions that gets executed every time a plane is touched
    public void SubscribeFunctionToPlaneTouch (PlaneTouchDelegate function )
    {
        functionsOnPlaneTouch.Add (function);
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
                        // Shoot a ray from the camera through the mouse position into the scene and store the collision point in touchPoint
                        // Also get the transform of the object that was hit
                        Transform raycastObjectTransform = TransformRaycast (Input.mousePosition);

                        // If functions have been stored in this list by other scripts
                        if ( raycastObjectTransform && raycastObjectTransform.tag == "Plane" && functionsOnPlaneTouch.Count > 0 )
                        {
                            // Iterate through all functions stored
                            foreach ( PlaneTouchDelegate delegateFunction in functionsOnPlaneTouch )
                            {
                                // Execute the stored function
                                delegateFunction (touchPoint);
                            }
                        }
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

            // Shoot a ray from the camera through the mouse position into the scene and Store the collision point in touchPoint
            // Also get the transform of the object that was hit
            Transform raycastObjectTransform = TransformRaycast (Input.mousePosition);
            
            // If functions have been stored in this list by other scripts
            if ( raycastObjectTransform && raycastObjectTransform.tag == "Plane" && functionsOnPlaneTouch.Count > 0 )
            {
                // Iterate through all functions stored
                foreach ( PlaneTouchDelegate delegateFunction in functionsOnPlaneTouch )
                {
                    // Execute the stored function
                    delegateFunction (touchPoint);
                }
            }
        }
    }

    // Do a raycast from the main camera to the specified position, set the mouse position and return the object hit
    Transform TransformRaycast ( Vector3 screen_point )
    {
        if ( !CheckRaycast (screen_point) ) return null;

        ray = mainCamera.ScreenPointToRay (screen_point);
        if ( Physics.Raycast (ray, out rayHit) )
        {
            touchPoint = rayHit.point;
            return rayHit.transform;
        }
        else return null;
    }

    //Vector3 PositionRaycast ( Vector3 screenPoint )
    //{
    //    if ( !CheckRaycast (screenPoint) ) return Vector3.zero;

    //    ray = mainCamera.ScreenPointToRay (screenPoint);
    //    if ( Physics.Raycast (ray, out rayHit) )
    //    {
    //        mousePoint = rayHit.point;
    //        Debug.Log ("Raycasting");
    //        return rayHit.point;
    //    }
    //    else return Vector3.zero;
    //}

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

        // everything is fine
        return true;
    }

}
