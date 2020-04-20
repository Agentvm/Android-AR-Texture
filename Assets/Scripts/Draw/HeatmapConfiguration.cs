﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Saves the coloring of a single object in the scene by saving it's brush positions and the
    accociated RenderTexture
*/
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
