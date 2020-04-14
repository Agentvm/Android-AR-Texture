using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw : MonoBehaviour
{
    [SerializeField] private Camera brushRenderCamera;

    // Start is called before the first frame update
    void Start ()
    {
        InputModule.Instance.SubscribeToTouch (drawOnClick);
    }

    // Update is called once per frame
    void Update ()
    {

    }

    public void drawOnClick ( RaycastHit raycastHit )
    {
        // check if hit object can be drawn on
        if ( raycastHit.transform.tag != "Drawable" && raycastHit.transform.tag != "Plane" )

        // make the render camera face the object
        //brushRenderCamera.transform.rotation = Quaternion.Euler (-raycastHit.normal);
        brushRenderCamera.transform.position = raycastHit.normal.normalized * 1.5f - raycastHit.point;
        brushRenderCamera.transform.LookAt (raycastHit.point);
    }
}
