using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderDrawings : MonoBehaviour
{
    private Camera renderCamera;
    private bool renderNow = false;

    // Start is called before the first frame update
    void Start()
    {
        renderCamera = this.GetComponent<Camera> ();
        this.gameObject.SetActive (false);
    }

    private void Update ()
    {
        // If enabled, use one frame to render, then deactivate self
        if ( renderNow )
            renderNow = false;
        else if (this.gameObject.activeSelf)
            this.gameObject.SetActive (false);
    }

    // Start rendering for one frame
    public void RenderBrushOnTexture (RenderTexture renderTexture)
    {
        this.gameObject.SetActive (true);
        //renderCamera.targetTexture = renderTexture;
        renderNow = true;
    }
}
