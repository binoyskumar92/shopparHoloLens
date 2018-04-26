using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisablezCanvas : MonoBehaviour {
    public static GameObject canvas;
	// Use this for initialization
	void Start () {
        canvas = GameObject.Find("Canvas");
        canvas.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
