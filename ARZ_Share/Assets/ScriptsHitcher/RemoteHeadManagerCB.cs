
using System;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Sharing.Tests;
using HoloToolkit.Sharing;
using System.Collections;

public class RemoteHeadManagerCB : Singleton<RemoteHeadManagerCB>
{
    public GameObject P1Helmet;
    public GameObject P2Helmet;
    private bool IAMSERVER = false;
    public bool GetAmServer() { return IAMSERVER; }
    bool started = false;
    public Vector3 targetedServerHead;
  //  public Vector3 clientHEad;

    public Material lazorMat;
    public Material bluemat;
    bool initIdentity() {
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

    public class RemoteHeadInfo
    {
        public long UserID;
        public GameObject HeadObject;
    }


    void Drawblue(Vector3 start, Vector3 end)
    {

        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
       // lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.material = lazorMat;
        lr.startColor = Color.blue;
        lr.endColor = Color.blue;
        lr.startWidth = 0.01f;
        lr.endWidth = 0.01f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, 0.2f);
    }
    /// <summary>
    /// Keep a list of the remote heads, indexed by XTools userID
    /// </summary>
    private Dictionary<long, RemoteHeadInfo> remoteHeads = new Dictionary<long, RemoteHeadInfo>();

    IEnumerator startin1() {
        yield return new WaitForSeconds(1);
        // SharingStage should be valid at this point, but we may not be connected.
        if (SharingStage.Instance.IsConnected)
        {
            IAMSERVER = initIdentity();
            Connected();
        }
        else
        {
            SharingStage.Instance.SharingManagerConnected += Connected;
        }

        started = true;
    }
    private void Start()
    {
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.HeadTransform] = UpdateHeadTransform;
        StartCoroutine("startin1");
      
    }

    private void Connected(object sender = null, EventArgs e = null)
    {
        SharingStage.Instance.SharingManagerConnected -= Connected;

        SharingStage.Instance.SessionUsersTracker.UserJoined += UserJoinedSession;
        SharingStage.Instance.SessionUsersTracker.UserLeft += UserLeftSession;
    }

    void findANDsetTargetServerHead() {
        if (IAMSERVER)
        {
            Transform headTransform = Camera.main.transform;
            targetedServerHead = transform.InverseTransformPoint(headTransform.position);
        }
        else {
            //if(clientHEad!=null)
            //serverhead = clientHEad;
        }
    }
    private void Update()
    {
        if (started)
        {         // Grab the current head transform and broadcast it to all the other users in the session
            Transform headTransform = Camera.main.transform;

            // Transform the head position and rotation from world space into local space
            Vector3 headPosition = transform.InverseTransformPoint(headTransform.position);
            Quaternion headRotation = Quaternion.Inverse(transform.rotation) * headTransform.rotation;

            CustomMessages.Instance.SendHeadTransform(headPosition, headRotation);

            if (IAMSERVER) { targetedServerHead = headPosition; }
            
                

           // findANDsetTargetServerHead();
           if(targetedServerHead!=null)
            Drawblue(this.transform.position, targetedServerHead);
        }

    }

    protected override void OnDestroy()
    {
        if (SharingStage.Instance != null)
        {
            if (SharingStage.Instance.SessionUsersTracker != null)
            {
                SharingStage.Instance.SessionUsersTracker.UserJoined -= UserJoinedSession;
                SharingStage.Instance.SessionUsersTracker.UserLeft -= UserLeftSession;
            }
        }

        base.OnDestroy();
    }

    /// <summary>
    /// Called when a new user is leaving the current session.
    /// </summary>
    /// <param name="user">User that left the current session.</param>
    private void UserLeftSession(User user)
    {
        int userId = user.GetID();
        if (userId != SharingStage.Instance.Manager.GetLocalUser().GetID())
        {
            RemoveRemoteHead(remoteHeads[userId].HeadObject);
            remoteHeads.Remove(userId);
        }
    }

    /// <summary>
    /// Called when a user is joining the current session.
    /// </summary>
    /// <param name="user">User that joined the current session.</param>
    private void UserJoinedSession(User user)
    {
        if (user.GetID() != SharingStage.Instance.Manager.GetLocalUser().GetID())
        {
            GetRemoteHeadInfo(user.GetID());
        }
    }

    /// <summary>
    /// Gets the data structure for the remote users' head position.
    /// </summary>
    /// <param name="userId">User ID for which the remote head info should be obtained.</param>
    /// <returns>RemoteHeadInfo for the specified user.</returns>
    public RemoteHeadInfo GetRemoteHeadInfo(long userId)
    {
        RemoteHeadInfo headInfo;

        // Get the head info if its already in the list, otherwise add it
        if (!remoteHeads.TryGetValue(userId, out headInfo))
        {
            headInfo = new RemoteHeadInfo();
            headInfo.UserID = userId;
            headInfo.HeadObject = CreateRemoteHead();

            remoteHeads.Add(userId, headInfo);
        }

        return headInfo;
    }

    /// <summary>
    /// Called when a remote user sends a head transform.
    /// </summary>
    /// <param name="msg"></param>
    private void UpdateHeadTransform(NetworkInMessage msg)
    {
        // Parse the message
        long userID = msg.ReadInt64();

        Vector3 headPos = CustomMessages.Instance.ReadVector3(msg);

        Quaternion headRot = CustomMessages.Instance.ReadQuaternion(msg);

        RemoteHeadInfo headInfo = GetRemoteHeadInfo(userID);
        headInfo.HeadObject.transform.localPosition = headPos;
        headInfo.HeadObject.transform.localRotation = headRot;

        // serverhead = headPos;
        targetedServerHead = transform.InverseTransformPoint(headPos);
    }

    /// <summary>
    /// Creates a new game object to represent the user's head.
    /// </summary>
    /// <returns></returns>
    private GameObject CreateRemoteHead()
    {
        //GameObject newHeadObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //newHeadObj.transform.parent = gameObject.transform;
        //newHeadObj.transform.localScale = Vector3.one * 0.2f;
        //return newHeadObj;
        if (IAMSERVER)
        {

            CONBUG.Instance.LOGit("I am server so I see ironman");

            GameObject newHeadObj = Instantiate(P2Helmet);
            newHeadObj.transform.parent = gameObject.transform;
            newHeadObj.transform.localScale = Vector3.one;
            return newHeadObj;

        }
        else
        {
            CONBUG.Instance.LOGit("I am cient i see the king");

            GameObject newHeadObj = Instantiate(P1Helmet);
            newHeadObj.transform.parent = gameObject.transform;
            newHeadObj.transform.localScale = Vector3.one;
            return newHeadObj;
        }
    }

    /// <summary>
    /// When a user has left the session this will cleanup their
    /// head data.
    /// </summary>
    /// <param name="remoteHeadObject"></param>
    private void RemoveRemoteHead(GameObject remoteHeadObject)
    {
        DestroyImmediate(remoteHeadObject);
    }
}
