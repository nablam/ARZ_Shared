using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class EditorSpeechMNGR : MonoBehaviour {

    public GameObject worldManagerObject;
    //public GameObject waveEditorManagerObject;

    WorldMNGR worldManager;
     
    

    KeywordRecognizer keywordRecognizer = null;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

 

    // Use this for initialization
    void Start()
    {
        if (worldManagerObject == null)
        {
            worldManager = FindObjectOfType<WorldMNGR>();
        }
        else
            worldManager = worldManagerObject.GetComponent<WorldMNGR>();
 
        keywords.Add("Reset world", () =>
        {
            // Call the OnReset method on every gameobject.
            Persisto[] objects = (Persisto[])GameObject.FindObjectsOfType(typeof(Persisto));
            foreach (Persisto obj in objects)
            {
                obj.SendMessage("OnRemove");
            }
        });



        keywords.Add("Place spawn", () =>
        {
            worldManager.CreateZombieSpawnPoint();
        });

        keywords.Add("Place barrier", () =>
        {
            worldManager.CreateBarrier();
        });
        keywords.Add("Place Path finder", () =>
        {
            worldManager.CreatePathFinder();
        });
        keywords.Add("Remove", () =>
        {
            var focusObject = GazeManager.Instance.HitObject;
            if (focusObject != null)
            {
                focusObject.SendMessage("OnRemove");
            }
        });

        // Tell the KeywordRecognizer about our keywords.
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

        // Register a callback for the KeywordRecognizer and start recognizing!
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }
}
