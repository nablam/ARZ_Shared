using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepMeAlive : MonoBehaviour {

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
