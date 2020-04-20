using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
    Places Brush Prefabs (specks of color) in the scene for display and in front of the render camera,
    to render them into object textures
*/
public class RenderDrawings : MonoBehaviour
{
    // Components
    private Camera renderCamera;
    [SerializeField]private GameObject brushObject;
    private RenderTexture OriginalRenderTexture;

    // Configuration
    [Tooltip("Scale for brushes rendered on non-planar Objects via RenderTexture")]
    [SerializeField][Range (0.1f, 3f)] float brushScale = 1.0f;
    [Tooltip("Scale for brushes placed on planes in the enviroment")]
    [SerializeField][Range (0.1f, 3f)] float planeBrushScale = 0.2f;
    [Tooltip("Number of taps that have to be near a newly placed one for it to be recolored")]
    [SerializeField]int tapQuantityTreshold = 2;
    [Tooltip("If other taps are within this distance to a newly placed one, they count towards the tapQuantityTreshold")]
    [SerializeField][Range (0.1f, 2f)]float tapDistanceTreshold = 1f;

    // Active Object
    private Renderer activeObjectRenderer = null;
    Transform lastActiveObject = null; // remember the last object painted on

    // Saved Brush Positions
    Dictionary<Transform, HeatmapConfiguration> HeatmapConfigurations = new Dictionary<Transform, HeatmapConfiguration> ();    

    // Pooling mechanism instead of constantly enabling and disabling objects
    Queue<Transform> inactiveBrushPool = new Queue<Transform> ();
    List<Transform> activeBrushes = new List<Transform> ();
    int brushPoolSize = 50;

    // Start is called before the first frame update
    void Start()
    {
        // Get Components and disable camera, so it can render on command with Render()
        renderCamera = this.GetComponent<Camera> ();
        OriginalRenderTexture = renderCamera.targetTexture;
        renderCamera.enabled = false;
        if ( !brushObject )
            brushObject = (GameObject)Resources.Load ("Brush");

        // Register RenderBrushesOnTexture() to get executed every time something is touched
        InputModule.Instance.SubscribeToTouch (RenderBrushesOnTexture);

        // Fill the brush pool with a number of newly instantiated brushes
        ReplenishBrushPool (50);

        // Render one picture to set the RenderTexture content to empty
        renderCamera.Render ();
    }

    // Called after this Camera has taken one image
    private void OnPostRender ()
    {
        if ( !activeObjectRenderer ) return;

        // Disable/enable the additional texture showing the heatmap, depending on the global setting
        if ( GameState.Instance.HeatmapActive )
            activeObjectRenderer.material.EnableKeyword ("_DETAIL_MULX2");
        else
            activeObjectRenderer.material.DisableKeyword ("_DETAIL_MULX2");

        // Assign the modified renderTexture to the touched object
        activeObjectRenderer.material.SetTexture ("_DetailAlbedoMap", HeatmapConfigurations[lastActiveObject].RenderTexture);
    }

    // Place Brushes accoding to input and saved bush positions
    // Then render the brushes to a RenderTexture
    public void RenderBrushesOnTexture (RaycastHit raycastHit)
    {
        Transform hitObjectTransform = raycastHit.transform;
        Vector2 hitTextureUVCoordinates = raycastHit.textureCoord;

        // Check what kind of object was hit
        if ( hitObjectTransform.tag == "Plane" )
        {
            // Simply place a brush Instance on the plane
            Transform placedBrush = RemoveBrushFromPool (raycastHit.point + new Vector3 (0f, 0.1f, 0f), hitObjectTransform.rotation);
            placedBrush.tag = "Drawing";
            placedBrush.localScale = new Vector3 (planeBrushScale, planeBrushScale, planeBrushScale);

            // Hide, if heatmap is not shown
            if ( !GameState.Instance.HeatmapActive )
                placedBrush.GetComponent<MeshRenderer> ().enabled = false;
        }
        else if (hitObjectTransform.tag == "Drawable")
        {
            // Save object renderer to assign renderTexture after rendering
            if ( hitObjectTransform.GetComponent<Renderer> () )
                activeObjectRenderer = hitObjectTransform.GetComponent<Renderer> ();
            else return;

            // Place the brushes according to stored information and new input
            RePlaceBrushes (hitObjectTransform, hitTextureUVCoordinates);

            // Remember the last object painted on
            lastActiveObject = hitObjectTransform;

            // Take a picture :)
            renderCamera.targetTexture = HeatmapConfigurations[hitObjectTransform].RenderTexture;
            renderCamera.Render ();
        }
    }

    // Place the brushes according to stored information and new input
    void RePlaceBrushes (Transform touchedObject, Vector2 uvCoodinates)
    {
        // Check if the object has been painted on before
        if ( HeatmapConfigurations.Count == 0 ) // no painting done at all
        {
            // Add a new HeatmapConfiguration containing the touched UV coordinate as first position
            HeatmapConfigurations.Add (touchedObject, new HeatmapConfiguration (uvCoodinates, OriginalRenderTexture));

            // Instantiate a new brush at the freshly clicked Position
            InstantiateBrushFromPool (uvCoodinates);
        }
        else if ( !HeatmapConfigurations.ContainsKey (touchedObject) ) // Object not yet known
        {
            // tidy up all active brushes for a clean slate
            stowAllBrushesAway ();

            // Add a new HeatmapConfiguration containing the touched UV coordinate as first position
            HeatmapConfigurations.Add (touchedObject, new HeatmapConfiguration (uvCoodinates, OriginalRenderTexture));

            // Instantiate a new brush at the freshly clicked Position
            InstantiateBrushFromPool (uvCoodinates);
        }
        else if ( lastActiveObject && lastActiveObject != touchedObject ) // we know this object
        {
            // clean slate
            stowAllBrushesAway ();

            // Add the newest touch to the known positions
            HeatmapConfigurations[touchedObject].AddBrushPosition (uvCoodinates);
            List<Vector2> restoredBrushPositions = HeatmapConfigurations[touchedObject].BrushPositions;

            // reposition the active brushes to fit the new Configuration or get inactive brushes from the pool
            foreach ( Vector2 storedBrushPosition in restoredBrushPositions )
            {
                InstantiateBrushFromPool (storedBrushPosition);
            }
        }
        else // this is the object we clicked on before
        {
            // Add the newest touch to the known positions and
            // Instantiate a new brush at the freshly clicked Position
            HeatmapConfigurations[touchedObject].AddBrushPosition (uvCoodinates);
            InstantiateBrushFromPool (uvCoodinates);
        }
    }

    // Fill brush pool with a number of newly instantiated brushes
    void ReplenishBrushPool ( int numberOfNewBrushes = 10 )
    {
        for ( int i = 0; i < brushPoolSize; i++ )
        {
            // instantiate at renderCamera position, so they can't be seen
            inactiveBrushPool.Enqueue (((GameObject)Instantiate (brushObject,
                                                                 this.transform.position,
                                                                 brushObject.transform.rotation,
                                                                 this.transform)).transform);
        }
    }

    // Dequeue one brush from the inactive queue into the active List
    // And place it according to the given uv coordinates
    Transform InstantiateBrushFromPool ( Vector2 uvCoordinates )
    {
        // set brush position to clicked uv coordinates (uv = screen: top-left (0,1), top-right (1,1), bottom-left (0,0), bottom-right (1,0))
        Vector3 brushPosition = renderCamera.ViewportToWorldPoint (new Vector3 (uvCoordinates.x, uvCoordinates.y, 0.9f));
        Transform activatedBrush = null;

        // Check Queue buffer
        if ( inactiveBrushPool.Count == 0 )
        {
            // Instantiate a new brush (not that efficient)
            activatedBrush = InstantiateBrushAtCoordinates (uvCoordinates);
        }
        else
        {
            // Get a brush Instance out of the pool and set its position
            activatedBrush = inactiveBrushPool.Dequeue ();
            activatedBrush.position = brushPosition;
        }

        // Mark the Instance active, scale it and recolor it, if the user is clicking on one spot repeatedly
        activeBrushes.Add (activatedBrush);
        determineBrushColor (activatedBrush, activeBrushes);
        activatedBrush.localScale = new Vector3 (brushScale, brushScale, brushScale);

        return activatedBrush;
    }

    // Dequeue one brush from the inactive queue, but don't put it into the active list,
    // permanently place it at the given position instead
    Transform RemoveBrushFromPool ( Vector3 brushPosition, Quaternion brushRotation )
    {
        Transform activatedBrush = null;

        // Check Queue buffer
        if ( inactiveBrushPool.Count == 0 )
        {
            // Instantiate a new brush (not that efficient)
            activatedBrush = InstantiateBrushAtPosition (brushPosition, brushRotation);
        }
        else
        {
            // Get a brush Instance out of the pool and set its position
            activatedBrush = inactiveBrushPool.Dequeue ();
            activatedBrush.position = brushPosition;
            activatedBrush.rotation = brushRotation;
        }

        // Do not mark the Instance active

        // Recolor it, if the user is clicking on one spot repeatedly
        List<Transform> taggedNodes = new List<Transform> ();
        foreach ( GameObject gO in GameObject.FindGameObjectsWithTag ("Drawing") )
            taggedNodes.Add (gO.GetComponent<Transform> ());

        determineBrushColor (activatedBrush, taggedNodes);

        return activatedBrush;
    }

    // Put all active brushes in the inactiveBrushPool Queue and clear the list of active brushes
    void stowAllBrushesAway ()
    {
        foreach (Transform brush in activeBrushes)
        {
            brush.position = this.transform.position;
            inactiveBrushPool.Enqueue (brush);
        }
        activeBrushes.Clear ();
    }

    // According to the proximity to placed brushes, recolor the newly placed brush
    void determineBrushColor (Transform brush, List<Transform> nearbyBrushes)
    {
        BrushColor brushColorScriptReference = brush.GetComponent<BrushColor> ();
        int neighborsInThreshold = 0;

        // count neighbors in threshold
        foreach (Transform activeBrushTransform in nearbyBrushes )
        {
            // Compute vector diff
            Vector3 vectorDifference = (brush.position - activeBrushTransform.position);

            // Determine touch distance by using magnitude (is this subject to camera scale?)
            if ( vectorDifference.magnitude < tapDistanceTreshold )
                neighborsInThreshold++;
        }

        // Set material according to number of neighbors
        if ( neighborsInThreshold >= tapQuantityTreshold * 2 ) brushColorScriptReference.TurnRed ();
        else if (neighborsInThreshold >= tapQuantityTreshold ) brushColorScriptReference.TurnYellow ();
        else brushColorScriptReference.TurnGreen ();
    }

    // Slower than taking a brush from the pool: Instantiate a new copy
    Transform InstantiateBrushAtCoordinates (Vector2 uvCoordinates)
    {
        // set brush position to clicked uv coordinates (uv = screen: top-left (0,1), top-right (1,1), bottom-left (0,0), bottom-right (1,0))
        Vector3 brushPosition = renderCamera.ViewportToWorldPoint (new Vector3 (uvCoordinates.x, uvCoordinates.y, 0.9f));

        // Instantiate a new brush Instance
        return ((GameObject)Instantiate (brushObject, brushPosition, brushObject.transform.rotation, this.transform)).transform;
    }

    // Slower than taking a brush from the pool: Instantiate a new copy
    Transform InstantiateBrushAtPosition ( Vector3 position, Quaternion orientation )
    {
        // Instantiate a new brush Instance
        return ((GameObject)Instantiate (brushObject, position, orientation, this.transform)).transform;
    }
}
