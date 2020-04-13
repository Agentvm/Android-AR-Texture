using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceObject : MonoBehaviour
{
    [SerializeField] private Object[] objectsToInstantiate;
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        InputModule.Instance.SubscribeFunctionToPlaneTouch (Place);
        mainCamera = Camera.main;

        // quickly load and then destroy the prefab so the screen dows not freeze on first placement
        foreach (Object objectInList in objectsToInstantiate)
        {
            GameObject preloadObject = (GameObject)Instantiate (objectInList, new Vector3 (0f, -20f, -20f), new Quaternion (), this.transform );
            Destroy (preloadObject);
            preloadObject = new GameObject (); // make sure the Garbage is collected
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Make a new Instance of the object assigned in the Inspector, place it at the given position and rotate it towards the camera
    void Place (Vector3 position)
    {
        // instantiate
        if ( objectsToInstantiate.Length == 0 ) return;
        Object objectToInstantiate = objectsToInstantiate[Random.Range (0, objectsToInstantiate.Length)];

        Transform instantiatedObject = ((GameObject)Instantiate (objectToInstantiate, position, new Quaternion (), this.transform )).transform;

        // rotate towards camera
        if ( !mainCamera ) return;

        float heightDifference = mainCamera.transform.position.y - instantiatedObject.position.y;
        instantiatedObject.LookAt (mainCamera.transform.position - Vector3.up * heightDifference);
    }
}
