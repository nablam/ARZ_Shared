using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cratepos : MonoBehaviour {

    public TextMesh tmlocal;

    public TextMesh tmworld;
    // Use this for initialization
    void Start () {
        tmlocal.text = this.transform.localPosition.ToString();
        tmworld.text = this.transform.position.ToString();

    }
	
	// Update is called once per frame
	void Update () {
        tmlocal.text = this.transform.localPosition.ToString();
        tmworld.text = this.transform.position.ToString();
    }
}
