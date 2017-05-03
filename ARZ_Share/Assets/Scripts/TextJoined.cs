using System;
using System.Collections;
using UnityEngine;
using HoloToolkit.Unity;

using UnityEngine.VR.WSA;
using UnityEngine.VR.WSA.Persistence;
using UnityEngine.VR.WSA.Sharing;
using HoloToolkit.Sharing;
 

public class TextJoined : MonoBehaviour {

    TextMesh tx;
	void Start () {
        tx = this.gameObject.GetComponent<TextMesh>();

        if (tx != null) { tx.text += "we got the txt"; }

        if (SharingStage.Instance != null)
        {
            if (SharingStage.Instance.SessionsTracker != null)
            {
                SharingStage.Instance.SessionsTracker.CurrentUserJoined += CurUserJoinedSesh;
                //SharingStage.Instance.SessionsTracker.CurrentUserLeft += CurrentUserLeftSession;
            }
        }
    }

    private void OnDestroy()
    {
        if (SharingStage.Instance != null)
        {
            if (SharingStage.Instance.SessionsTracker != null)
            {
                SharingStage.Instance.SessionsTracker.CurrentUserJoined -= CurUserJoinedSesh;
                //SharingStage.Instance.SessionsTracker.CurrentUserLeft += CurrentUserLeftSession;
            }
        }
    }


    private void CurUserJoinedSesh(Session session)
    {
      
            tx.text+= "\n user joined" ;
        string unanme = SharingStage.Instance.UserName;
        
       
    }

    // Update is called once per frame
    void Update () {
		
	}
}
