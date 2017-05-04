using HoloToolkit.Sharing;
using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class TappedHandlerCB : MonoBehaviour {
    public PrefabSpawnManager spawnManager;

    void Start()
    {
        this.recognizer = new GestureRecognizer();
        this.recognizer.TappedEvent += OnTapped;
        this.recognizer.StartCapturingGestures();
    }
 
    void OnTapped(InteractionSourceKind source, int tapCount, Ray headRay)
    {

        CONBUG.Instance.LOGit("we tapped");
        // If we're networking...
        if (SharingStage.Instance.IsConnected)
        {
            Vector3 headPosition = Camera.main.transform.position;
            Vector3 gazeDirection = Camera.main.transform.forward;

            RaycastHit hitInfo;
            if (Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f))
            {
                CONBUG.Instance.LOGit("we HIT plane");

                var newCubePosition = hitInfo.point;
          
                this.spawnManager.Spawn(
                  new SyncSpawnedObject(),
                  newCubePosition,
                  Quaternion.identity,
                  this.gameObject,
                  "MyCube",
                  false);
            }

    
        }
    }
    GestureRecognizer recognizer;
}
