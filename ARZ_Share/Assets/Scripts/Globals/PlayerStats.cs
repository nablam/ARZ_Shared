// @Author Nabil Lamriben ©2017
using HoloToolkit.Sharing;
using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class PlayerStats : Singleton<PlayerStats> {

    #region ServerClient
    private bool IS_SERVER;
    private bool HASBEENSET;

    public void SetIsServer(bool argIsserver) {
        if (!HASBEENSET) {
            IS_SERVER = argIsserver;
            HASBEENSET = true;
        }
    }
    public bool GetIsServer() { return IS_SERVER; }
    public bool GetHasBeenSet()
    {
        return HASBEENSET;
    }
    #endregion

    protected SpatialMappingManager spatialMappingManager;
    public GameObject ParentWithImportExport;
    public GameObject dumbassCube;

    //   public GameObject 
    public PrefabSpawnManager spawnManager;
    public void PlaceSpawnPoint()
    {
        // If we're networking...
        if (SharingStage.Instance.IsConnected || true)
        {
            Vector3 headPosition = Camera.main.transform.position;
            Vector3 gazeDirection = Camera.main.transform.forward;
            RaycastHit hitInfo;
            if (Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f ))
            {
                // Rotate this object to face the user.
                Quaternion toQuat = Camera.main.transform.localRotation;
                toQuat.x = 0;
                toQuat.z = 0;

                // Use the span manager to span a 'SyncSpawnedObject' at that position with
                // some random rotation, parent it off our gameObject, give it a base name (MyCube)
                // and do not claim ownership of it so it stays behind in the scene even if our
                // device leaves the session.


                // GameObject go = Instantiate(dumbassCube, hitInfo.point, Quaternion.identity) as GameObject;
                this.spawnManager.Spawn(
                  new SyncSpawnedObject(),
                  hitInfo.point,
                  Quaternion.identity,
                  this.gameObject,
                  "ZSpawn",
                  false);

            }

            // Make a new cube that is 2m away in direction of gaze but then get that position
            // relative to the object that we are attached to (which is world anchor'd across
            // our devices).
            //var newCubePosition =
            //  this.gameObject.transform.InverseTransformPoint(
            //    (GazeManager.Instance.GazeOrigin + GazeManager.Instance.GazeNormal * 2.0f));


        }
    }
    public void PlacePath() { }

    protected override void Awake()
    {
        base.Awake();
        IS_SERVER = false;
        HASBEENSET = false;
    }


    void Start () {
		
	}
   


}
