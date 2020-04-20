﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveInEditor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive (false);
        #if UNITY_EDITOR
            Debug.Log ("Unity Editor");
            this.gameObject.SetActive (true);
        #endif
    }
}