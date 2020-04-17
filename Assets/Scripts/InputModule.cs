using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class InputModule : MonoBehaviour
{
    // Static instance of InputModule which allows it to be accessed by any other script.
    private static InputModule instance = null;

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
    float clickDelay = 0.1f;

    // Raycasting
    Camera mainCamera;
    Ray ray;

    // Properties
    public static InputModule Instance { get => instance; }
    public bool TouchInputActive { get => touchInputActive; }
    public Vector3 TouchPoint { get => touchPoint; }

    // Singleton
    void Awake ()
    {
        // check that there is only one instance of this and that it is not destroyed on load
        if ( instance == null )
            instance = this;
        else if ( instance != this )
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

        // check ray integrity and check if UI blocking
        if ( !CheckRaycast (screen_point) || (EventSystem.current && EventSystem.current.IsPointerOverGameObject () ))
            return raycastHit;

        // cast ray
        Physics.Raycast (ray, out raycastHit);

        // If a model containing a skinned SkinnedMeshRenderer was hit, update the MeshCollider and cast another ray to get the right uv coordinates
        if ( raycastHit.transform && raycastHit.transform.GetComponent<SkinnedMeshRenderer> () && raycastHit.transform.GetComponent<MeshCollider> () )
        {
            recalculateCollisionMesh (  raycastHit.transform.GetComponent<SkinnedMeshRenderer> (),
                                        raycastHit.transform.GetComponent<MeshCollider> () );

            Physics.Raycast (ray, out raycastHit);
        }

        return raycastHit;
    }

    void recalculateCollisionMesh (SkinnedMeshRenderer skinnedMeshRenderer, MeshCollider meshCollider )
    {
        // DEBUG: Timing
        //float start_time = Time.realtimeSinceStartup;

        // Bake the current status of the mesh
        Mesh mesh = new Mesh();
        skinnedMeshRenderer.BakeMesh (mesh);


        //float baking_time = (Time.realtimeSinceStartup - start_time);
        //float interval_time = Time.realtimeSinceStartup;

        // Re-scale vertices
        Vector3[] verts = mesh.vertices;
        float scale = 1.0f/skinnedMeshRenderer.transform.lossyScale.y;
        for ( int i = 0; i < verts.Length; i+=1 )
            verts[i] = verts[i] * scale;
        mesh.vertices = verts;
        
        //float re_scaling_time = (float)(Time.realtimeSinceStartup - interval_time);
        //interval_time = Time.realtimeSinceStartup;

        mesh.RecalculateBounds ();
        
        //float bounds_time = (Time.realtimeSinceStartup - interval_time);
        //interval_time = Time.realtimeSinceStartup;

        // Assign calculated mesh to meshCollider
        meshCollider.sharedMesh = mesh;

        //float assign_time = (Time.realtimeSinceStartup - interval_time);

        //float overall_time = (Time.realtimeSinceStartup - start_time);
        //Debug.Log ("Overall: " + overall_time + " seconds");

        //Debug.Log ("Baking: " + ((baking_time/overall_time)*100) + "%");
        //Debug.Log ("Re-scaling: " + ((re_scaling_time / overall_time) * 100) + "%");
        //Debug.Log ("Bounds re-calculating: " + ((bounds_time / overall_time) * 100) + "%");
        //Debug.Log ("Assigning: " + ((assign_time / overall_time) * 100) + "%");
        
    }

    // validates Raycast point
    bool CheckRaycast (Vector3 screenPoint )
    {
        // Is main camera missing?
        if ( !mainCamera ) return false;

        // Is point in front of camera?
        Vector3 viewport_point = mainCamera.ScreenToViewportPoint(screenPoint);
        if ( viewport_point.x > 1 || viewport_point.x < 0 ||
            viewport_point.y > 1 || viewport_point.y < 0 ||
            viewport_point.z < 0 )
        {
            return false;
        }

        // Pointing on UI?
        if ( EventSystem.current && EventSystem.current.IsPointerOverGameObject () ) return false;

        // Everything is fine
        return true;
    }

}
