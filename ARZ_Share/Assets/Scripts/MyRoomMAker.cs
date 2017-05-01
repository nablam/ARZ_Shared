using HoloToolkit.Sharing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class MyRoomMAker : MonoBehaviour {

    private RoomManagerAdapter listener;
    private RoomManager roomMgr;
    private Room myroom;
    private bool IAMSERVER;
    private string roomName = "New Room";
    private string anchorName = "New Anchor";
    private readonly byte[] anchorTestData = new byte[5 * 1024 * 1024]; // 5 meg test buffer

    private void CreateRoooom()
    {
        if (IAMSERVER) {
            CONBUG.Instance.LOGit("I AM SERVER i should make room 1337");

            myroom = roomMgr.CreateRoom(roomName, 1337, false);
            CONBUG.Instance.LOGit(" created room 1337");
            if (myroom == null)
            {
                CONBUG.Instance.LOGit("Cannot create room");
            }
        }

    }
    private void JoinRooom() {
        CONBUG.Instance.LOGit("trying to join");
        if (!roomMgr.JoinRoom(myroom))
     {
            CONBUG.Instance.LOGit("Cannot join room");
     }
        
    }
    private void LaveRooom() { }

        // Use this for initialization
    void Start () {
        for (int i = 0; i < anchorTestData.Length; ++i)
        {
            anchorTestData[i] = (byte)(i % 256);
        }

        SharingStage stage = SharingStage.Instance;
        if (stage != null)
        {
            SharingManager sharingMgr = stage.Manager;
            if (sharingMgr != null)
            {
                roomMgr = sharingMgr.GetRoomManager();

                listener = new RoomManagerAdapter();
                listener.RoomAddedEvent += OnRoomAdded;
                listener.RoomClosedEvent += OnRoomClosed;
                listener.UserJoinedRoomEvent += OnUserJoinedRoom;
                listener.UserLeftRoomEvent += OnUserLeftRoom;
                listener.AnchorsChangedEvent += OnAnchorsChanged;
                listener.AnchorsDownloadedEvent += OnAnchorsDownloaded;
                listener.AnchorUploadedEvent += OnAnchorUploadComplete;

                roomMgr.AddListener(listener);


                IAMSERVER = ShouldLocalUserCreateRoom();
                CreateRoooom();

                JoinRooom();
            }


        }

    }
    #region notstart

    private bool ShouldLocalUserCreateRoom()
    {
            if (SharingStage.Instance == null || SharingStage.Instance.SessionUsersTracker == null)
            {
                return false;
            }

            long localUserId;
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

    private void OnDestroy()
    {
        roomMgr.RemoveListener(listener);
        listener.Dispose();
    }

    // Update is called once per frame

    private void OnRoomAdded(Room newRoom)
    {
        CONBUG.Instance.LOGit(string.Format("Room {0} added", newRoom.GetName().GetString()));
    }

    private void OnRoomClosed(Room room)
    {
        CONBUG.Instance.LOGit("Room "+ room.GetName().GetString() + " closed");
    }

    private void OnUserJoinedRoom(Room room, int user)
    {
        User joinedUser = SharingStage.Instance.SessionUsersTracker.GetUserById(user);
        CONBUG.Instance.LOGit(string.Format("User {0} joined Room {1}", joinedUser.GetName(), room.GetName().GetString()));
    }

    private void OnUserLeftRoom(Room room, int user)
    {
        User leftUser = SharingStage.Instance.SessionUsersTracker.GetUserById(user);
        
        CONBUG.Instance.LOGit(string.Format("User {0} left Room {1}", leftUser.GetName(), room.GetName().GetString()));
    }

    private void OnAnchorsChanged(Room room)
    {
        
        CONBUG.Instance.LOGit(string.Format("Anchors changed for Room {0}", room.GetName().GetString()));
    }

    private void OnAnchorsDownloaded(bool successful, AnchorDownloadRequest request, XString failureReason)
    {
        if (successful)
        {
            CONBUG.Instance.LOGit(string.Format("Anchors download succeeded for Room {0}", request.GetRoom().GetName().GetString()));
        }
        else
        {
            CONBUG.Instance.LOGit(string.Format("Anchors download failed: {0}", failureReason.GetString()));
        }
    }

    private void OnAnchorUploadComplete(bool successful, XString failureReason)
    {
        if (successful)
        {
            CONBUG.Instance.LOGit(("Anchors upload succeeded")); 
        }
        else
        {
            CONBUG.Instance.LOGit(string.Format("Anchors upload failed: {0}", failureReason.GetString()));
        }
    }

    #endregion




    private void DOit()
    {
        if (roomMgr != null)
        {
            SessionManager sessionMgr = SharingStage.Instance.Manager.GetSessionManager();
            if (sessionMgr != null)
            {

                //CREATE WAS HERE 

                Room currentRoom = roomMgr.GetCurrentRoom();

                for (int i = 0; i < roomMgr.GetRoomCount(); ++i)
                {
                    Room room = roomMgr.GetRoom(i);


                    bool keepOpen = room.GetKeepOpen();
                    CONBUG.Instance.LOGit("is room keep onen " + keepOpen.ToString());
                    room.SetKeepOpen(keepOpen);


                    if (currentRoom != null && room.GetID() == currentRoom.GetID())
                    {

                        roomMgr.LeaveRoom();

                    }


                    else
                    {


                    }



                    // AreaEffector2D in room =room.GetName().GetString());
                }
            }
        }

    }

}
