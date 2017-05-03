using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Persistence;

public class RoomLoader : MonoBehaviour {

    public GameObject managerObject;            // the room manager for this scene
    public GameObject surfaceObject;            // prefab for surface mesh objects
    string fileName = "nabroom";             // name of file to store meshes
    string anchorStoreName = "nabmesh";              // name of world anchor for room

    WorldAnchorStore anchorStore;               // store of world anchors
    List<Mesh> roomMeshes;                      // list of room meshes
    List<GameObject> roomObjects;               // list of game objects that hold room meshes
                                                //List<MeshFilter> filters;                 // mesh filters that store loaded meshes

    // Use this for initialization
    void Start()
    {
        //filters = new List<MeshFilter>();
        CONBUG.Instance.LOGit("async");
        // get instance of WorldAnchorStore
        WorldAnchorStore.GetAsync(AnchorStoreReady);
    }

    public void ToggleRoom()
    {
        foreach (GameObject obj in roomObjects)
        {
            if (obj.activeInHierarchy)
                obj.SetActive(false);
            else
                obj.SetActive(true);
        }
    }

    void AnchorStoreReady(WorldAnchorStore store)
    {
        // save instance
        anchorStore = store;
       
        // load room meshes
        roomMeshes = MeshSaver.Load(fileName) as List<Mesh>;
        roomObjects = new List<GameObject>();
        CONBUG.Instance.LOGit("room meshes in meshsaver =" + roomMeshes.Count);
        for (int x = 1; x <= roomMeshes.Count; x++) {

           // CONBUG.Instance.LOGit("GEtting ---> "  + roomMeshes[x]  surface.name);
        }
        foreach (Mesh surface in roomMeshes)
        {

            CONBUG.Instance.LOGit("GEtting ---> " + surface.name);
            GameObject obj = Instantiate(surfaceObject) as GameObject;
            obj.GetComponent<MeshFilter>().mesh = surface;
            obj.GetComponent<MeshCollider>().sharedMesh = surface;
            roomObjects.Add(obj);
            //filters.Add(obj.GetComponent<MeshFilter>());

            if (!anchorStore.Load(surface.name, obj))
                CONBUG.Instance.LOGit("WorldAnchor load failed...");

            // Debug.Log("Mesh " + surface.name + " Position: " + obj.transform.position + "\n--- Rotation: " + obj.transform.localRotation + "\n--- Scale: " + obj.transform.localScale);
        }

        //foreach (Mesh surface in roomMeshes)
        //{

        //    CONBUG.Instance.LOGit("GEtting ---> "+surface.name);
        //   GameObject obj = Instantiate(surfaceObject) as GameObject;
        //    obj.GetComponent<MeshFilter>().mesh = surface;
        //    obj.GetComponent<MeshCollider>().sharedMesh = surface;
        //    roomObjects.Add(obj);
        //    //filters.Add(obj.GetComponent<MeshFilter>());

        //    if (!anchorStore.Load(surface.name, obj))
        //        CONBUG.Instance.LOGit("WorldAnchor load failed...");

        //    // Debug.Log("Mesh " + surface.name + " Position: " + obj.transform.position + "\n--- Rotation: " + obj.transform.localRotation + "\n--- Scale: " + obj.transform.localScale);
        //}

        if (managerObject != null)
            managerObject.SendMessage("RoomLoaded");
    }

    void OnDestroy()
    {
        foreach (Mesh mesh in roomMeshes)
        {
            Destroy(mesh);
        }
        roomMeshes.Clear();
    }
}
