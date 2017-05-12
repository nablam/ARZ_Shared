using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Myposition : MonoBehaviour {

    public Text camlocpos;
    public Text camWORLDpos;
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        camlocpos.text ="loc="+ this.transform.localPosition.ToString();
        camWORLDpos.text = "worl" + this.transform.position.ToString();
    }
}
