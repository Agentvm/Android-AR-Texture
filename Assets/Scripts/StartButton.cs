using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    Button startButton;
    float timeLastClicked;
    float durationDoubleTapThreshold = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        // Execute DestroyOnDoubleClick whenever the Button this script is attached to is clicked
        startButton = this.GetComponent<Button> ();
        startButton.onClick.AddListener (DestroyOnDoubleClick);
        timeLastClicked = Time.time - durationDoubleTapThreshold;
    }

    // Set this gameobject to be inactive when two quick successive clicks are registered
    void DestroyOnDoubleClick ()
    {
        if ( Time.time - timeLastClicked < durationDoubleTapThreshold )
            this.gameObject.SetActive (false);
        timeLastClicked = Time.time;
    }
}
