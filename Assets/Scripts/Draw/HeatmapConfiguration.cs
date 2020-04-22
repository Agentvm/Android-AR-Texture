/*
    Copyright 2020 Jannik Busse

    Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the
    License. You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an
    "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific
    language governing permissions and limitations under the License.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Saves the coloring of a single object in the scene by saving it's brush positions and the
    accociated RenderTexture
*/
public class HeatmapConfiguration// : MonoBehaviour
{
    List<Vector3> brushPositions = new List<Vector3> ();
    RenderTexture renderTexture;

    public HeatmapConfiguration (Vector3 initialBrushPosition, RenderTexture renderTextureToCopy)
    {
        brushPositions.Add (initialBrushPosition);
        renderTexture = new RenderTexture (renderTextureToCopy);
    }

    public List<Vector3> BrushPositions { get => brushPositions; }
    public RenderTexture RenderTexture { get => renderTexture; }

    public void AddBrushPosition (Vector3 brushPosition)
    {
        brushPositions.Add (brushPosition);
    }
}
