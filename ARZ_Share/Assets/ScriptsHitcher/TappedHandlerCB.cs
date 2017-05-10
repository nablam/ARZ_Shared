using HoloToolkit.Sharing;
using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class TappedHandlerCB : MonoBehaviour {
    public PrefabSpawnManager spawnManager;
    GestureRecognizer recognizer;
    void Start()
    {
        this.recognizer = new GestureRecognizer();
        this.recognizer.TappedEvent += OnTapped;
        this.recognizer.StartCapturingGestures();
    }
 
    void OnTapped(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        
        if (SharingStage.Instance.IsConnected)
        {
            placeCrateOnFLoor();
        }
    }
    void placeCrateOnFLoor()
    {
        Vector3 headPosition = Camera.main.transform.position; 
        if (GazeManager.Instance.IsGazingAtObject)
        {
            Vector3 CratePos = this.gameObject.transform.InverseTransformPoint(GazeManager.Instance.HitInfo.point);
            this.spawnManager.Spawn(new SyncSpawnedObject(),
              CratePos, Quaternion.identity, null,
              this.gameObject,
              "MyCrate",
              true);
        }
    }
}
