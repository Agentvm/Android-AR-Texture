using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMovement : MonoBehaviour
{
    // Components
    RectTransform rectTransform;

    // Configuration
    [SerializeField] float panelShowDuration = 3f;
    [SerializeField] float panelReturnSpeed = 0.075f;

    // Panel position
    bool panelActive = true;
    Vector3 activePosition;
    Vector3 disabledPosition;
    Vector3 changedPosition = new Vector3 ();
    float dampingVariable = 0f;

    // Touch
    float showTime = 0f;

    //// Swipe status
    //Vector3 swipeStartPosition;
    //Vector3 swipeCurrentPosition;
    //bool swipeActive = false;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = this.GetComponent<RectTransform> ();

        activePosition = this.transform.position;
        disabledPosition = activePosition - new Vector3 (0f, 360f, 0); // should be height

        //InputModule.Instance.SubscribeToSwipe (OnSwipe);
        //InputModule.Instance.SubscribeToTouch (ShowOnTouch);
    }

    // Update is called once per frame
    void Update()
    {
        // Show panel on Mouse click or Touch
        if ( Input.touchCount > 0 || Input.GetMouseButtonDown (0))
            showTime = Time.time;

        // Show panel for panelShowDuration
        if ( Time.time > showTime + panelShowDuration )
            panelActive = false;
        else
            panelActive = true;

        // Slowly return Panel to active or inactive State
        if ( panelActive && this.transform.position != activePosition )
        {
            changedPosition = this.transform.position;
            changedPosition.y = Mathf.SmoothDamp (this.transform.position.y, activePosition.y, ref dampingVariable,
                          (1/panelReturnSpeed) * Time.deltaTime);
            this.transform.position = changedPosition;
        }
        else if ( !panelActive && this.transform.position != disabledPosition )
        {
            changedPosition = this.transform.position;
            changedPosition.y = Mathf.SmoothDamp (this.transform.position.y, disabledPosition.y, ref dampingVariable,
                          (1/panelReturnSpeed) * Time.deltaTime);
            this.transform.position = changedPosition;
        }

        //if (swipeActive)
        //{
        //    Debug.Log ("start/current: " + swipeStartPosition + " / " + swipeCurrentPosition);

        //    // Swiping up results in a positive verticalSwipeStrenght
        //    float verticalSwipeStrenght = swipeCurrentPosition.y - swipeStartPosition.y;

        //    Debug.Log ("verticalSwipeStrenght: " + verticalSwipeStrenght);

        //    // Move Panel towards top
        //    if (verticalSwipeStrenght > 0 && !panelActive)
        //    {
        //        Debug.Log ("Swiping Up");

        //        Mathf.SmoothDamp (this.transform.position.y, activePosition.y, ref dampingVariable,
        //                      verticalSwipeStrenght * swipeStrenghtMultiplier * Time.deltaTime);

        //        if ( this.transform.position == activePosition )
        //            panelActive = true;
        //    }
        //    else if (verticalSwipeStrenght <= 0 && panelActive) // towards bottom
        //    {
        //        Debug.Log ("Swiping Down");

        //        Mathf.SmoothDamp (this.transform.position.y, disabledPosition.y, ref dampingVariable,
        //                      Mathf.Abs (verticalSwipeStrenght) * swipeStrenghtMultiplier * Time.deltaTime);

        //        if ( this.transform.position == disabledPosition )
        //            panelActive = false;
        //    }                
        //}
        //else // No Swipe detected
        //{
        //    // Slowly return Panel to active or inactive State
        //    if (panelActive && this.transform.position != activePosition )
        //    {
        //        Debug.Log ("Returning Up");

        //        Mathf.SmoothDamp (this.transform.position.y, activePosition.y, ref dampingVariable,
        //                      panelReturnSpeed * Time.deltaTime);
        //    }
        //    else if ( !panelActive && this.transform.position != disabledPosition )
        //    {
        //        Debug.Log ("Returning Down");

        //        Mathf.SmoothDamp (this.transform.position.y, disabledPosition.y, ref dampingVariable,
        //                      panelReturnSpeed * Time.deltaTime);
        //    }
        //}
    }

//    void OnSwipe (Vector3 startPosition, Vector3 currentPosition, bool swipeEnded)
//    {
//        swipeStartPosition = startPosition;
//        swipeCurrentPosition = currentPosition;
//        swipeActive = !swipeEnded;
//    }
}
