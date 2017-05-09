using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gridmaker : MonoBehaviour {

    public GameObject gc;
   
    public GameObject OB_cursorRed;
    public GameObject OB_anchorGreen;
    public GameObject OB_hl;

    public GameObject RL_anchGreen;
    public GameObject RL_hl;

    GameObject OB_hlINST;

    Vector3 VectorFromHL;

    void Start() {
        MakeGrid();

        //VectorFromHL= OB_hl.transform.TransformDirection(Vector3.forward) * 5;
      //  PlaceHlRel(VectorFromHL);
       // WOW();
    }

    void MakeGrid()
    {
        for (int z = -10; z < 11; z++)
        {
            for (int x = -10; x < 11; x++)
            {

                GameObject go = Instantiate(gc, new Vector3(x, 0, z), Quaternion.identity) as GameObject;
                go.GetComponentInChildren<TextMesh>().text = "(" + x + "," + z + ")";
                go.transform.parent = this.transform;
            }
        }
    }
    void Update()
    {
      //  Vector3 forward = OB_hl.transform.TransformDirection(Vector3.forward) * 5;
      //  Debug.DrawRay(OB_hl.transform.position, forward, Color.green);
    }

    void PlaceHlRel(Vector3 here) {

        GameObject go = Instantiate(RL_hl, here, Quaternion.identity) as GameObject;
        go.transform.parent = OB_hl.transform;
    }

    void WOW() {
        // Vector3 newCubePosition2 =OB_anchorGreen.transform.InverseTransformPoint((OB_hl.transform.position + VectorFromHL));
        Vector3 newCubePosition2 =((OB_anchorGreen.transform.position + VectorFromHL));
        Vector3 offset =  OB_hl.transform.position- OB_anchorGreen.transform.position;
        GameObject go = Instantiate(RL_anchGreen, newCubePosition2+ offset, Quaternion.identity) as GameObject;
        go.transform.parent = OB_anchorGreen.transform;

    }
}
