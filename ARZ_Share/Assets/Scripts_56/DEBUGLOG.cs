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

public class DEBUGLOG : Singleton<DEBUGLOG> {

   // int _lineNumber = 0;
    int _GREEN_LINECNT = 0;
   // int _maxlines = 18;
    int _GREEN_MAXLINES = 18;
   // Queue<string> _debugLines = new Queue<string>();
    Queue<string> _GREEN_Q = new Queue<string>();
    //public Text debugscreen;
    public Text GREENSCREEN;

    public bool DisplayOn = true;

    public void LOG2(string str)
    {
        AddLineToQue2(str);
        if(DisplayOn)
        dumpQueToScreen2();
    }
  
 

    void AddLineToQue2(string str)
    {
        _GREEN_LINECNT++;
        CheckOverflow2();
        str = _GREEN_LINECNT + " " + str;
        _GREEN_Q.Enqueue(str);
    }
    

    void CheckOverflow2()
    {
        if (_GREEN_Q.Count >= _GREEN_MAXLINES)
        {
            _GREEN_Q.Dequeue();
        }
    }

  

    void dumpQueToScreen2()
    {
        StringBuilder sb = new StringBuilder();

        foreach (string str in _GREEN_Q)
        {
            sb.Append(str);
            sb.Append(Environment.NewLine);
        }

        GREENSCREEN.text = "";
        GREENSCREEN.text = sb.ToString();
    }
}
