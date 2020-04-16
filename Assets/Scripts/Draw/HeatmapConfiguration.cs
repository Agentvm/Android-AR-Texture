using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatmapConfiguration// : MonoBehaviour
{
    List<Vector2> brushPositions = new List<Vector2> ();
    RenderTexture renderTexture;

    public HeatmapConfiguration (Vector2 initialBrushPosition, RenderTexture renderTextureToCopy)
    {
        brushPositions.Add (initialBrushPosition);
        renderTexture = new RenderTexture (renderTextureToCopy);
    }

    public List<Vector2> BrushPositions { get => brushPositions; }
    public RenderTexture RenderTexture { get => renderTexture; }

    public void AddBrushPosition (Vector2 brushPosition)
    {
        brushPositions.Add (brushPosition);
    }
}
