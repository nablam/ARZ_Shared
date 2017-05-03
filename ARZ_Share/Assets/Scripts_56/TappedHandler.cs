using HoloToolkit.Sharing;
using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class TappedHandler : MonoBehaviour
{
    public PrefabSpawnManager spawnManager;
    protected SpatialMappingManager spatialMappingManager;

    void Start()
    {
        this.recognizer = new GestureRecognizer();
        this.recognizer.TappedEvent += OnTapped;
        this.recognizer.StartCapturingGestures();
    }

    void Update()
    {
        // Do a raycast into the world that will only hit the Spatial Mapping mesh.



    }

    void OnTapped(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        // If we're networking...
        if (SharingStage.Instance.IsConnected)
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

                this.spawnManager.Spawn(
           new SyncSpawnedObject(),
           hitInfo.point,
           Random.rotation,
           this.gameObject,
           "MyCube",
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
    GestureRecognizer recognizer;
}