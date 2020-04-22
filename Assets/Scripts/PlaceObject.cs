/*
    Copyright 2020 Jannik Busse

    Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the
    License. You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an
    "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific
    language governing permissions and limitations under the License.
*/

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
    [SerializeField] private GameObject[] objectsToInstantiate;

    // Start is called before the first frame update
    void Start()
    {
        // Subscribe to user touch events, PlaceOnPlane becomes a callback function
        InputModule.Instance.SubscribeToTouch (PlaceOnPlane);
        mainCamera = Camera.main;

        // Quickly load and then destroy all prefabs so the screen does not freeze on first placement
        List<GameObject> preloadedObjects = new List<GameObject> ();
        foreach (GameObject objectToInstantiate in objectsToInstantiate)
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
        if ( !GameState.Instance.PlacementActive || GameState.Instance.PlacementForbidden ||
             objectsToInstantiate.Length == 0 || raycastHit.transform.tag != "Plane" ) return;

        // Get Raycast info and randomly choose an object to deploy
        GameObject objectToInstantiate = objectsToInstantiate[Random.Range (0, objectsToInstantiate.Length)];
        Transform touchedObjectTransform = raycastHit.transform;
        Vector3 touchPosition = raycastHit.point;

        // Check the raycast and object information
        if ( !objectToInstantiate || !touchedObjectTransform || touchedObjectTransform.tag != "Plane" )
            return;

        // Instantiate the object and register it
        Transform instantiatedObject = ((GameObject)Instantiate (objectToInstantiate, touchPosition,
                                                                 Quaternion.identity, this.transform )).transform;
        GameState.Instance.registerPlacedObject (instantiatedObject);

        // Rotate towards camera
        if ( !mainCamera ) return;

        float heightDifference = mainCamera.transform.position.y - instantiatedObject.position.y;
        instantiatedObject.LookAt (mainCamera.transform.position - Vector3.up * heightDifference);

        // Quickfix for faulty Models
        if ( objectToInstantiate.transform.rotation.x != 0f)
            instantiatedObject.transform.Rotate (-90f, 0, 0);
    }
}
