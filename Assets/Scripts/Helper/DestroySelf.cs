using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    float startTime;
    [Range (0f, 5f)][SerializeField] float lifeSpan = 2f;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if ( Time.time > startTime + lifeSpan )
            Destroy (this.gameObject );
    }
}
