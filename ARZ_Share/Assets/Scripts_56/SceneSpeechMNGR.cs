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



    // Use this for initialization
    void Start()
    {
    

        keywords.Add("Main Menu", () =>
        {
            // load main menu scene
            SceneManager.LoadScene("main56");
        });
        keywords.Add("Editor", () =>
        {
            // load main menu scene
            SceneManager.LoadScene("Edit56");
        });
        keywords.Add("Scanner", () =>
        {
            // load main menu scene
            SceneManager.LoadScene("Scan56");
        });
        keywords.Add("Start Game", () =>
        {
            // load main menu scene
            SceneManager.LoadScene("Game56");
        });
        keywords.Add("Connect Play", () =>
        {
            // load main menu scene
            SceneManager.LoadScene("Multi56");
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

    //int cnt = 0;
    //private void Update()
    //{
    //    cnt++;
    //    if(cnt%11 == 0)
    //    CONBUG.Instance.LOGit(""+cnt);
    //}

}
