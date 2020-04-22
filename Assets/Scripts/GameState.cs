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
    Singleton that allows global information to be safely accessed.
*/
public class GameState : MonoBehaviour
{
    // Static instance of GameState which allows it to be accessed by any other script.
    private static GameState instance = null;

    // Game Variables
    [SerializeField]private bool heatmapActive = false;
    [SerializeField]private bool placementActive = true;
    [SerializeField]private bool placementForbidden = false;
    [SerializeField]int maxPlacedObjects = 4;
    List<Transform> placedObjects = new List<Transform> ();

    // References
    [SerializeField]PlacementButton placementButtonReference;

    // Properties
    public static GameState Instance { get => instance; }
    public bool HeatmapActive { get => heatmapActive; set => heatmapActive = value; }
    public bool PlacementActive { get => placementActive; set => placementActive = value; }
    public bool PlacementForbidden { get => placementForbidden; }
    public List<Transform> PlacedObjects { get => placedObjects; }
    public int MaxPlacedObjects { get => maxPlacedObjects; }

    // Singleton
    void Awake ()
    {
        // check that there is only one instance of this and that it is not destroyed on load
        if ( instance == null )
            instance = this;
        else if ( instance != this )
            Destroy (gameObject);
        DontDestroyOnLoad (gameObject);
    }

    public void registerPlacedObject (Transform objectTransform)
    {
        placedObjects.Add (objectTransform);

        // Disable Placement
        if ( PlacedObjects.Count == MaxPlacedObjects )
        {
            placementForbidden = true;
            if ( placementButtonReference )
                placementButtonReference.Disable ("Maximum Objects");
        }
    }
}
