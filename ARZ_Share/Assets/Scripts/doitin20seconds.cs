﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doitin20seconds : MonoBehaviour {

    public GameObject thisdgy;
	void Start () {
		
	}


    bool builtonce = false;
    float twentycesonds = 10f;
	void Update () {
        twentycesonds -= Time.deltaTime;
        if (twentycesonds<0) {
            buildonce();
        }	
	}

    void buildonce() {
        if (!builtonce) {
            // Instantiate(thisdgy);
            thisdgy.AddComponent<bogus>();
             builtonce = true;
        }
    }
}
