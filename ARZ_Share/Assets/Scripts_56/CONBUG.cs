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

    public void LOGitError(string argstr) {
        LOGit(argstr, "ERROR!");
    }
    public void LOGitFormat(string ArgFormat, string argstr) {
        string s = string.Format(ArgFormat, argstr);
        LOGit(s);

    }
    public void LOGitWarning(string argstr) {
        LOGit(argstr, "WARNING!");
    }
    public void LOGit(string str,string who)
    {
        linenum++;
        tm.text += Environment.NewLine;
        tm.text += linenum + "|("+ who +")" + str;
    }

}
