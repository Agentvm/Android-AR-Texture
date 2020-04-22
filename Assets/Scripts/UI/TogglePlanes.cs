using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class TogglePlanes : MonoBehaviour
{
    // Components
    ARPlaneManager m_ARPlaneManager;
    

    // Start is called before the first frame update
    void Start()
    {
        // Get Components
        m_ARPlaneManager = GetComponent<ARPlaneManager> ();
    }

    /// <summary>
    /// Iterates over all the existing planes and activates
    /// or deactivates their <c>GameObject</c>s'.
    /// </summary>
    /// <param name="value">Each planes' GameObject is SetActive with this value.</param>
    public void SetAllPlanesActive ( bool value )
    {
        foreach ( var plane in m_ARPlaneManager.trackables )
            plane.gameObject.SetActive (value);
    }
}
