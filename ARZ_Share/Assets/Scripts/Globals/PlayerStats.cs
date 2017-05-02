using HoloToolkit.Unity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class PlayerStats : Singleton<PlayerStats> {

    private bool IS_SERVER;
    private bool HASBEENSET;

    public void SetIsServer(bool argIsserver) {
        if (!HASBEENSET) {
            IS_SERVER = argIsserver;
            HASBEENSET = true;
        }
    }
    public bool GetIsServer() { return IS_SERVER; }

    protected override void Awake()
    {
        base.Awake();
        IS_SERVER = false;
        HASBEENSET = false;
    }

    public bool GetHasBeenSet() {
        return HASBEENSET;
    }
    void Start () {
		
	}

 
}
