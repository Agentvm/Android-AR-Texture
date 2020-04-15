using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderDrawings : MonoBehaviour
{
    // Components
    private Camera renderCamera;
    private Transform brushTransform;
    private Material unwrappingScreenMaterial;

    // Logic
    private bool renderNow = false;

    // Start is called before the first frame update
    void Start()
    {
        // Get Componentes
        renderCamera = this.GetComponent<Camera> ();
        brushTransform = this.transform.GetChild (0).transform;
        unwrappingScreenMaterial = this.transform.GetChild (1).transform.GetComponent<Renderer> ().material;

        //this.gameObject.SetActive (false);

        // Register RenderBrushOnTexture() to get executed every time something is touched
        InputModule.Instance.SubscribeToTouch (RenderBrushOnTexture);
    }

    private void Update ()
    {
        // If enabled, use one frame to render, then deactivate self
        //if ( renderNow )
        //    renderNow = false;
        //else if (this.gameObject.activeSelf)
        //    this.gameObject.SetActive (false);
    }

    // Start rendering for one frame
    public void RenderBrushOnTexture (RaycastHit raycastHit)
    {


        this.gameObject.SetActive (true);
        //renderCamera.targetTexture = renderTexture;
        renderNow = true;

        // Set the textures
        //unwrappingScreenMaterial = raycastHit.transform.GetComponent<Renderer> ().material;

        // set brush to clicked uv coordinates (uv = screen: top-left (0,1), top-right (1,1), bottom-left (0,0), bottom-right (1,0))
        Debug.Log ("raycastHit.textureCoord: " + raycastHit.textureCoord);
        brushTransform.position = renderCamera.ViewportToWorldPoint (new Vector3(1f - raycastHit.textureCoord.x, 1f - raycastHit.textureCoord.y, 0.9f));
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
