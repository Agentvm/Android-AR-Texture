using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushColor : MonoBehaviour
{
    // Components
    Renderer brushRenderer;

    // Color
    [SerializeField] Material greenBrushMaterial;
    [SerializeField] Material yellowBrushMaterial;
    [SerializeField] Material redBrushMaterial;
    Color red, green, yellow;

    // Old Brushes may be removed to improve performance
    float placementTime;

    // Property
    public float PlacementTime { get => placementTime; }

    private void Awake ()
    {
        brushRenderer = this.GetComponent<Renderer> ();
    }

    // Start is called before the first frame update
    void Start()
    {
        brushRenderer = this.GetComponent<Renderer> ();
        placementTime = Time.time;
    }

    public void ResetPlacementTime ()
    {
        placementTime = Time.time;
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
}
