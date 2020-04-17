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

    // Start is called before the first frame update
    void Start()
    {
        // Subscribe to user touch events, PlaceOnPlane becomes a callback function
        InputModule.Instance.SubscribeToTouch (PlaceOnPlane);
        mainCamera = Camera.main;

        // Quickly load and then destroy all prefabs so the screen does not freeze on first placement
        List<GameObject> preloadedObjects = new List<GameObject> ();
        foreach (Object objectToInstantiate in objectsToInstantiate)
        {
            preloadedObjects.Add ((GameObject)Instantiate (objectToInstantiate, new Vector3 (0f, 0f, 20f), Quaternion.identity, this.transform ));
        }
        StartCoroutine (DestroyPreloadedObjects (preloadedObjects));
    }

    // Coroutine that waits until End of Frame, then destroys the preloaded Objects
    IEnumerator DestroyPreloadedObjects ( List<GameObject> preloadedObjects )
    {
        yield return new WaitForEndOfFrame ();

        for ( int i = 0; i < preloadedObjects.Count; i++ )
        {
            // destoy the preloaded Instance
            Destroy (preloadedObjects[i]);

            // make sure the Garbage is collected by overwriting the object
            preloadedObjects[i] = new GameObject ();
            Destroy (preloadedObjects[i]);
        }

        preloadedObjects.Clear ();
    }

    // Get the raycast information from the touch event
    // Make a new Instance of one of the objects assigned in the Inspector,
    // place it at the touched position and rotate it towards the camera
    void PlaceOnPlane (RaycastHit raycastHit)
    {
        // Check if placement is active, if a plane was hit and there is at least one object to instantiate
        if ( !GameState.Instance.PlacementActive || objectsToInstantiate.Length == 0 || raycastHit.transform.tag != "Plane" ) return;

        // Get Raycast info and randomly choose an object to deploy
        Object objectToInstantiate = objectsToInstantiate[Random.Range (0, objectsToInstantiate.Length)];
        Transform touchedObjectTransform = raycastHit.transform;
        Vector3 touchPosition = raycastHit.point;

        // Check the raycast and object information
        if ( !objectToInstantiate || !touchedObjectTransform || touchedObjectTransform.tag != "Plane" )
            return;

        // Instantiate the object and start a Coroutine that will disable the bright overlaying texture used for heatmaps
        Transform instantiatedObject = ((GameObject)Instantiate (objectToInstantiate, touchPosition,
                                                                 Quaternion.identity, this.transform )).transform;
        //StartCoroutine (ToggleKeyword (instantiatedObject));

        // Rotate towards camera
        if ( !mainCamera ) return;

        float heightDifference = mainCamera.transform.position.y - instantiatedObject.position.y;
        instantiatedObject.LookAt (mainCamera.transform.position - Vector3.up * heightDifference);
    }

    //IEnumerator ToggleKeyword (Transform objectTransform)
    //{
    //    yield return new WaitForSeconds (2f);

    //    if ( objectTransform.GetComponent<Renderer> () )
    //    {
    //        // Disable/enable the additional texture showing the heatmap, depending on the global setting
    //        if ( GameState.Instance.HeatmapActive )
    //            objectTransform.GetComponent<Renderer> ().material.EnableKeyword ("_DETAIL_MULX2");
    //        else
    //            objectTransform.GetComponent<Renderer> ().material.DisableKeyword ("_DETAIL_MULX2");
    //    }
    //}
}
