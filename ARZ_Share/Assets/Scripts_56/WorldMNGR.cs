using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VR.WSA.Persistence;
using UnityEngine.VR.WSA;
using System.Collections;
using System.Collections.Generic;
 
using HoloToolkit.Unity.InputModule;
 
using UnityEngine.VR.WSA.Input;

public class WorldMNGR : MonoBehaviour {

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
    void Start () {

        Name_Anchor_ZombiSpwaner = "ARZzobiespawner";
        Name_Anchor_Barrier = "ARZbarrier";
        Name_Anchor_PathFinder = "ARZpathfinder";
        List_OBJZombieSpawners = new List<GameObject>();
        List_OBJBarriers = new List<GameObject>();
        WorldAnchorStore.GetAsync(AnchorStoreReady);

    }

    // Update is called once per frame
    void Update () {
        //if (!calledToAnchorStore)
        //{       
        //        calledToAnchorStore = true;
        //        WorldAnchorStore.GetAsync(AnchorStoreReady);           
        //}
    }
    public WorldAnchorStore GetWorldAnchorStore() { return anchorStore; }

    void AnchorStoreReady(WorldAnchorStore store)
    {
        anchorStore = store;

        // list of strings
        List<string> ListID_ZombieSpawners = new List<string>();
        List<string> ListID_Barriers = new List<string>();


        // gather all stored anchors
        string[] ids = anchorStore.GetAllIds();

        for (int index = 0; index < ids.Length; index++)
        {
          
             if (ids[index] == Name_Anchor_PathFinder)
            {
                // if anchor is the pathfinder object
                GameObject obj = Instantiate(OBJ_PathFinder) as GameObject;
                Persisto pscript = obj.GetComponent<Persisto>();
                pscript.SetAnchorStoreName(ids[index]);
                pathFinderPlaced = true;
            }
             else 
            if (ids[index].Contains(Name_Anchor_ZombiSpwaner))
            {
                int thisId = int.Parse(ids[index].Substring(Name_Anchor_ZombiSpwaner.Length));
                if (thisId > ID_Anchor_ZombieSpawner)
                {
                    ID_Anchor_ZombieSpawner = thisId;
                }
                ListID_ZombieSpawners.Add(ids[index]);
            }

            else
            if (ids[index].Contains(Name_Anchor_Barrier))
            {
                int thisId = int.Parse(ids[index].Substring(Name_Anchor_Barrier.Length));
                if (thisId > ID_Anchor_Barrier)
                {
                    ID_Anchor_Barrier = thisId;
                }
                ListID_Barriers.Add(ids[index]);
            }

        }


         
        foreach (string id in ListID_ZombieSpawners)
        {
            List_OBJZombieSpawners.Add(InstantiateObject(OBJ_ZombieSpawner,id));
        }
        foreach (string id in ListID_Barriers)
        {
            List_OBJBarriers.Add(InstantiateObject(OBJ_Barrier, id));
        }


    }

    GameObject InstantiateObject(GameObject obj, string id, bool rotateOnNormals = false, bool keepUpright = false)
    {
        GameObject o = Instantiate(obj) as GameObject;
        Persisto pscript = o.GetComponent<Persisto>();
        pscript.SetAnchorStoreName(id);
        pscript.SetRotateOnNormals(rotateOnNormals);
        pscript.KeepUpright(keepUpright);
        return o;
    }

    GameObject InstantiateObject(GameObject obj, string id, Vector3 position, Quaternion rotation, bool rotateOnNormals = false, bool keepUpright = false)
    {
        GameObject o = Instantiate(obj, position, rotation) as GameObject;
        Persisto  pscript = o.GetComponent<Persisto>();
        pscript.SetAnchorStoreName(id);
        pscript.SetRotateOnNormals(rotateOnNormals);
        pscript.KeepUpright(keepUpright);
        return o;
    }


    public int GetZombieSpawnCount()
    {
        return List_OBJZombieSpawners.Count;
    }

    public int GetBarriersCount()
    {
        return List_OBJBarriers.Count;
    }
 
 
 

 

    public bool isPathFinderPlaced()
    {
        return pathFinderPlaced;
    }

    public int GetZombieSpawnIdNum()
    {
        ID_Anchor_ZombieSpawner++;
        return ID_Anchor_ZombieSpawner;
    }

    public int GetBarrierIdNum()
    {
        ID_Anchor_Barrier++;
        return ID_Anchor_Barrier;
    }



    public void CreateZombieSpawnPoint()
    {
        //Vector3  pos = this.gameObject.transform.InverseTransformPoint( (GazeManager.Instance.GazeOrigin + GazeManager.Instance.GazeNormal * 2.0f));

        Vector3 pos = Camera.main.transform.position + GazeManager.Instance.GazeNormal * 2f;
        string id = Name_Anchor_ZombiSpwaner + GetZombieSpawnIdNum().ToString();
        List_OBJZombieSpawners.Add(InstantiateObject(OBJ_ZombieSpawner, id, pos, Quaternion.identity));
         
    }

    public void CreateBarrier()
    {
        //Vector3 pos = this.gameObject.transform.InverseTransformPoint((GazeManager.Instance.GazeOrigin + GazeManager.Instance.GazeNormal * 2.0f));
        Vector3 pos = Camera.main.transform.position + GazeManager.Instance.GazeNormal * 2f;
        string id = Name_Anchor_Barrier + GetBarrierIdNum().ToString();
        List_OBJBarriers.Add(InstantiateObject(OBJ_Barrier, id, pos, Quaternion.identity));
    }

    public void CreatePathFinder()
    {

        if (pathFinderPlaced)
            return;

        //Vector3 pos = this.gameObject.transform.InverseTransformPoint((GazeManager.Instance.GazeOrigin + GazeManager.Instance.GazeNormal * 2.0f));
        Vector3 pos = Camera.main.transform.position + GazeManager.Instance.GazeNormal * 2f;
        InstantiateObject(OBJ_PathFinder, Name_Anchor_PathFinder, pos, Quaternion.identity);
        pathFinderPlaced = true;
        
    }


    public void Removing(Persisto pScript)
    {
        if (pScript.AnchorStoreBaseName == Name_Anchor_PathFinder)
        {
            pathFinderPlaced = false;
        }
       
        else if (pScript.AnchorStoreBaseName.Contains(Name_Anchor_Barrier))
        {
            List_OBJBarriers.Remove(pScript.gameObject);
        }
        else if (pScript.AnchorStoreBaseName.Contains(Name_Anchor_ZombiSpwaner))
        {
            List_OBJZombieSpawners.Remove(pScript.gameObject);
        } 
    }
    void OnReset()
    {
        ID_Anchor_Barrier = 0;
        ID_Anchor_ZombieSpawner = 0;
        pathFinderPlaced = false;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}
