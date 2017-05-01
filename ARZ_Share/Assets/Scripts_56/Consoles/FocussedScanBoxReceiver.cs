using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;

using System;
using HoloToolkit.Sharing;

public class FocussedScanBoxReceiver : MonoBehaviour, IFocusable, IInputClickHandler
{

    public TextMesh tx;
    Color defaultNotConnectedCoplor;
    Color defaultConnectedCoplor;
    bool connectionEstablished;

    public RoomSaver roomsaver;
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnFocusEnter()
    {
        GetComponent<MeshRenderer>().material.color = Color.green;
    }

    public void OnFocusExit()
    {
        if (connectionEstablished)
            GetComponent<MeshRenderer>().material.color = defaultConnectedCoplor;
        else
            GetComponent<MeshRenderer>().material.color = defaultNotConnectedCoplor;
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        roomsaver.SaveRoom();
    }
}
