using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bogus : MonoBehaviour {

    MeshRenderer mr;
	void Start () {
        mr = this.gameObject.GetComponent<MeshRenderer>();
        mr.material.color = Color.red;

        if (PlayerStats.Instance.GetIsServer())
        {
            CONBUG.Instance.LOGit("Sir, you are the SERVER");
            mr.material.color = Color.red;
        }
        else
        {
            CONBUG.Instance.LOGit("bro you're a client");
            mr.material.color = Color.blue;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
