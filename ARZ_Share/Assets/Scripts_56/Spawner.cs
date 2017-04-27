using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Persistence;
using UnityEngine.VR.WSA;
using HoloToolkit.Unity.SpatialMapping;
using HoloToolkit.Unity;
using HoloToolkit.Sharing;
using UnityEngine.VR.WSA.Sharing;
using System;

public class Spawner : MonoBehaviour {

    public GameObject OBJtoSpawn;

    WorldAnchorManager _WAmngr;
    WorldAnchor _myAnchor;
    SharingStage _Sharing;
    bool _pendingExport;
    // Use this for initialization
    int bulletId = 0;
	void Start () {
        timecounter = MasterTimerCounter;
        //_WAmngr = WorldAnchorManager.Instance;
       // _WAmngr.AttachAnchor(this.gameObject, "SPAWNERTstrName");

        _Sharing = SharingStage.Instance;
        _myAnchor = this.gameObject.AddComponent<WorldAnchor>();
       // _pendingExport = true;
    }

    float MasterTimerCounter = 3f;
    float timecounter;

    void CounTDownandSpawn() {

        if (timecounter <= 0f) {
            IncrementID();
             timecounter = MasterTimerCounter;
            GameObject go=Instantiate(OBJtoSpawn,this.transform.position, this.transform.localRotation)as GameObject;
            
            Spawneee buletscript = go.GetComponent<Spawneee>();
            buletscript.UpdateLBLID("id="+bulletId.ToString());
            go.transform.parent = this.transform;
        }

        timecounter -= Time.deltaTime;
    }

	void Update () {
 
        if(_Sharing.SessionUsersTracker.CurrentUsers.Count>1)
        CounTDownandSpawn();

    }

    void IncrementID() {
        bulletId++;
    }

    void SetColor(Color argColor)
    {
        this.gameObject.GetComponent<Renderer>().material.color = argColor;
    }

    public enum SpawnerMode {
        ServerMode,
        ClientMode
    }
    public SpawnerMode myMode;

    void BegginExport() {
        WorldAnchorTransferBatch batch = new WorldAnchorTransferBatch();
        batch.AddWorldAnchor("SPAWNERTstrName", _myAnchor);
        WorldAnchorTransferBatch.ExportAsync(batch, OnExportDataAvailable, OnExportCOmplete);
    }
    private void OnExportDataAvailable(byte[] data)
    {
        SetColor(Color.cyan);
    }
    private void OnExportCOmplete(SerializationCompletionReason completionReason)
    {
        SetColor(completionReason == SerializationCompletionReason.Succeeded ? Color.grey : Color.green);
    }


}
