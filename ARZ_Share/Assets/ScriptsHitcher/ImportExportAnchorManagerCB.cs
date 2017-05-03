
//ImportExportAnchorManagerCB  
 
using System;
using System.Collections;
using UnityEngine;
using HoloToolkit.Unity;


using System.Collections.Generic;
using UnityEngine.VR.WSA;
using UnityEngine.VR.WSA.Persistence;
using UnityEngine.VR.WSA.Sharing;


namespace HoloToolkit.Sharing.Tests
{
    /// <summary>
    /// Manages creating anchors and sharing the anchors with other clients.
    /// </summary>
    public class ImportExportAnchorManagerCB : Singleton<ImportExportAnchorManagerCB>
    {
        private enum ImportExportState
        {
            // Overall states
            Start,
            Failed,
            Ready,
            RoomApiInitialized,
            AnchorEstablished,
            // AnchorStore states
            AnchorStore_Initializing,
            // Anchor creation values
            InitialAnchorRequired,
            CreatingInitialAnchor,
            ReadyToExportInitialAnchor,
            UploadingInitialAnchor,
            // Anchor values
            DataRequested,
            DataReady,
            Importing
        }

        private ImportExportState currentState = ImportExportState.Start;

        public string StateName
        {
            get
            {
                return currentState.ToString();
            }
        }

        public bool AnchorEstablished
        {
            get
            {
                return currentState == ImportExportState.AnchorEstablished;
            }
        }
        public long RoomID
        {
            get
            {
                return roomID;
            }

            set
            {
                if (currentRoom == null)
                {
                    roomID = value;
                }
            }
        }

        private static bool ShouldLocalUserCreateRoom
        {
            get
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
        }

 
        public event Action<bool> AnchorUploaded;

        public event Action AnchorLoaded;
        private WorldAnchorTransferBatch sharedAnchorInterface;
        private WorldAnchorStore anchorStore;
        private string exportingAnchorName;
        private List<byte> exportingAnchorBytes = new List<byte>();
        private const uint MinTrustworthySerializedAnchorDataSize = 100000;
        private byte[] rawAnchorData;
        private bool sharingServiceReady;
        private RoomManager roomManager;     
        private Room currentRoom;
        private long roomID = 8675309;
        public bool KeepRoomAlive;
        public string RoomName = "DefaultRoom";
        private RoomManagerAdapter roomManagerListener;

        #region Untiy APIs

        protected override void Awake()
        {
            base.Awake();


        }

        private void Start()
        {
            currentState = ImportExportState.AnchorStore_Initializing;
            WorldAnchorStore.GetAsync(AnchorStoreReady);


            // SharingStage should be valid at this point, but we may not be connected.
            if (SharingStage.Instance.IsConnected)
            {
                Connected();
            }
            else
            {
                SharingStage.Instance.SharingManagerConnected += Connected;
            }
        }

        private void Update()
        {
            switch (currentState)
            {
                // If the local anchor store is initialized.
                case ImportExportState.Ready:
                    if (sharingServiceReady)
                    {
                        StartCoroutine(InitRoomApi());
                    }
                    break;
                case ImportExportState.RoomApiInitialized:
                    StartAnchorProcess();
                    break;
                case ImportExportState.DataReady:
                    // DataReady is set when the anchor download completes.
                    currentState = ImportExportState.Importing;
                    WorldAnchorTransferBatch.ImportAsync(rawAnchorData, ImportComplete);
                    break;
                case ImportExportState.InitialAnchorRequired:
                    currentState = ImportExportState.CreatingInitialAnchor;
                    CreateAnchorLocally();
                    break;
                case ImportExportState.ReadyToExportInitialAnchor:
                    // We've created an anchor locally and it is ready to export.
                    currentState = ImportExportState.UploadingInitialAnchor;
                    Export();
                    break;
            }
        }

        protected override void OnDestroy()
        {
            if (SharingStage.Instance != null)
            {
                if (SharingStage.Instance.SessionsTracker != null)
                {
                    SharingStage.Instance.SessionsTracker.CurrentUserJoined -= CurrentUserJoinedSession;
                    SharingStage.Instance.SessionsTracker.CurrentUserLeft -= CurrentUserLeftSession;
                }
            }

            if (roomManagerListener != null)
            {
                roomManagerListener.AnchorsChangedEvent -= RoomManagerCallbacks_AnchorsChanged;
                roomManagerListener.AnchorsDownloadedEvent -= RoomManagerListener_AnchorsDownloaded;
                roomManagerListener.AnchorUploadedEvent -= RoomManagerListener_AnchorUploaded;

                if (roomManager != null)
                {
                    roomManager.RemoveListener(roomManagerListener);
                }

                roomManagerListener.Dispose();
                roomManagerListener = null;
            }

            if (roomManager != null)
            {
                roomManager.Dispose();
                roomManager = null;
            }

            base.OnDestroy();
        }

        #endregion

        #region Event Callbacks

        private void Connected(object sender = null, EventArgs e = null)
        {
            SharingStage.Instance.SharingManagerConnected -= Connected;

            if (CONBUG.Instance != null){CONBUG.Instance.LOGit("impexpAnchor Manager: Starting...");}

            // Setup the room manager callbacks.
            roomManager = SharingStage.Instance.Manager.GetRoomManager();
            roomManagerListener = new RoomManagerAdapter();

            roomManagerListener.AnchorsChangedEvent += RoomManagerCallbacks_AnchorsChanged;
            roomManagerListener.AnchorsDownloadedEvent += RoomManagerListener_AnchorsDownloaded;
            roomManagerListener.AnchorUploadedEvent += RoomManagerListener_AnchorUploaded;

            roomManager.AddListener(roomManagerListener);

            // We will register for session joined and left to indicate when the sharing service
            // is ready for us to make room related requests.
            SharingStage.Instance.SessionsTracker.CurrentUserJoined += CurrentUserJoinedSession;
            SharingStage.Instance.SessionsTracker.CurrentUserLeft += CurrentUserLeftSession;
        }

        private void RoomManagerListener_AnchorUploaded(bool successful, XString failureReason)
        {
            if (successful)
            {

                if (CONBUG.Instance != null){ CONBUG.Instance.LOGit("impExp impexpAnchor Manager: Sucessfully uploaded anchor");}
                currentState = ImportExportState.AnchorEstablished;
            }
            else
            {
                if (CONBUG.Instance != null){ CONBUG.Instance.LOGitError("impExp impexpAnchor Manager: Upload failed " + failureReason);}             
                currentState = ImportExportState.Failed;
            }

            if (AnchorUploaded != null)
            {
                AnchorUploaded(successful);
            }
        }

        private void RoomManagerListener_AnchorsDownloaded(bool successful, AnchorDownloadRequest request, XString failureReason)
        {
            // If we downloaded anchor data successfully we should import the data.
            if (successful)
            {
                int datasize = request.GetDataSize();

                if (CONBUG.Instance != null){CONBUG.Instance.LOGitFormat("impexpAnchor Manager: Anchor size: {0} bytes.", datasize.ToString());}

                rawAnchorData = new byte[datasize];

                request.GetData(rawAnchorData, datasize);
                currentState = ImportExportState.DataReady;
            }
            else
            {
                if (CONBUG.Instance != null){CONBUG.Instance.LOGitWarning("impexpAnchor Manager: Anchor DL failed " + failureReason);}

                // If we failed, we can ask for the data again.
#if UNITY_WSA && !UNITY_EDITOR
                MakeAnchorDataRequest();
#endif
            }
        }

        private void RoomManagerCallbacks_AnchorsChanged(Room room)
        {
            if (CONBUG.Instance != null){CONBUG.Instance.LOGitFormat("impexpAnchor Manager: Anchors in room {0} changed", room.GetName());}

            // if we're currently in the room where the anchors changed
            if (currentRoom == room)
            {
                ResetState();
            }
        }

        private void CurrentUserJoinedSession(Session session)
        {
            if (SharingStage.Instance.Manager.GetLocalUser().IsValid())
            {
                sharingServiceReady = true;
            }
            else
            {
                if (CONBUG.Instance != null) { CONBUG.Instance.LOGitWarning("Unable to get local user on session joined");  };
            }
        }

        private void CurrentUserLeftSession(Session session)
        {
            sharingServiceReady = false;
            // Reset the state so that we join a new room when we eventually rejoin a session
            ResetState();
        }

        #endregion

        private void ResetState()
        {
            if (anchorStore != null)
            {
                currentState = ImportExportState.Ready;
            }
            else
            {
                currentState = ImportExportState.AnchorStore_Initializing;
            }

        }

        private IEnumerator InitRoomApi()
        {
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
                        if (CONBUG.Instance != null){CONBUG.Instance.LOGit("impexpAnchor Manager: Creating room " + RoomName);}

                        // To keep anchors alive even if all users have left the session ...
                        // Pass in true instead of false in CreateRoom.
                        currentRoom = roomManager.CreateRoom(new XString(RoomName), roomID, KeepRoomAlive);
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

                            if (CONBUG.Instance != null){CONBUG.Instance.LOGit("impexpAnchor Manager: Joining room " + room.GetName().GetString());}

                            break;
                        }
                    }

                    if (currentRoom == null)
                    {
                        // Couldn't locate a matching room, just join the first one.
                        currentRoom = roomManager.GetRoom(0);
                        roomManager.JoinRoom(currentRoom);
                        RoomName = currentRoom.GetName().GetString();
                    }

                    currentState = ImportExportState.RoomApiInitialized;
                }

                yield return new WaitForEndOfFrame();
            }

            if (currentRoom.GetAnchorCount() == 0)
            {
                // If the room has no anchors, request the initial anchor
                currentState = ImportExportState.InitialAnchorRequired;
            }
            else
            {
                // Room already has anchors
                currentState = ImportExportState.RoomApiInitialized;
            }

            if (CONBUG.Instance != null){CONBUG.Instance.LOGit("impexpAnchor Manager: In room {" + roomManager.GetCurrentRoom().GetName().GetString() + "} with ID {" + roomManager.GetCurrentRoom().GetID().ToString() + " }");}

            yield return null;
        }

        /// <summary>
        /// Kicks off the process of creating the shared space.
        /// </summary>
        private void StartAnchorProcess()
        {
            // First, are there any anchors in this room?
            int anchorCount = currentRoom.GetAnchorCount();

            if (CONBUG.Instance != null){CONBUG.Instance.LOGitFormat("impexpAnchor Manager: {0} anchors found.", anchorCount.ToString());}


            // If there are anchors, we should attach to the first one.
            if (anchorCount > 0)
            {
                // Extract the name of the anchor.
                XString storedAnchorString = currentRoom.GetAnchorName(0);
                string storedAnchorName = storedAnchorString.GetString();

                // Attempt to attach to the anchor in our local anchor store.
                if (AttachToCachedAnchor(storedAnchorName) == false)
                {
                    if (CONBUG.Instance != null){CONBUG.Instance.LOGit("impexpAnchor Manager: Starting room anchor download of " + storedAnchorString);}

                    // If we cannot find the anchor by name, we will need the full data blob.
                    MakeAnchorDataRequest();
                }
            }


        }

        #region WSA Specific Methods




        private void MakeAnchorDataRequest()
        {
            if (roomManager.DownloadAnchor(currentRoom, currentRoom.GetAnchorName(0)))
            {
                currentState = ImportExportState.DataRequested;
            }
            else
            {

                if (CONBUG.Instance != null){CONBUG.Instance.LOGitError("impexpAnchor Manager: Couldn't make the download request.");}

                currentState = ImportExportState.Failed;
            }
        }

        private void AnchorStoreReady(WorldAnchorStore store)
        {
            anchorStore = store;

            if (!KeepRoomAlive)
            {
                anchorStore.Clear();
            }

            currentState = ImportExportState.Ready;
        }

        private void CreateAnchorLocally()
        {
            WorldAnchor anchor = this.EnsureComponent<WorldAnchor>();
            if (anchor.isLocated)
            {
                currentState = ImportExportState.ReadyToExportInitialAnchor;
            }
            else
            {
                anchor.OnTrackingChanged += Anchor_OnTrackingChanged_InitialAnchor;
            }
        }

  
        private void Anchor_OnTrackingChanged_InitialAnchor(WorldAnchor self, bool located)
        {
            if (located)
            {
                if (CONBUG.Instance != null){CONBUG.Instance.LOGit("impexpAnchor Manager: Found anchor, ready to export");}

                currentState = ImportExportState.ReadyToExportInitialAnchor;
            }
            else
            {
                if (CONBUG.Instance != null){CONBUG.Instance.LOGitError("impexpAnchor Manager: Failed to locate local anchor!");}

                currentState = ImportExportState.Failed;
            }

            self.OnTrackingChanged -= Anchor_OnTrackingChanged_InitialAnchor;
        }

        private bool AttachToCachedAnchor(string anchorName)
        {
            if (CONBUG.Instance != null){CONBUG.Instance.LOGitFormat("impexpAnchor Manager: Looking for cahced anchor {0}...", anchorName);}

            string[] ids = anchorStore.GetAllIds();
            for (int index = 0; index < ids.Length; index++)
            {
                if (ids[index] == anchorName)
                {
                    if (CONBUG.Instance != null){CONBUG.Instance.LOGitFormat("impexpAnchor Manager: Attempting to load cached anchor {0}...", anchorName);}

                    WorldAnchor anchor = anchorStore.Load(ids[index], gameObject);

                    if (anchor.isLocated)
                    {
                        AnchorLoadComplete();
                    }
                    else
                    {
                        if (CONBUG.Instance != null){ CONBUG.Instance.LOGitFormat("impexpAnchor Manager:  {0} is not yet located ", anchorName);}

                        anchor.OnTrackingChanged += ImportExportAnchorManager_OnTrackingChanged_Attaching;
                        currentState = ImportExportState.AnchorEstablished;
                    }
                    return true;
                }
            }

            // Didn't find the anchor, so we'll download from room.
            return false;
        }


        private void ImportExportAnchorManager_OnTrackingChanged_Attaching(WorldAnchor self, bool located)
        {
            if (located)
            {
                AnchorLoadComplete();
            }
            else
            {

                if (CONBUG.Instance != null){CONBUG.Instance.LOGitWarning("impexpAnchor Manager: Failed to find local anchor from cache.");}

                MakeAnchorDataRequest();
            }

            self.OnTrackingChanged -= ImportExportAnchorManager_OnTrackingChanged_Attaching;
        }

        private void ImportComplete(SerializationCompletionReason status, WorldAnchorTransferBatch anchorBatch)
        {
            if (status == SerializationCompletionReason.Succeeded)
            {
                if (anchorBatch.GetAllIds().Length > 0)
                {
                    string first = anchorBatch.GetAllIds()[0];

                    if (CONBUG.Instance != null){CONBUG.Instance.LOGit("impexpAnchor Manager: Sucessfully imported anchor " + first);}

                    WorldAnchor anchor = anchorBatch.LockObject(first, gameObject);
                    anchorStore.Save(first, anchor);
                }

                AnchorLoadComplete();
            }
            else
            {

                if (CONBUG.Instance != null){CONBUG.Instance.LOGitError("impexpAnchor Manager: Import failed");}
                currentState = ImportExportState.DataReady;
            }
        }

        private void AnchorLoadComplete()
        {
            if (AnchorLoaded != null)
            {
                AnchorLoaded();
            }

            currentState = ImportExportState.AnchorEstablished;
        }


        private void Export()
        {
            WorldAnchor anchor = this.GetComponent<WorldAnchor>();
            string guidString = Guid.NewGuid().ToString();
            exportingAnchorName = guidString;

            // Save the anchor to our local anchor store.
            if (anchor != null && anchorStore.Save(exportingAnchorName, anchor))
            {

                if (CONBUG.Instance != null){CONBUG.Instance.LOGit("impexpAnchor Manager: Exporting anchor " + exportingAnchorName);}

                sharedAnchorInterface = new WorldAnchorTransferBatch();
                sharedAnchorInterface.AddWorldAnchor(guidString, anchor);
                WorldAnchorTransferBatch.ExportAsync(sharedAnchorInterface, WriteBuffer, ExportComplete);
            }
            else
            {
                if (CONBUG.Instance != null){CONBUG.Instance.LOGitWarning("impexpAnchor Manager: Failed to export anchor, trying again...");}

                currentState = ImportExportState.InitialAnchorRequired;
            }
        }

        private void WriteBuffer(byte[] data)
        {
            exportingAnchorBytes.AddRange(data);
        }

        private void ExportComplete(SerializationCompletionReason status)
        {
            if (status == SerializationCompletionReason.Succeeded && exportingAnchorBytes.Count > MinTrustworthySerializedAnchorDataSize)
            {
                if (CONBUG.Instance != null){CONBUG.Instance.LOGit("impexpAnchor Manager: Uploading anchor: " + exportingAnchorName);}

                roomManager.UploadAnchor(
                    currentRoom,
                    new XString(exportingAnchorName),
                    exportingAnchorBytes.ToArray(),
                    exportingAnchorBytes.Count);
            }
            else
            {
                if (CONBUG.Instance != null){CONBUG.Instance.LOGitWarning("impexpAnchor Manager: Failed to upload anchor, trying again...");}

                currentState = ImportExportState.InitialAnchorRequired;
            }
        }
        #endregion // WSA Specific Methods
    }
}
