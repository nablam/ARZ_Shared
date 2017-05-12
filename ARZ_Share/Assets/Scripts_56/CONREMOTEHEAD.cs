using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Sharing;
using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Unity.InputModule;
using UnityEngine.VR.WSA.Input;
using UnityEngine.UI;
using System.Text;
public class CONREMOTEHEAD : Singleton<CONREMOTEHEAD>
{

    // public TextMesh remoteheadTxt; public TextMesh remoteheadCONVERTED;
    // public TextMesh sendingmyhead; public TextMesh sendingmyheadCONVERTED;

    public Text MYheadtransLocal;
    public Text MYheadtransWorld;
    public Text parentInv;
    public Text myHead_sentV3;


    public Text headPosReceivedmsg;
    public Text headPosWORLD;
    public Text serverheadLocal;
    public Text serverheadWORLD;
    public GameObject canv;
    public void LOG_sendmyheadTransform(Transform argHEadInUpdateOfRemoteHeadMNG, Transform parentINV , Vector3 senthead)
    {
        MYheadtransLocal.text = argHEadInUpdateOfRemoteHeadMNG.localPosition.ToString();
        MYheadtransWorld.text = argHEadInUpdateOfRemoteHeadMNG.position.ToString() + "<-inv";
        parentInv.text = parentINV.name+ "\n L" + parentINV.localPosition.ToString()+ "\n W" + parentINV.position.ToString();
        myHead_sentV3.text = senthead.ToString();
    }
    bool ison = true;
    public void FlipLog() {
        canv.SetActive(!canv.activeSelf);

    }
    public void LOG_ReceivedHead(Vector3 headPos, Vector3 BUTGLOBAL) {
        headPosReceivedmsg.text = headPos.ToString();
        headPosWORLD.text = BUTGLOBAL.ToString();
    }

    public void LOG_SERVERHEADTARG(bool amiserver, Vector3 targ) {
        if (amiserver)
        {
            if (targ!= null) { serverheadLocal.text = "Isvr targ=" + targ.ToString(); }
            else
            { serverheadLocal.text = "Isvr targ= NULL"; }
        }
        else {
            if (targ != null)
            {
                serverheadLocal.text = "Icli targ=" + targ.ToString();
            }
            else { { serverheadLocal.text = "Icli targ= NULL"; }}
            //   serverheadWORLD.text = "Icli targ=" + targ.position.ToString();
        }

    }
   void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
