using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImportOrExport : MonoBehaviour {

    bool _amserver;
	// Use this for initialization
	void Start () {
        if (PlayerStats.Instance.GetIsServer())
        {
            _amserver = true;
        }
        else
        {
            _amserver = false;
        }
    }
    void determinIfServer()
    {
        if (PlayerStats.Instance.GetIsServer())
        {
            _amserver = true;
        }
        else
        {
            _amserver = false;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
