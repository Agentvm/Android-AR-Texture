using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class PlacementButton : MonoBehaviour
{
    Button placementButton;
    Text buttonText;
    bool disabled = false;

    // Start is called before the first frame update
    void Start ()
    {
        // Get Components and link function to button
        placementButton = this.GetComponent<Button> ();
        buttonText = placementButton.transform.GetChild (0).GetComponent<Text> ();
        placementButton.onClick.AddListener (TogglePlacement);

        // Check game state
        if (GameState.Instance.PlacementActive)
        {
            buttonText.text = "Disable Placement";
            placementButton.image.color = Color.grey;
        }
        else
        {
            buttonText.text = "Enable Placement";
            placementButton.image.color = Color.white;
        }
    }

    public void TogglePlacement ()
    {
        if ( disabled ) return;

        if ( GameState.Instance.PlacementActive )
        {
            // Disable Heatmap
            GameState.Instance.PlacementActive = false;
            buttonText.text = "Enable Placement";
            placementButton.image.color = Color.white;
        }
        else
        {
            // Activate placement
            GameState.Instance.PlacementActive = true;
            buttonText.text = "Disable Placement";
            placementButton.image.color = Color.grey;
        }
    }

    public void Disable (string disabledText = "Placement Disabled")
    {
        // Disable Button
        GameState.Instance.PlacementActive = false;
        buttonText.text = disabledText;
        placementButton.image.color = Color.grey;
        disabled = true;
    }
}
