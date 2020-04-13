using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceObject : MonoBehaviour
{
    [SerializeField] private Object objectToInstantiate;

    // Start is called before the first frame update
    void Start()
    {
        InputModule.Instance.functionsOnPlaneTouch.Add (Place);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Place (Vector3 position)
    {
        if ( !objectToInstantiate ) return;

        Instantiate (objectToInstantiate, position, new Quaternion (), this.transform );
    }
}
