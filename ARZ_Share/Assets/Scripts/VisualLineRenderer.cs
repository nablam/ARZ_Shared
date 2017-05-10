using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualLineRenderer : MonoBehaviour {

    //Addes a line to this object


    public Color c1 = Color.yellow;
    public Color c2 = Color.red;
   // public int lengthOfLineRenderer = 20;
    LineRenderer line  ;
    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
        //coolsetup();
    }

    void Update()
    {
        line.SetPosition(0, this.transform.position);
        if (this.gameObject.transform.parent != null)
        {
            line.SetPosition(1, this.gameObject.transform.parent.position);
        }
        else
            line.SetPosition(1, this.transform.up * 5f);
    }
    public Material material;
    private Material[] mats;
    public Shader myshade;
    void coolsetup()
    {
          this.gameObject.GetComponent<Renderer>().material.color=Color.black;
        mats = new Material[1];
        mats[0] = material;
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Resources/shadyshader"));// lineRenderer.material = new Material(myshade); // lineRenderer.material = new Material(Shader.Find("Particles/Additive")); ;//this.gameObject.GetComponent<Renderer>().material; 

         lineRenderer.widthMultiplier = 0.2f;
        lineRenderer.positionCount = 2;

        // A simple 2 color gradient with a fixed alpha of 1.0f.
        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
        lineRenderer.colorGradient = gradient;
    }
    void cool()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>(); 
        lineRenderer.SetPosition(0, this.transform.position);
        if (this.gameObject.transform.parent != null) {
            lineRenderer.SetPosition(1, this.gameObject.transform.parent.position);
        }
        else
            lineRenderer.SetPosition(1, this.transform.up * 5f);


    }


 

    void SetupLine()
    {
        //line.sortingLayerName = "OnTop";
        //line.sortingOrder = 5;
        //line.SetVertexCount(2);
        //line.SetPosition(0, startingPoint);
        //line.SetPosition(1, endPoint);
        //line.SetWidth(0.5f, 0.5f);
        //line.useWorldSpace = true;
       // line.material = LineMaterial;
    }
}
