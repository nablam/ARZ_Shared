using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualParentLink : MonoBehaviour {

	// Use this for initialization
	void Start () {
        start = this.transform.position;
        _col = Color.red;
        if (this.transform.parent != null) hasparent = true;
        if (hasparent) {
            end = this.transform.parent.transform.position;
        }
        else
            end = transform.TransformDirection(Vector3.forward) * 10;

        DrawLineRED();
    }
	
	 
    private Vector3 start;
    private Vector3 end;
    private float lineWidth = 1f;
    bool hasparent;
   
    private Color _col;

    void DrawLineRED()
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(_col, _col);
        lr.SetWidth(0.01f, 0.01f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, 20f);
    }
 
    void drawQ(){
        Vector3 normal = Vector3.Cross(start, end);
        Vector3 side = Vector3.Cross(normal, end - start);
        side.Normalize();
        Vector3 a = start + side * (lineWidth / 2);
        Vector3 b = start + side * (lineWidth / -2);
        Vector3 c = end + side * (lineWidth / 2);
        Vector3 d = end + side * (lineWidth / -2);
    }
}
