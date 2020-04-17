using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    // Static instance of GameState which allows it to be accessed by any other script.
    private static GameState instance = null;

    // Game Variables
    [SerializeField]private bool heatmapActive = false;
    [SerializeField]private bool placementActive = true;

    // Properties
    public static GameState Instance { get => instance; }
    public bool HeatmapActive { get => heatmapActive; set => heatmapActive = value; }
    public bool PlacementActive { get => placementActive; set => placementActive = value; }

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
}
