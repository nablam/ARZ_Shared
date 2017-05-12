using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualLineRenderer : MonoBehaviour {

    Transform parent;
    LineRenderer line  ;
    void Start()
    {
        parent = this.gameObject.transform.parent;
        CONBUG.Instance.LOGit("my dady is " + parent.name);
           line = gameObject.GetComponent<LineRenderer>();
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
    }

    void Update()
    {
        Vector3 targ = RemoteHeadManagerCB.Instance.targetedServerHead;

        line.SetPosition(0, this.transform.position);
        if (targ != null)
        {
            line.SetPosition(1, parent.InverseTransformPoint( targ));
        }
        else
            line.SetPosition(1, this.transform.up * 5f);
    }

     
}
