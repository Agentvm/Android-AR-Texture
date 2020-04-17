using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class PlacementButton : MonoBehaviour
{
    Button placementButton;
    Text buttonText;

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
        if ( GameState.Instance.PlacementActive )
        {
            // Disable Hatmap
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
}
