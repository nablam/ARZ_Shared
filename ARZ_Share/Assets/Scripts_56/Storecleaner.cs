using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VR.WSA.Persistence;
using UnityEngine.VR.WSA;
using System.Collections;
using System.Collections.Generic;

using HoloToolkit.Unity.InputModule;

using UnityEngine.VR.WSA.Input;
using UnityEngine.UI;

public class Storecleaner : MonoBehaviour {

    public LayerMask layerMask = Physics.DefaultRaycastLayers;

    public Text log;
 
 
 

    WorldAnchorStore anchorStore;
   
    void Start()
    {
 
        WorldAnchorStore.GetAsync(AnchorStoreReady);

    }

    // Update is called once per frame
    void Update()
    {
 
    }
 

    void AnchorStoreReady(WorldAnchorStore store)
    {
        anchorStore = store;

      
        // gather all stored anchors
        string[] ids = anchorStore.GetAllIds();

        string s1 = "found " + ids.Length;
        log.text = s1;
        log.text = "cleaning";
        store.Clear();
        string[] ids2 = anchorStore.GetAllIds();

        string s2 = "found " + ids2.Length;
        log.text = s2;

        log.text = "";
        log.text = s1 + " clen " + s2; 

    }
 

}
