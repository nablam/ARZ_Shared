using System;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;

//*************************************************
//the script is a ‘fallback handler’ for when the user air-taps(or clicks) 
//and there isn’t an object in focus which handles that click.
//The handler then just creates an instance of the prefab at 1.5 metres in front of the user and I’ve configured the prefab in the editor to be my Xmas present;
//*************************************************

public class PalceholderCB : MonoBehaviour, IInputClickHandler
{

    public Transform prefab;

    private void Start()
    {
        InputManager.Instance.PushFallbackInputHandler(this.gameObject);
    }


    bool loaded;
    int count;

    // Update handler which attempts to wait until the WorldAnchorManager has loaded up its store of anchors at the start of the application and it then uses any IDs stored in there to recreate the objects that they represent at the place where they were previously positioned
    bool doneOnce = false;
    private void Update()
    {
        if (!doneOnce)
        {
            CONBUG.Instance.LOGit("WorldAnchorManager NOT Store Ready");
            doneOnce = true;
        }


        if (!this.loaded && (WorldAnchorManager.Instance.AnchorStore != null))
        {
            CONBUG.Instance.LOGit("WorldAnchorManager Store Ready");
            var ids = WorldAnchorManager.Instance.AnchorStore.GetAllIds();

            // NB: I'm assuming that the ordering here is either preserved or
            // maybe doesn't matter.
            foreach (var id in ids)
            {
                var instance = Instantiate(this.prefab);
                WorldAnchorManager.Instance.AttachAnchor(instance.gameObject, id);
            }
            this.loaded = true;
            this.count = ids.Length;
        }
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {

        //*************************************************
        Transform instance = Instantiate(prefab);
        instance.gameObject.transform.position = GazeManager.Instance.GazeOrigin + GazeManager.Instance.GazeNormal * 1.5f;
        //*************************************************  
        TapToPlaceCB tapToPlace = instance.gameObject.AddComponent<TapToPlaceCB>();
        tapToPlace.SavedAnchorFriendlyName = (++this.count).ToString();
    }
}
