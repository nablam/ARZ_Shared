using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawneee : MonoBehaviour {
    
    float movespeed = 0.5f;
    string _spawneeeID;
    public TextMesh tm;
    private void Awake()
    {
        _spawneeeID = "default";
        tm.text = _spawneeeID;
    }
    void Start () {
 
    }
	
	// Update is called once per frame
	void Update () {
        transform.Translate(Vector3.up*Time.deltaTime*movespeed);
	}

    public void UpdateLBLID(string argLBLID) {
        tm.text = "wewewfwfwfe";
        tm.text = argLBLID;
    }
}
