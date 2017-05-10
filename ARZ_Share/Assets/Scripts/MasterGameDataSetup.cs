using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterGameDataSetup : MonoBehaviour {

    bool IsServer;
    
    public bool getIsServer() { return IsServer; }
    public void SetIsServer(bool argserver) { IsServer = argserver; }

    public List<GameObject> cratesPlaced;
    private void Awake()
    {
        cratesPlaced = new List<GameObject>();
    }

    public void ShowList() {
        foreach (GameObject go in cratesPlaced) {
            CONBUG.Instance.LOGit(go.name);
            CONBUG.Instance.LOGit(go.transform.position.ToString());
        }
    }

    public void replaceCrateswithbox() {

        foreach (GameObject go in cratesPlaced) {
            
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            cube.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, go.transform.position.z);
        }

    }


    bool started = false;
    IEnumerator init() {
        yield return new WaitForSeconds(5);
        IsServer = RemoteHeadManagerCB.Instance.GetAmServer();
        started = true;

    }
    void Start () {
		
	}

    Vector3 target;
	void Update () {
        if (started) {
            target = RemoteHeadManagerCB.Instance.targetedServerHead;
        }
	}
    public void BANGBANG() {
        DrawLazer(this.transform.position, target);

    }
    void DrawLazer(Vector3 start, Vector3 end)
    {
         
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = RemoteHeadManagerCB.Instance.lazorMat;
        lr.startColor = Color.red;
        lr.endColor = Color.red;
        lr.startWidth = 0.01f;
        lr.endWidth = 0.01f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        //GameObject.Destroy(myLine, 20f);
    }
}
