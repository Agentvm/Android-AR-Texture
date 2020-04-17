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
        // Find all GameObjects tagged Drawable
        foreach ( GameObject gObject in GameObject.FindGameObjectsWithTag ("Drawable") )
            if ( gObject.GetComponent<Renderer> () )
                gObject.GetComponent<Renderer> ().material.EnableKeyword ("_DETAIL_MULX2");

        // Find all GameObjects tagged Plane
        foreach ( GameObject gObject in GameObject.FindGameObjectsWithTag ("Plane") )
            if ( gObject.GetComponent<Renderer> () )
                gObject.GetComponent<Renderer> ().material.EnableKeyword ("_DETAIL_MULX2");
    }

    // Hide Heatmaps by disabling the Albedo Detail Texture on all materials of GameObjects tagged "Drawable" or "Plane"
    void HideHeatmap ()
    {
        // Find all GameObjects tagged Drawable
        foreach ( GameObject gObject in GameObject.FindGameObjectsWithTag ("Drawable") )
            if ( gObject.GetComponent<Renderer> () )
                gObject.GetComponent<Renderer> ().material.DisableKeyword ("_DETAIL_MULX2");

        // Find all GameObjects tagged Plane
        foreach ( GameObject gObject in GameObject.FindGameObjectsWithTag ("Plane") )
            if ( gObject.GetComponent<Renderer> () )
                gObject.GetComponent<Renderer> ().material.DisableKeyword ("_DETAIL_MULX2");
    }
}
