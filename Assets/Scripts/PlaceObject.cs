using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceObject : MonoBehaviour
{
    [SerializeField] private Object[] objectsToInstantiate;
    private List<GameObject> preloadedObjects = new List<GameObject> ();
    private Camera mainCamera;
    private bool preloadComplete = false;



    // Start is called before the first frame update
    void Start()
    {
        InputModule.Instance.SubscribeToTouch (PlaceOnPlane);
        mainCamera = Camera.main;

        // quickly load and then destroy all prefabs so the screen does not freeze on first placement
        foreach (Object objectToInstantiate in objectsToInstantiate)
        {
            preloadedObjects.Add ((GameObject)Instantiate (objectToInstantiate, new Vector3 (0f, 0f, 20f), new Quaternion (), this.transform ));
        }
        
    }

    // Update is called once per frame
    void Update()
    {
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

    // Make a new Instance of the object assigned in the Inspector, place it at the given position and rotate it towards the camera
    void PlaceOnPlane (Transform touchedObjectTransform, Vector3 touchPosition)
    {
        // Check the touch information
        if ( !touchedObjectTransform || touchedObjectTransform.tag != "Plane" )
            return;

        // check if there is at least one object to instantiate
        if ( objectsToInstantiate.Length == 0 ) return;

        // instantiate
        Object objectToInstantiate = objectsToInstantiate[Random.Range (0, objectsToInstantiate.Length)];
        Transform instantiatedObject = ((GameObject)Instantiate (objectToInstantiate, touchPosition, new Quaternion (), this.transform )).transform;

        // rotate towards camera
        if ( !mainCamera ) return;

        float heightDifference = mainCamera.transform.position.y - instantiatedObject.position.y;
        instantiatedObject.LookAt (mainCamera.transform.position - Vector3.up * heightDifference);
    }
}
