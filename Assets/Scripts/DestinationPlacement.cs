using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class DestinationPlacement : MonoBehaviour {

    void Start()

    {

       
    }

   
    public void OnInputClicked(InputClickedEventData eventData)

    {

        if (GazeManager.Instance.IsGazingAtObject)

        {

            var hitInfo = GazeManager.Instance.HitInfo;

            transform.position = hitInfo.point + transform.localScale.y * Vector3.up;

        }

    }
}
