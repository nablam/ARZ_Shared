using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;

using System;
using HoloToolkit.Sharing;

public class FocussedRoomBoxReceiver : MonoBehaviour, IFocusable, IInputClickHandler
{

    public TextMesh tx;
    Color defaultNotConnectedCoplor;
    Color defaultConnectedCoplor;
    bool connectionEstablished;
   
    protected RoomManager roomManager;
    protected Room currentRoom;
    protected RoomManagerAdapter roomManagerListener;

    long localUserId;
    string localUsername;

    void Start()
    {
        localUsername = "noname";
        localUserId = 0;
        GetComponent<MeshRenderer>().material.color = defaultNotConnectedCoplor;
        connectionEstablished = false;
        defaultNotConnectedCoplor = Color.gray;
        defaultConnectedCoplor = Color.cyan;
    }

    public void OnFocusEnter()
    {
        GetComponent<MeshRenderer>().material.color = Color.green;
        if (!connectionEstablished)
            GetRoomInfo();
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
       
        if (!connectionEstablished)
            tx.text += "\n lol";
        else
        {
            tx.text = "";
            using (User localUser = SharingStage.Instance.Manager.GetLocalUser())
            {
                localUserId = localUser.GetID();
                localUsername = localUser.GetName().ToString();
            }

            tx.text += "\n room name="+currentRoom.GetName().ToString();
            tx.text += "\n room id=  " + currentRoom.GetID().ToString();
            tx.text += "\n user id=  " + localUserId.ToString();
            tx.text += " name=" + localUserId.ToString();

            int acnt = currentRoom.GetAnchorCount();
            tx.text += "\n anchors in room =" + currentRoom.GetAnchorCount().ToString();
            //  localUser.GetName().ToString();
            tx.text += "\n";
            for (int x= 0; x< acnt; x++) {
                tx.text += "\n " + currentRoom.GetAnchorName(x);
            }




        }
    }

    private  bool IsthisUseraServer()
    {
            
            using (User localUser = SharingStage.Instance.Manager.GetLocalUser())
            {
                localUserId = localUser.GetID();
          
            }

            for (int i = 0; i < SharingStage.Instance.SessionUsersTracker.CurrentUsers.Count; i++)
            {
                if (SharingStage.Instance.SessionUsersTracker.CurrentUsers[i].GetID() < localUserId)
                {
                    return false;
                }
            }

            return true;
        
    }
    void GetRoomInfo()
    {
        if (SharingStage.Instance.IsConnected)
        {
            connectionEstablished = true;
            GetComponent<MeshRenderer>().material.color = defaultConnectedCoplor;
            Connecttoroom();
        }
    }
    void Connecttoroom()
    {
        roomManager = SharingStage.Instance.Manager.GetRoomManager();
        roomManagerListener = new RoomManagerAdapter();

        currentRoom = roomManager.GetCurrentRoom();
    }

   

}

