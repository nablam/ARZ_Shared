using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenAnchorSpawnManager : MonoBehaviour {

    public GameObject RL_Anchor;
	void Start () {
		
	}

    public bool Spawn( Vector3 localPosition, Quaternion localRotation, GameObject parent, string baseName, bool isOwnedLocally)
    {
        return Spawn(RL_Anchor, localPosition, localRotation, null, parent, baseName, isOwnedLocally);
    }

    public bool Spawn(GameObject OBdataModel, Vector3 localPosition, Quaternion localRotation, Vector3? localScale, GameObject parent, string baseName, bool isOwnedLocally)
    {
     

        if (OBdataModel == null)
        {
            Debug.LogError("Can't spawn an object: dataModel argument is null.");
            return false;
        }

        if (parent == null)
        {
            parent = gameObject;
            Debug.Log("dady=" + parent.name);
        }
        else
            Debug.Log(" No parent passed");
        // Validate that the prefab is valid
        GameObject prefabToSpawn = OBdataModel;
        if (!prefabToSpawn)
        {
            Debug.Log("nothing to spaewn"  );
            return false;
        }



        //will call Spawnmanager OnObjectAdded() ------------------------------------------------UP which calls this InstantiateFromNetwork
        InstantiateFromNetwork(OBdataModel, localPosition, parent);

        return true;
    }

    void InstantiateFromNetwork(GameObject spawnedObject,Vector3 localPosition, GameObject argParent)
    {
        GameObject prefab = spawnedObject;
        if (!prefab)
        {
            Debug.Log("no prefab");
            return;
        }

        // Find the parent object
        GameObject parent = null;
        if (argParent != null)
        {
            parent = argParent;

            if (parent == null)
            {
                Debug.LogErrorFormat("Bad skill tree Parent object ");
                return;
            }
            else
            {
                Debug.Log("Parent object =" + parent.name);

            }
        }

        CreatePrefabInstance( prefab, localPosition, parent, spawnedObject.name);
    }

     GameObject CreatePrefabInstance(  GameObject prefabToInstantiate,Vector3 localPosition, GameObject parentObject, string objectName)
    {
        GameObject instance = Instantiate(prefabToInstantiate, localPosition, Quaternion.identity);

        //            GameObject instance = Instantiate(prefabToInstantiate, dataModel.Transform.Position.Value, prefabToInstantiate.transform.rotation);

       
        instance.transform.SetParent(parentObject.transform, false);
        instance.gameObject.name = objectName;

        
        return instance;
    }

}
