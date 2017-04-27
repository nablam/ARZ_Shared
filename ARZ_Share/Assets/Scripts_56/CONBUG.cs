using System;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Sharing;
using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Unity.InputModule;
using UnityEngine.VR.WSA.Input;
using UnityEngine.UI;
using System.Text;


public class CONBUG : Singleton<CONBUG>{

    public TextMesh tm;
    int linenum = 0;
    public void LOGit(string str) {
        linenum++;
        tm.text += Environment.NewLine;
        tm.text += linenum+"|"+str;
    }

    public void LOGit(string str,string who)
    {
        linenum++;
        tm.text += Environment.NewLine;
        tm.text += linenum + "|("+ who +")" + str;
    }

}
