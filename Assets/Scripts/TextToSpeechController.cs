using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextToSpeechController : MonoBehaviour {

    public static GameObject soundManager;
    public static TextToSpeech tts;

    // Use this for initialization
    void Start () {
        soundManager = GameObject.Find("Audiomanager");
        tts = soundManager.GetComponent<TextToSpeech>();
        tts.Voice = TextToSpeechVoice.Zira;
        tts.StartSpeaking("Welcome to shoppAR, the future of shopping!. If any assistance is required, you simply say HELP");
        
    }

    public static void speakText(string text)
    {
        tts.StartSpeaking(text);
    }
}
