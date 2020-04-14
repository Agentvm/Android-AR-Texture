using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw : MonoBehaviour
{
    [SerializeField] private Camera brushRenderCamera;
    RenderDrawings renderDrawingsScriptReference;

    // Start is called before the first frame update
    void Start ()
    {
        renderDrawingsScriptReference = brushRenderCamera.GetComponent<RenderDrawings> ();

        InputModule.Instance.SubscribeToTouch (drawOnClick);
    }

    // Update is called once per frame
    void Update ()
    {

    }

    public void drawOnClick ( RaycastHit raycastHit )
    {
        // check if hit object can be drawn on
        if ( raycastHit.transform.tag != "Drawable" && raycastHit.transform.tag != "Plane" ) return;

        // make the render camera face the object
        //brushRenderCamera.transform.rotation = Quaternion.Euler (-raycastHit.normal);
        brushRenderCamera.transform.position = raycastHit.point + raycastHit.normal.normalized;
        brushRenderCamera.transform.LookAt (raycastHit.point);

        // draw on texture
        renderDrawingsScriptReference.RenderBrushOnTexture ((RenderTexture)raycastHit.transform.GetComponent<Renderer> ().material.mainTexture);
    }
}
