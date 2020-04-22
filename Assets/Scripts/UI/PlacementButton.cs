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
