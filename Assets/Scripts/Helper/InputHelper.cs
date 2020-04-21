using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputHelper : MonoBehaviour
{

    private static TouchCreator lastFakeTouch;
    //private static TouchCreator mirrorFakeTouch;

    public static List<Touch> GetTouches ()
    {
        List<Touch> touches = new List<Touch>();
        touches.AddRange (Input.touches);
#if UNITY_EDITOR
        if ( lastFakeTouch == null )
        {
            lastFakeTouch = new TouchCreator ();
            //mirrorFakeTouch = new TouchCreator ();
        }
        if ( Input.GetMouseButtonDown (0) )
        {
            lastFakeTouch.phase = TouchPhase.Began;
            lastFakeTouch.deltaPosition = new Vector2 (0, 0);
            lastFakeTouch.position = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
            lastFakeTouch.fingerId = 0;

            //mirrorFakeTouch.phase = TouchPhase.Began;
            //mirrorFakeTouch.deltaPosition = new Vector2 (0, 0);
            //mirrorFakeTouch.position = new Vector2 (Input.mousePosition.x, Input.mousePosition.y) + Vector2.right * 10f;
            //mirrorFakeTouch.fingerId = 13;
        }
        else if ( Input.GetMouseButtonUp (0) )
        {
            lastFakeTouch.phase = TouchPhase.Ended;
            Vector2 newPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            lastFakeTouch.deltaPosition = newPosition - lastFakeTouch.position;
            lastFakeTouch.position = newPosition;
            lastFakeTouch.fingerId = 0;

            //mirrorFakeTouch.phase = TouchPhase.Ended;
            //newPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y) + Vector2.right * 10f;
            //mirrorFakeTouch.deltaPosition = newPosition - mirrorFakeTouch.position;
            //mirrorFakeTouch.position = newPosition;
            //mirrorFakeTouch.fingerId = 13;
        }
        else if ( Input.GetMouseButton (0) )
        {
            lastFakeTouch.phase = TouchPhase.Moved;
            Vector2 newPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            lastFakeTouch.deltaPosition = newPosition - lastFakeTouch.position;
            lastFakeTouch.position = newPosition;
            lastFakeTouch.fingerId = 0;

            //mirrorFakeTouch.phase = TouchPhase.Moved;
            //newPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y) + Vector2.right * 10f;
            //mirrorFakeTouch.deltaPosition = newPosition - mirrorFakeTouch.position;
            //mirrorFakeTouch.position = newPosition;
            //mirrorFakeTouch.fingerId = 13;
        }
        else
        {
            lastFakeTouch = null;
            //mirrorFakeTouch = null;
        }
        if ( lastFakeTouch != null )
        {
            touches.Add (lastFakeTouch.Create ());
            //touches.Add (mirrorFakeTouch.Create ());
        }
#endif


        return touches;
    }

}