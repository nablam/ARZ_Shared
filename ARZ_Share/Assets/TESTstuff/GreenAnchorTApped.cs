using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenAnchorTApped : MonoBehaviour {

    public GameObject HL;
    public GameObject REL_HL;
    Vector3 VectorFromHL;
    GreenAnchorSpawnManager spawnManager;

    void Start () {
        spawnManager = this.gameObject.GetComponent<GreenAnchorSpawnManager>();
        VectorFromHL = HL.transform.TransformDirection(Vector3.forward) ;
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 forward = HL.transform.TransformDirection(Vector3.forward) * 2;
        Debug.DrawRay(HL.transform.position, forward, Color.green);

        REL_HL.transform.position = HL.transform.position+forward;
        if (Input.GetKeyDown(KeyCode.A)) {
            HitcherWay();
        }
    }

    void HitcherWay()
    {
        var newCubePosition =
        this.gameObject.transform.InverseTransformPoint(
          (HL.transform.position + VectorFromHL * 2.0f));

        // Use the span manager to span a 'SyncSpawnedObject' at that position with
        // some random rotation, parent it off our gameObject, give it a base name (MyCube)
        // and do not claim ownership of it so it stays behind in the scene even if our
        // device leaves the session.
        if (spawnManager != null)
            this.spawnManager.Spawn(newCubePosition, Random.rotation, this.gameObject, "MyCube", false);
        else
            Debug.Log("no spawn manger");
    }
}
