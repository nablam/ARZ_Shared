using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tappedRel : MonoBehaviour {
    public GameObject kid;
    Vector3 Invers;

    void Start () {

        Debug.Log("yooo" + kid.transform.localPosition);

    }

    // Update is called once per frame
    void Update () {

        var local1 = this.gameObject.transform.InverseTransformPoint(kid.transform.position);
        Debug.DrawRay(this.gameObject.transform.position, local1, Color.green);
        var local2 = kid.transform.position - this.gameObject.transform.position;
        Debug.DrawRay(this.gameObject.transform.position, local2, Color.red);
    }
}
