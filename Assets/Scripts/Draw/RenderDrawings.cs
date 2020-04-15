using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RenderDrawings : MonoBehaviour
{
    // Components
    private Camera renderCamera;
    [SerializeField]private GameObject brushObject;
    //private MeshRenderer unwrappingScreenRenderer;
    private RenderTexture renderTexture;

    //
    private Renderer activeObjectRenderer = null;

    // Logic
    private bool renderNow = false;

    // Start is called before the first frame update
    void Start()
    {
        // Get Components and disable camera, so it can render on command with Render()
        renderCamera = this.GetComponent<Camera> ();
        renderTexture = renderCamera.targetTexture;
        renderCamera.enabled = false;
        if ( !brushObject )
            brushObject = (GameObject)Resources.Load ("Brush");

        // Register RenderBrushOnTexture() to get executed every time something is touched
        InputModule.Instance.SubscribeToTouch (RenderBrushOnTexture);
    }

    // Called after this Camera has taken one image
    private void OnPostRender ()
    {
        // Assign the modified renderTexture to the touched object
        activeObjectRenderer.material.EnableKeyword ("_DETAIL_MULX2");
        activeObjectRenderer.material.SetTexture ("_DetailAlbedoMap", renderTexture);

        //activeObjectRenderer.material.mainTexture = renderTexture;
        //activeObjectRenderer.material.SetTexture ("_Diffuse", renderTexture);

    }

    // Start rendering for one frame
    public void RenderBrushOnTexture (RaycastHit raycastHit)
    {
        Debug.Log ("raycastHit.transform.name: " + raycastHit.transform.name);

        // save object renderer to assign renderTexture after rendering
        if ( raycastHit.transform.GetComponent<Renderer> () )
            activeObjectRenderer = raycastHit.transform.GetComponent<Renderer> ();
        //if ( raycastHit.transform.GetChild (0).GetComponent<Renderer> () )
        //    activeObjectRenderer = raycastHit.transform.GetChild (0).GetComponent<Renderer> ();
        else return;

        // set brush position to clicked uv coordinates (uv = screen: top-left (0,1), top-right (1,1), bottom-left (0,0), bottom-right (1,0))
        Vector3 brushPosition = renderCamera.ViewportToWorldPoint (new Vector3(raycastHit.textureCoord.x, raycastHit.textureCoord.y, 0.9f));

        // Instantiate a new brush Instance
        Instantiate (brushObject, brushPosition, brushObject.transform.rotation, this.transform);

        // Take a picture :)
        renderCamera.Render ();
    }
}

/*
    //List<Texture> allTexture = new List<Texture>();
    Shader shader = activeObjectRenderer.material.shader;
    for ( int i = 0; i < ShaderUtil.GetPropertyCount (shader); i++ )
    {
        if ( ShaderUtil.GetPropertyType (shader, i) == ShaderUtil.ShaderPropertyType.TexEnv )
        {
            Debug.Log ("ShaderUtil.GetPropertyName(shader, i): " + ShaderUtil.GetPropertyName (shader, i));
            //Texture texture = activeObjectRenderer.material.GetTexture(ShaderUtil.GetPropertyName(shader, i));
            //allTexture.Add (texture);
        }
    }
*/
