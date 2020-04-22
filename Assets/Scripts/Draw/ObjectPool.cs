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
    Creates a pool of Instances from a given prefab and re-places them at runtime
*/
public class ObjectPool : MonoBehaviour
{
    // The object that the pool consists of
    [SerializeField]private GameObject pooledObject;

    // Pooling mechanism instead of constantly enabling and disabling objects
    Queue<Transform> inactiveElementsPool = new Queue<Transform> ();
    List<Transform> activeElements = new List<Transform> ();
    int initialPoolSize = 50;

    // Properties
    public int TotalElementsCount { get => InactiveElementsCount + ActiveElementsCount; }
    public int InactiveElementsCount { get => inactiveElementsPool.Count; }
    public int ActiveElementsCount { get => activeElements.Count; }
    public List<Transform> ActiveElements { get => activeElements; }
    public Queue<Transform> InactiveElementsPool { get => inactiveElementsPool; }

    private void Start ()
    {
        if ( !pooledObject )
            pooledObject = (GameObject)Resources.Load ("Brush");

        // Initially fill pool
        ReplenishPool (initialPoolSize);
    }

    // Fill pool with a number of newly instantiated objects
    void ReplenishPool ( int numberOfNewElements = 10 )
    {
        for ( int i = 0; i < numberOfNewElements; i++ )
        {
            // instantiate at renderCamera position, so they can't be seen
            inactiveElementsPool.Enqueue (((GameObject)Instantiate (pooledObject,
                                                                 this.transform.position,
                                                                 pooledObject.transform.rotation,
                                                                 this.transform)).transform);
        }
    }

    // Dequeue one element from the inactive queue into the active List
    // And place it according to the given uv coordinates
    public Transform InstantiateElementFromPool ( Vector3 position )
    {
        Transform activatedElement = null;

        // Check Queue buffer
        if ( inactiveElementsPool.Count == 0 )
        {
            // Instantiate a new element and refill the pool (not that efficient)
            activatedElement = InstantiateElementAtPosition (position);
            ReplenishPool ();
        }
        else
        {
            // Get an element Instance out of the pool and set its position
            activatedElement = inactiveElementsPool.Dequeue ();
            activatedElement.position = position;
        }

        // Mark the Instance active, scale it and recolor it, if the user is clicking on one spot repeatedly
        activeElements.Add (activatedElement);

        return activatedElement;
    }

    // Put all active elements in the inactiveElementPool Queue and clear the list of active elements
    public void poolAllElements ()
    {
        foreach ( Transform element in activeElements )
        {
            element.position = this.transform.position;
            inactiveElementsPool.Enqueue (element);
        }
        activeElements.Clear ();
    }

    // Dequeue one element from the inactive queue, but don't put it into the active list,
    // permanently place it at the given position instead
    public Transform RemoveElementFromPool ( Vector3 position, Quaternion rotation )
    {
        Transform activatedElement = null;

        // Check Queue buffer
        if ( InactiveElementsCount == 0 )
        {
            // Instantiate a new element (not that efficient)
            activatedElement = InstantiateElementAtPosition (position, rotation);
        }
        else
        {
            // Get an object Instance out of the pool and set its position
            activatedElement = inactiveElementsPool.Dequeue ();
            activatedElement.position = position;
            activatedElement.rotation = rotation;
        }

        // Do not mark the Instance active

        return activatedElement;
    }

    // Slower than taking an element from the pool: Instantiate a new copy
    Transform InstantiateElementAtPosition ( Vector3 position, Quaternion orientation )
    {
        // Instantiate a new object Instance
        return ((GameObject)Instantiate (pooledObject, position, orientation, this.transform)).transform;
    }

    // Slower than taking an element from the pool: Instantiate a new copy
    Transform InstantiateElementAtPosition ( Vector3 position )
    {
        // Instantiate a new object Instance
        return ((GameObject)Instantiate (pooledObject, position, pooledObject.transform.rotation, this.transform)).transform;
    }
}
