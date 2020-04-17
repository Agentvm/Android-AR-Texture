using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushColor : MonoBehaviour
{
    Renderer brushRenderer;
    [SerializeField] Material greenBrushMaterial;
    [SerializeField] Material yellowBrushMaterial;
    [SerializeField] Material redBrushMaterial;

    Color red, green, yellow;

    private void Awake ()
    {
        brushRenderer = this.GetComponent<Renderer> ();
    }

    // Start is called before the first frame update
    void Start()
    {
        brushRenderer = this.GetComponent<Renderer> ();
        
        //// r
        //red.r = 0f;
        //red.g = 180f;
        //red.b = 0f;

        //// g
        //green.r = 180f;
        //green.g = 0f;
        //green.b = 0f;

        //// y
        //yellow.r = 0f;
        //yellow.g = 180f;
        //yellow.b = 180f;
    }

    public void TurnGreen ()
    {
        brushRenderer.material = greenBrushMaterial;
    }

    public void TurnRed ()
    {
        brushRenderer.material = redBrushMaterial;
    }

    public void TurnYellow ()
    {
        brushRenderer.material = yellowBrushMaterial;
    }

    //public void TurnGreen ()
    //{
    //    //brushRenderer.material = greenBrushMaterial;
    //    brushRenderer.material.SetColor ("_Color", green);
    //    brushRenderer.material.SetColor ("_EmissionColor", green);
    //}

    //public void TurnRed ()
    //{
    //    brushRenderer.material.SetColor ("_Color", red);
    //    brushRenderer.material.SetColor ("_EmissionColor", red);
    //}

    //public void TurnYellow ()
    //{
    //    brushRenderer.material.SetColor ("_Color", yellow);
    //    brushRenderer.material.SetColor ("_EmissionColor", yellow);
    //}
}
