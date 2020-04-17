using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RenderDrawings : MonoBehaviour
{
    // Components
    private Camera renderCamera;
    [SerializeField]private GameObject brushObject;
    private RenderTexture OriginalRenderTexture;

    // Active Object
    private Renderer activeObjectRenderer = null;
    Transform lastActiveObject = null; // remember the last object painted on

    // Saved Brush Positions
    Dictionary<Transform, HeatmapConfiguration> HeatmapConfigurations = new Dictionary<Transform,HeatmapConfiguration> ();

    // Brush Color
    int tapQuantityTreshold = 2;
    float tapDistanceTreshold = 1f;

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

        // Register RenderBrushOnTexture() to get executed every time something is touched
        InputModule.Instance.SubscribeToTouch (RenderBrushOnTexture);

        // Fill brush pool with a number of newly instantiated brushes
        ReplenishBrushPool (50);
    }

    // Called after this Camera has taken one image
    private void OnPostRender ()
    {
        // Disable/enable the additional texture showing the heatmap, depending on the global setting
        if ( GameState.Instance.HeatmapActive )
            activeObjectRenderer.material.EnableKeyword ("_DETAIL_MULX2");
        else
            activeObjectRenderer.material.DisableKeyword ("_DETAIL_MULX2");

        // Assign the modified renderTexture to the touched object
        activeObjectRenderer.material.SetTexture ("_DetailAlbedoMap", HeatmapConfigurations[lastActiveObject].RenderTexture);
    }

    // Place Brushes accoding to input and saved bush positions
    public void RenderBrushOnTexture (RaycastHit raycastHit)
    {
        Transform hitObjectTransform = raycastHit.transform;
        Vector2 hitTextureUVCoordinates = raycastHit.textureCoord;

        // save object renderer to assign renderTexture after rendering
        if ( hitObjectTransform.GetComponent<Renderer> () )
            activeObjectRenderer = hitObjectTransform.GetComponent<Renderer> ();
        else return;

        // check if the object has been painted on before
        // and place the brushes accordingly
        if ( HeatmapConfigurations.Count == 0 ) // no painting done at all
        {
            // Add a new HeatmapConfiguration containing the touched UV coordinate as first position
            HeatmapConfigurations.Add (hitObjectTransform, new HeatmapConfiguration (hitTextureUVCoordinates, OriginalRenderTexture ));
            
            // Instantiate a new brush at the freshly clicked Position
            InstantiateBrushFromPool (hitTextureUVCoordinates);
        }
        else if ( !HeatmapConfigurations.ContainsKey (hitObjectTransform) ) // Object not yet known
        {
            // tidy up all active brushes for a clean slate
            stowAllBrushesAway ();

            // Add a new HeatmapConfiguration containing the touched UV coordinate as first position
            HeatmapConfigurations.Add (hitObjectTransform, new HeatmapConfiguration (hitTextureUVCoordinates, OriginalRenderTexture));

            // Instantiate a new brush at the freshly clicked Position
            InstantiateBrushFromPool (hitTextureUVCoordinates);
        }
        else if ( lastActiveObject && lastActiveObject != hitObjectTransform ) // we know this object
        {
            // clean slate
            stowAllBrushesAway ();

            // Add the newest touch to the known positions
            HeatmapConfigurations[hitObjectTransform].AddBrushPosition (hitTextureUVCoordinates);
            List<Vector2> restoredBrushPositions = HeatmapConfigurations[hitObjectTransform].BrushPositions;

            // reposition the active brushes to fit the new Configuration or get inactive brushes from the pool
            foreach (Vector2 storedBrushPosition in restoredBrushPositions )
            {
                InstantiateBrushFromPool (storedBrushPosition);
            }
        }
        else // this is the object we clicked on before
        {
            // Add the newest touch to the known positions and
            // Instantiate a new brush at the freshly clicked Position
            HeatmapConfigurations[hitObjectTransform].AddBrushPosition (hitTextureUVCoordinates);
            InstantiateBrushFromPool (hitTextureUVCoordinates);
        }

        // Remember the last object painted on
        lastActiveObject = hitObjectTransform;

        // Take a picture :)
        renderCamera.targetTexture = HeatmapConfigurations[hitObjectTransform].RenderTexture;
        renderCamera.Render ();
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
    void InstantiateBrushFromPool ( Vector2 uvCoordinates )
    {
        // set brush position to clicked uv coordinates (uv = screen: top-left (0,1), top-right (1,1), bottom-left (0,0), bottom-right (1,0))
        Vector3 brushPosition = renderCamera.ViewportToWorldPoint (new Vector3 (uvCoordinates.x, uvCoordinates.y, 0.9f));
        Transform activatedBrush = null;

        // Check Queue buffer
        if ( inactiveBrushPool.Count == 0 )
        {
            // Instantiate a new brush (not that efficient)
            activatedBrush = InstantiateBrushAtPosition (uvCoordinates);
        }
        else
        {
            // Get a brush Instance out of the pool and set its position
            activatedBrush = inactiveBrushPool.Dequeue ();
            activatedBrush.position = brushPosition;
        }

        // Mark the Instance active and recolor it, if the user is clicking on one spot repeatedly
        activeBrushes.Add (activatedBrush);
        determineBrushColor (activatedBrush);
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
    void determineBrushColor (Transform brush)
    {
        BrushColor brushColorScriptReference = brush.GetComponent<BrushColor> ();
        int neighborsInThreshold = 0;

        // count neighbors in threshold
        foreach (Transform activeBrushTransform in activeBrushes)
        {
            // compute vector diff
            Vector2 vectorDifference = new Vector2 ();
            vectorDifference.x = (brush.position.x - activeBrushTransform.position.x);
            vectorDifference.y = (brush.position.z - activeBrushTransform.position.z);

            // Determine touch distance by using magnitude (is this subject to camera scale? How do I scale this?)
            if ( vectorDifference.magnitude < tapDistanceTreshold )
                neighborsInThreshold++;
        }

        // Set material according to number of neighbors
        if ( neighborsInThreshold >= tapQuantityTreshold * 2 ) brushColorScriptReference.TurnRed ();
        else if (neighborsInThreshold >= tapQuantityTreshold ) brushColorScriptReference.TurnYellow ();
        else brushColorScriptReference.TurnGreen ();
    }


    // Slower than taking a brush from the pool: Instantiate a new copy
    Transform InstantiateBrushAtPosition (Vector2 uvCoordinates)
    {
        // set brush position to clicked uv coordinates (uv = screen: top-left (0,1), top-right (1,1), bottom-left (0,0), bottom-right (1,0))
        Vector3 brushPosition = renderCamera.ViewportToWorldPoint (new Vector3 (uvCoordinates.x, uvCoordinates.y, 0.9f));

        // Instantiate a new brush Instance
        return ((GameObject)Instantiate (brushObject, brushPosition, brushObject.transform.rotation, this.transform)).transform;
    }
}
