using HoloToolkit.Sharing;
using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class TappedHandlerCB : MonoBehaviour {
    public PrefabSpawnManager spawnManager;
    public GameObject C1;
    public GameObject C2;
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
            CONBUG.Instance.LOGit("we tapped2");

            betterway();
        }
    }
    void betterway()
    {

        Vector3 headPosition = Camera.main.transform.position; //GazeManager.Instance.GazeOrigin;// Camera.main.transform.position;
        // Vector3 gazeDirection = Camera.main.transform.forward;

        // RaycastHit hitInfo;
        if (GazeManager.Instance.IsGazingAtObject)
        {
            CONBUG.Instance.LOGit("we HIT plane YI");
 
           // var VectorFromHL = GazeManager.Instance.HitInfo.point + Camera.main.transform.position;
              var newCubeBOZ = this.gameObject.transform.InverseTransformPoint(GazeManager.Instance.HitInfo.point);

           // var newCubeBOZ = C2.transform.position;

            //Vector3 newCubePosition2 = ((this.gameObject.transform.position + VectorFromHL));
            //Vector3 offset = headPosition - this.gameObject.transform.position;
            //Vector3 final = newCubePosition2 + offset;

            this.spawnManager.Spawn(new SyncSpawnedObject(),
              newCubeBOZ, Quaternion.identity, null,
              this.gameObject,
              "MyCube",
              true);

        }



    }
    void myway() {

        Vector3 headPosition = GazeManager.Instance.GazeOrigin;// Camera.main.transform.position;
        // Vector3 gazeDirection = Camera.main.transform.forward;

        // RaycastHit hitInfo;
        if (GazeManager.Instance.IsGazingAtObject)
        {
            CONBUG.Instance.LOGit("we HIT plane YI");

            // Make a new cube that is 2m away in direction of gaze but then get that position
            // relative to the object that we are attached to (which is world anchor'd across
            // our devices).
            // var newCubePosition2 = this.gameObject.transform.InverseTransformPoint((GazeManager.Instance.GazeOrigin - GazeManager.Instance.HitInfo.point));

           var VectorFromHL = GazeManager.Instance.HitInfo.point + Camera.main.transform.position;
            // var newCubeBOZ = this.gameObject.transform.InverseTransformPoint((GazeManager.Instance.GazeOrigin + VectorFromHL));
            
            var newCubeBOZ = C2.transform.position;

            //Vector3 newCubePosition2 = ((this.gameObject.transform.position + VectorFromHL));
            //Vector3 offset = headPosition - this.gameObject.transform.position;
            //Vector3 final = newCubePosition2 + offset;

            this.spawnManager.Spawn(new SyncSpawnedObject(),
              newCubeBOZ, Quaternion.identity, null,
              this.gameObject,
              "MyCube",
              true);

        }
    }
    void HitcherWay() {
        var newCubePosition =
        this.gameObject.transform.InverseTransformPoint(
          (GazeManager.Instance.GazeOrigin + GazeManager.Instance.GazeNormal * 2.0f));

        // Use the span manager to span a 'SyncSpawnedObject' at that position with
        // some random rotation, parent it off our gameObject, give it a base name (MyCube)
        // and do not claim ownership of it so it stays behind in the scene even if our
        // device leaves the session.
        this.spawnManager.Spawn(
          new SyncSpawnedObject(),
          newCubePosition,
          Random.rotation,
          this.gameObject,
          "MyCube",
          false);
    }
    GestureRecognizer recognizer;
}
