 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Sharing;
using System;
using HoloToolkit.Unity;

//roommanager will set IS_SERVER in PlayerStats
public class PlayerRoomManager : Singleton<PlayerRoomManager>
{

    private RoomManager roomManager;
    private Room currentRoom;
    private long roomID = 1337;
    private string RoomName = "LEETroom";
    public bool sharingServiceReady;
    private bool AlreadyDone;

    private enum RoomManagerStates
    {
        // Overall states
        Start,
        Failed,
        Ready,
        RoomApiInitialized
    }

    private RoomManagerStates currentState; 

    void Start () {
        AlreadyDone = false;
        sharingServiceReady = false;
       
        // SharingStage should be valid at this point, but we may not be connected.
        if (SharingStage.Instance.IsConnected){Connected();}else{ SharingStage.Instance.SharingManagerConnected += Connected;}
        currentState = RoomManagerStates.Ready;
    }

    private void Connected(object sender = null, EventArgs e = null)
    {
        SharingStage.Instance.SharingManagerConnected -= Connected;
        CONBUG.Instance.LOGit("Connected to Server");
        
        // Setup the room manager callbacks.
        roomManager = SharingStage.Instance.Manager.GetRoomManager();
 

        // We will register for session joined and left to indicate when the sharing service
        // is ready for us to make room related requests.
        SharingStage.Instance.SessionsTracker.CurrentUserJoined += CurrentUserJoinedSession;
        SharingStage.Instance.SessionsTracker.CurrentUserLeft += CurrentUserLeftSession;
    }
    private void CurrentUserJoinedSession(Session session){
        if (SharingStage.Instance.Manager.GetLocalUser().IsValid()){sharingServiceReady = true; }else{ CONBUG.Instance.LOGit("Unable to get local user on session joined"); }
    }

    private void CurrentUserLeftSession(Session session)
    {
        sharingServiceReady = false;
    }

    // Update is called once per frame
    void Update () {

        switch (currentState)
        {
            // If the local anchor store is initialized.
            case RoomManagerStates.Ready:
                if (sharingServiceReady)
                {
                    StartCoroutine(InitRoomApi());
                }
                break;
            case RoomManagerStates.RoomApiInitialized:
                if (!AlreadyDone)
                {
                    CONBUG.Instance.LOGit("done");
                    AlreadyDone = true;
                }

                break;
        }

    }

    private IEnumerator InitRoomApi()
    {
        // First check if there is a current room
        currentRoom = roomManager.GetCurrentRoom();

        while (currentRoom == null)
        {
            // If we have a room, we'll join the first room we see.
            // If we are the user with the lowest user ID, we will create the room.
            // Otherwise we will wait for the room to be created.
            if (roomManager.GetRoomCount() == 0)
            {
                if (ShouldLocalUserCreateRoom)
                {
                    PlayerStats.Instance.SetIsServer(true);
                    CONBUG.Instance.LOGit("Creating room 4I am server " + RoomName);
                    // To keep anchors alive even if all users have left the session ...
                    // Pass in true instead of false in CreateRoom.
                    currentRoom = roomManager.CreateRoom(new XString(RoomName), roomID, false);
                }
                else
                {
                    PlayerStats.Instance.SetIsServer(false);
                }
               
            }
            else
            {
                // Look through the existing rooms and join the one that matches the room name provided.
                int roomCount = roomManager.GetRoomCount();
                for (int i = 0; i < roomCount; i++)
                {
                    Room room = roomManager.GetRoom(i);
                    if (room.GetName().GetString().Equals(RoomName, StringComparison.OrdinalIgnoreCase))
                    {
                        currentRoom = room;
                        roomManager.JoinRoom(currentRoom);
                        CONBUG.Instance.LOGit("Anchor Manager: Joining room " + room.GetName().GetString());
                       
                        break;
                    }
                }
                /*
                if (currentRoom == null)
                {
                    // Couldn't locate a matching room, just join the first one.
                    currentRoom = roomManager.GetRoom(0);
                    roomManager.JoinRoom(currentRoom);
                    RoomName = currentRoom.GetName().GetString();
                }*/
               

            }
           
            yield return new WaitForEndOfFrame();
        }
 
 
           string s= string.Format("Anchor Manager: In room {0} with ID {1}", roomManager.GetCurrentRoom().GetName().GetString(),roomManager.GetCurrentRoom().GetID().ToString());
            CONBUG.Instance.LOGit(s);
        currentState = RoomManagerStates.RoomApiInitialized;


        yield return null;
    }


    private static bool ShouldLocalUserCreateRoom
    {
        get
        {
            if (SharingStage.Instance == null || SharingStage.Instance.SessionUsersTracker == null){return false;  }
            long localUserId;
            using (User localUser = SharingStage.Instance.Manager.GetLocalUser()) {localUserId = localUser.GetID();}
            for (int i = 0; i < SharingStage.Instance.SessionUsersTracker.CurrentUsers.Count; i++){
                if (SharingStage.Instance.SessionUsersTracker.CurrentUsers[i].GetID() < localUserId){return false;}
            }
            return true;
        }
    }

    void OnDestroy()
    {
        if (SharingStage.Instance != null) {
            if (SharingStage.Instance.SessionsTracker != null) { SharingStage.Instance.SessionsTracker.CurrentUserJoined -= CurrentUserJoinedSession;SharingStage.Instance.SessionsTracker.CurrentUserLeft -= CurrentUserLeftSession;}}
        if (roomManager != null){ roomManager.Dispose();roomManager = null; }
    }




}
