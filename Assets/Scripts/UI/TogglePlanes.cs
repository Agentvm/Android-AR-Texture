﻿/*
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