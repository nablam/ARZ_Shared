// @Author Nabil Lamriben ©2017
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows.Speech;

public class SceneSpeechMNGR : MonoBehaviour {


    KeywordRecognizer keywordRecognizer = null;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();
    public MasterGameDataSetup mds;

    // Use this for initialization
    void Start()
    {

        keywords.Add("bang bang", () =>
        {
            mds.BANGBANG();
        });

        keywords.Add("show me the money", () =>
        {
            mds.ShowList();
        });

        keywords.Add("other room", () =>
        {
            SceneManager.LoadScene("otherscene");
        });

        keywords.Add("replace cubes", () =>
        {
            mds.replaceCrateswithbox();
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
