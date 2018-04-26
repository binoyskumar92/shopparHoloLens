using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class CanvasPopUp : MonoBehaviour, ITrackableEventHandler
{

    private TrackableBehaviour mTrackableBehaviour;

    private bool mShowGUICanvas = false;
    public GameObject gScreen;

    // Use this for initialization
    void Start()
    {
        
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            mShowGUICanvas = true;
            Debug.Log("mShowGUICanvas: " + mShowGUICanvas.ToString());
        }
        else
        {
            mShowGUICanvas = false;
            Debug.Log("mShowGUICanvas: " + mShowGUICanvas.ToString());
        }
        gScreen.SetActive(mShowGUICanvas);
    }
}
