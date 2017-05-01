using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VR.WSA.Persistence;
using HoloToolkit.Unity;
using System;

public class myObjectAnchorManager : MonoBehaviour {

    List<GameObject> List_sceneobjects;
    WorldAnchorManager WAM;
	void Start () {
        WAM = WorldAnchorManager.Instance;
        List_sceneobjects = GameObject.FindGameObjectsWithTag("SharedObject").ToList();
        AddAnAnchorToall();
    }

    void AddAnAnchorToall() {
        foreach (GameObject go in List_sceneobjects)
        {
            string miniguid = go.name + "_" + Guid.NewGuid().ToString().Substring(0, 4);
            Debug.Log("adding anchor to " + go.name);
            WAM.AttachAnchor(go, miniguid);
        }
    }

	void Update () {
		
	}
}
