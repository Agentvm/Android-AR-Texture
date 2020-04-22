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
using UnityEngine.UI;


public class HeatmapButton : MonoBehaviour
{
    Button heatmapButton;
    Text buttonText;

    // Start is called before the first frame update
    void Start ()
    {
        // Get Components and link function to button
        heatmapButton = this.GetComponent<Button> ();
        buttonText = heatmapButton.transform.GetChild (0).GetComponent<Text> ();
        heatmapButton.onClick.AddListener (ToggleHeatmap);

        // Check heatmap status
        if ( GameState.Instance.HeatmapActive )
        {
            // Set the button color to is-enabled and show the heatmap
            ShowHeatmap ();
            heatmapButton.image.color = Color.grey;
        }
        else
        {
            // Set the button color to can-be-enabled and hide the heatmap
            HideHeatmap ();
            heatmapButton.image.color = Color.white;
        }
    }

    public void ToggleHeatmap ()
    {
        if ( GameState.Instance.HeatmapActive )
        {
            // Disable Hatmap
            GameState.Instance.HeatmapActive = false;
            buttonText.text = "Show Heatmap";
            heatmapButton.image.color = Color.white;
            HideHeatmap ();
        }
        else
        {
            // Activate Heatmap
            GameState.Instance.HeatmapActive = true;
            buttonText.text = "Hide Heatmap";
            heatmapButton.image.color = Color.grey;
            ShowHeatmap ();
        }
    }

    // Show Heatmaps by enabling the Albedo Detail Texture on all materials of GameObjects tagged "Drawable" or "Plane"
    void ShowHeatmap ()
    {
        // Find all GameObjects tagged Drawable (Objects with RenderTexture)
        foreach ( GameObject gObject in GameObject.FindGameObjectsWithTag ("Drawable") )
            if ( gObject.GetComponent<Renderer> () )
                gObject.GetComponent<Renderer> ().material.EnableKeyword ("_DETAIL_MULX2");

        // Find all GameObjects tagged Drawing (Brushes placed in Scene)
        foreach ( GameObject gObject in GameObject.FindGameObjectsWithTag ("Drawing") )
            gObject.GetComponent<MeshRenderer> ().enabled = true;
    }

    // Hide Heatmaps by disabling the Albedo Detail Texture on all materials of GameObjects tagged "Drawable" or "Plane"
    void HideHeatmap ()
    {
        // Find all GameObjects tagged Drawable (Objects with RenderTexture)
        foreach ( GameObject gObject in GameObject.FindGameObjectsWithTag ("Drawable") )
            if ( gObject.GetComponent<Renderer> () )
                gObject.GetComponent<Renderer> ().material.DisableKeyword ("_DETAIL_MULX2");

        // Find all GameObjects tagged Drawing (Brushes placed in Scene)
        foreach ( GameObject gObject in GameObject.FindGameObjectsWithTag ("Drawing") )
            gObject.GetComponent<MeshRenderer> ().enabled = false;
    }
}
