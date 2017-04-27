using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VR.WSA.Persistence;
using UnityEngine.VR.WSA;
using System.Collections;
using System.Collections.Generic;

using HoloToolkit.Unity.InputModule;

using UnityEngine.VR.WSA.Input;
using HoloToolkit.Sharing;

public class GameMNGR : MonoBehaviour {


    public LayerMask layerMask = Physics.DefaultRaycastLayers;


    public GameObject OBJ_ZombieSpawner;
      string Name_Anchor_ZombiSpwaner;  
    List<GameObject> List_OBJZombieSpawners;
    int ID_Anchor_ZombieSpawner = 0;

    public GameObject OBJ_Barrier;
      string Name_Anchor_Barrier;
    List<GameObject> List_OBJBarriers;
    int ID_Anchor_Barrier = 0;

    public GameObject OBJ_PathFinder;
      string Name_Anchor_PathFinder;
    bool pathFinderPlaced = false;

    WorldAnchorStore anchorStore;
    bool calledToAnchorStore = false;

    // Use this for initialization
    void Start () {
        Name_Anchor_ZombiSpwaner = "ARZzobiespawner";
        Name_Anchor_Barrier = "ARZbarrier";
        Name_Anchor_PathFinder = "ARZpathfinder";
        List_OBJZombieSpawners = new List<GameObject>();
        List_OBJBarriers = new List<GameObject>();
        //I CAN WAIT FOR ROOM LOADER TO MAKE ITS OWN ASYNC CALL , GET ITS SHIT  , AND CALL HERE TO STTART ANOTHER ASYNC AND GET MY SHIT
        WorldAnchorStore.GetAsync(AnchorStoreReady);


    }
    void AnchorStoreReady(WorldAnchorStore store)
    {
         anchorStore = store;
        
         
       LoadObjects();
    }
    // Update is called once per frame
    void Update () {
		
	}
 
    void LoadObjects()
    { 


        // gather all stored anchors
        string[] ids = anchorStore.GetAllIds();


        Debug.Log("found " + ids.Length + " anchor ids");
        foreach (string id in ids) { Debug.Log("id" + id); }

       


        for (int index = 0; index < ids.Length; index++)
        {
            if (ids[index] == Name_Anchor_PathFinder)
            {
                GameObject obj = Instantiate(OBJ_PathFinder) as GameObject;
                anchorStore.Load(ids[index], obj);

                // delete anchor component
                WorldAnchor attachedAnchor = obj.GetComponent<WorldAnchor>();
                if (attachedAnchor != null)
                    DestroyImmediate(attachedAnchor);
                //obj.transform.Rotate(transform.up, 180.0f);
            }

            else if (ids[index].Contains(Name_Anchor_Barrier))
            {
                // if anchor is barrier
                // instantiate barrier from anchor data
                GameObject obj = Instantiate(OBJ_Barrier) as GameObject;
                anchorStore.Load(ids[index], obj);

                List_OBJBarriers.Add(obj);

                // delete anchor component
                WorldAnchor attachedAnchor = obj.GetComponent<WorldAnchor>();
                if (attachedAnchor != null)
                    DestroyImmediate(attachedAnchor);
            }
            else if (ids[index].Contains(Name_Anchor_ZombiSpwaner))
            {
                // if anchor is a spawn point
                // instantiate spawn point from anchor data
                GameObject obj = Instantiate(OBJ_ZombieSpawner) as GameObject;
                anchorStore.Load(ids[index], obj);

                // add spawn point to collection
                List_OBJZombieSpawners.Add(obj);

                // delete anchor component
                WorldAnchor attachedAnchor = obj.GetComponent<WorldAnchor>();
                if (attachedAnchor != null)
                    DestroyImmediate(attachedAnchor);
            }

        }

       // LevelLoaded();
    }
}
