using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        activeObjectRenderer.material.mainTexture = renderTexture;
    }

    // Start rendering for one frame
    public void RenderBrushOnTexture (RaycastHit raycastHit)
    {
        // save object renderer to assign renderTexture after rendering
        activeObjectRenderer = raycastHit.transform.GetComponent<Renderer> ();

        // set brush position to clicked uv coordinates (uv = screen: top-left (0,1), top-right (1,1), bottom-left (0,0), bottom-right (1,0))
        Vector3 brushPosition = renderCamera.ViewportToWorldPoint (new Vector3(raycastHit.textureCoord.x, raycastHit.textureCoord.y, 0.9f));

        // Instantiate a new brush Instance
        Instantiate (brushObject, brushPosition, brushObject.transform.rotation, this.transform);

        // Take a picture :)
        renderCamera.Render ();
    }
}

/*
    //Fetch the Renderer from the GameObject
    m_Renderer = GetComponent<Renderer> ();

    //Make sure to enable the Keywords
    m_Renderer.material.EnableKeyword ("_NORMALMAP");
    m_Renderer.material.EnableKeyword ("_METALLICGLOSSMAP");

    //Set the Texture you assign in the Inspector as the main texture (Or Albedo)
    m_Renderer.material.SetTexture("_MainTex", m_MainTexture);
    //Set the Normal map using the Texture you assign in the Inspector
    m_Renderer.material.SetTexture("_BumpMap", m_Normal);
    //Set the Metallic Texture as a Texture you assign in the Inspector
    m_Renderer.material.SetTexture ("_MetallicGlossMap", m_Metal);
*/
