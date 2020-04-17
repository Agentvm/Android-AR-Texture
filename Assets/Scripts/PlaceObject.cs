using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Subscribes to Touch Event. On touch, instantiates one of the preloaded object at the
    raycasted position and turns it towards the camera.
*/
public class PlaceObject : MonoBehaviour
{
    // Components
    private Camera mainCamera;

    // Objects
    [SerializeField] private Object[] objectsToInstantiate;
    private List<GameObject> preloadedObjects = new List<GameObject> ();
    private bool preloadComplete = false;

    // Start is called before the first frame update
    void Start()
    {
        // Subscribe to user touch events, PlaceOnPlane becomes a callback function
        InputModule.Instance.SubscribeToTouch (PlaceOnPlane);
        mainCamera = Camera.main;

        // Quickly load and then destroy all prefabs so the screen does not freeze on first placement
        foreach (Object objectToInstantiate in objectsToInstantiate)
        {
            preloadedObjects.Add ((GameObject)Instantiate (objectToInstantiate, new Vector3 (0f, 0f, 20f), Quaternion.identity, this.transform ));
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        // Destroy preloaded objects shortly after the start of the app
        if ( !preloadComplete && Time.time > 0.5f)
        {
            for( int i = 0; i < preloadedObjects.Count; i++ )
            {
                // destoy the preloaded Instance
                Destroy (preloadedObjects[i]);

                // make sure the Garbage is collected by overwriting the object
                preloadedObjects[i] = new GameObject ();
                Destroy (preloadedObjects[i]);
            }

            preloadedObjects.Clear ();
            preloadComplete = true;
        }
    }

    // Get the raycast information from the touch event
    // Make a new Instance of one of the objects assigned in the Inspector,
    // place it at the touched position and rotate it towards the camera
    void PlaceOnPlane (RaycastHit raycastHit)
    {
        // Check if placement is active and there is at least one object to instantiate
        if ( !GameState.Instance.PlacementActive || objectsToInstantiate.Length == 0 ) return;

        // Get Raycast info and randomly choose an object to deploy
        Object objectToInstantiate = objectsToInstantiate[Random.Range (0, objectsToInstantiate.Length)];
        Transform touchedObjectTransform = raycastHit.transform;
        Vector3 touchPosition = raycastHit.point;

        // Check the raycast and object information
        if ( !objectToInstantiate || !touchedObjectTransform || touchedObjectTransform.tag != "Plane" )
            return;

        // Instantiate the object and disable/enable the additional texture for the heatmap, depending on the global setting
        Transform instantiatedObject = ((GameObject)Instantiate (objectToInstantiate, touchPosition,
                                                                 Quaternion.identity, this.transform )).transform;
        if ( instantiatedObject.GetComponent<Renderer> () )
        {
            if ( GameState.Instance.HeatmapActive )
                instantiatedObject.GetComponent<Renderer> ().material.EnableKeyword ("_DETAIL_MULX2");
            else
                instantiatedObject.GetComponent<Renderer> ().material.DisableKeyword ("_DETAIL_MULX2");
        }

        // Rotate towards camera
        if ( !mainCamera ) return;

        float heightDifference = mainCamera.transform.position.y - instantiatedObject.position.y;
        instantiatedObject.LookAt (mainCamera.transform.position - Vector3.up * heightDifference);
    }
}
