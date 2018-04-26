/*==============================================================================
Copyright (c) 2017 PTC Inc. All Rights Reserved.

Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Vuforia;
using SimpleJSON;
using System;
using TMPro;
using UnityEngine.UI;

/// <summary>
///     A custom handler that implements the ITrackableEventHandler interface.
/// </summary>
public class DefaultTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{
    #region PRIVATE_MEMBER_VARIABLES

    protected TrackableBehaviour mTrackableBehaviour;
    private bool isDataLoaded=false;
    private string host = "http://shoppar-env.us-east-2.elasticbeanstalk.com/";

    #endregion // PRIVATE_MEMBER_VARIABLES

    #region UNTIY_MONOBEHAVIOUR_METHODS

    protected virtual void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
    }

    #endregion // UNTIY_MONOBEHAVIOUR_METHODS

    #region PUBLIC_METHODS

    /// <summary>
    ///     Implementation of the ITrackableEventHandler function called when the
    ///     tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged(
        TrackableBehaviour.Status previousStatus,
        TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
            OnTrackingFound();
        }
        else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                 newStatus == TrackableBehaviour.Status.NOT_FOUND)
        {
            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
            OnTrackingLost();
        }
        else
        {
            // For combo of previousStatus=UNKNOWN + newStatus=UNKNOWN|NOT_FOUND
            // Vuforia is starting, but tracking has not been lost or found yet
            // Call OnTrackingLost() to hide the augmentations
            OnTrackingLost();
        }
    }

    #endregion // PUBLIC_METHODS

    #region PRIVATE_METHODS

    protected virtual void OnTrackingFound()
    {
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

        // Enable rendering:
        foreach (var component in rendererComponents)
            component.enabled = true;

        // Enable colliders:
        foreach (var component in colliderComponents)
            component.enabled = true;

        // Enable canvas':
        foreach (var component in canvasComponents)
            component.enabled = true;
        Debug.Log("Trackable "+mTrackableBehaviour.TrackableName+" found");
        string id = mTrackableBehaviour.TrackableName;

        //dummy products for test
        if (mTrackableBehaviour.TrackableName == "ARONEIMG")
        {
           // TextMesh textObject = GameObject.Find("Text1").GetComponent<TextMesh>();
           // textObject.text = mTrackableBehaviour.TrackableName;
            string urlAlt = "http://shoppar-env.us-east-2.elasticbeanstalk.com/recommendation/productId/5ab59a66734d1d57bac465a5";
            StartCoroutine(getRequest("5ab59a66734d1d57bac465a5"));
            StartCoroutine(getAlternativesRequest(urlAlt));

        }
        else if (mTrackableBehaviour.TrackableName == "ketchup")
        {
           // TextMesh textObject = GameObject.Find("Text2").GetComponent<TextMesh>();
           // textObject.text = mTrackableBehaviour.TrackableName;
            string urlAlt = "http://shoppar-env.us-east-2.elasticbeanstalk.com/recommendation/productId/5ab59a66734d1d57bac465a5";
            StartCoroutine(getRequest("5ab59a66734d1d57bac465a5"));
            StartCoroutine(getAlternativesRequest(urlAlt));
        }
        else
        {
            //caching responses for products so that second time network calls can be avoided
            string urlAlt = "http://shoppar-env.us-east-2.elasticbeanstalk.com/recommendation/productId/"+ mTrackableBehaviour.TrackableName;

            SpeechDetector.currentScannedItemId = id; // to track which item is currently scanne din speech detector class
            if (!SpeechDetector.products.ContainsKey(id))
            {
                
                StartCoroutine(getRequest(id));
            }
            else
            {
                createNutritionInfo(SpeechDetector.products[id]);
            }
            
            StartCoroutine(getAlternativesRequest(urlAlt));
        }


    }

    public IEnumerator getAlternativesRequest(string uri)
    {
        

        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            //clear the list
            ShopScrollList tempShop = GameObject.Find("Content").GetComponent<ShopScrollList>();
            tempShop.itemList.Clear();
            var myStr= "{\"alternatives\":"+uwr.downloadHandler.text+"}";

            //Find the correct alternatives
            var N = JSON.Parse(myStr);
            var myAlt = N["alternatives"];
            for (int i=0;i<myAlt.Count;i++)
            {
                var currentAlt = myAlt[i];
                var prodName = currentAlt["name"];
                var prodPrice = (float)currentAlt["price"];
                var prodId = currentAlt["_id"];
                var prodImageUrl="";
                var prodImgArr = myAlt[i]["images"];
                for (int j=0;j< prodImgArr.Count;j++)
                {
                    if (prodImgArr[j]["entityType"] == "PRIMARY")
                    {
                        prodImageUrl = prodImgArr[j]["largeImage"];
                    }
                }
                Debug.Log("Element: " + prodName);
                Debug.Log("Element: " + prodPrice);
                Debug.Log("Element: " + prodImageUrl);
                Debug.Log("Element: " + prodId);
                StartCoroutine( AddAltItemToShop( prodId,prodName, prodPrice, prodImageUrl, myAlt.Count, tempShop));
            }      // versionString will be a string containing "1.0"
        }
    }
    public IEnumerator AddAltItemToShop(string prodId,string prodName, float prodPrice,string prodImageUrl,int altCount, ShopScrollList tempShop)
    {
        
        Sprite imgIcon;
        WWW www = new WWW(prodImageUrl);
        yield return www;

        imgIcon = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));

        Item tempItem = new Item(prodId,prodName, imgIcon, prodPrice);
        tempShop.myAltDisplay.text = "Alternatives:" + altCount.ToString();
        tempShop.itemList.Add(tempItem);
        tempShop.RefreshDisplay();
    }
    public IEnumerator getRequest(string id)
    {
        string uri = host+"product/id/"+id;
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();
        
        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
            SpeechDetector.products[id] = uwr.downloadHandler.text + "";
            createNutritionInfo(uwr.downloadHandler.text);
        }
    }

    private void createNutritionInfo(String JsonInput)
    {
      
        var result = JSON.Parse(JsonInput);
        TextMeshProUGUI valText;
        GameObject keys, row;
        string[] aKeys = new string[] { "Calories", "Fat", "Cholesterol", "Sodium", "Carbohydrate", "Protein", "Fiber"};

        setCanvasVisibility(true);

        keys = GameObject.Find("Keys"); //Find keys empty object with vertical layout
        row = GameObject.Find("Name");// Load prefab from resources fiolder
        valText = (TextMeshProUGUI)row.transform.Find("value").gameObject.GetComponent<TextMeshProUGUI>();
        valText.SetText(result["name"].Value.ToString());

        row = GameObject.Find("Price");// Load prefab from resources fiolder
        valText = (TextMeshProUGUI)row.transform.Find("value").gameObject.GetComponent<TextMeshProUGUI>();
        valText.SetText(result["price"].Value.ToString()+" $");

        row = GameObject.Find("Calories");// Load prefab from resources fiolder
        valText = (TextMeshProUGUI)row.transform.Find("value").gameObject.GetComponent<TextMeshProUGUI>();
        valText.SetText(result["nutrition"]["calories"].Value.ToString());


        row = GameObject.Find("Fat");
        valText = (TextMeshProUGUI)row.transform.Find("value").gameObject.GetComponent<TextMeshProUGUI>();
        valText.SetText(result["nutrition"]["fat"]["total"].Value.ToString());

        row = GameObject.Find("Cholesterol");    
        valText = (TextMeshProUGUI)row.transform.Find("value").gameObject.GetComponent<TextMeshProUGUI>();
        valText.SetText(result["nutrition"]["cholesterol"].Value.ToString());

        row = GameObject.Find("Sodium");
        valText = (TextMeshProUGUI)row.transform.Find("value").gameObject.GetComponent<TextMeshProUGUI>();
        valText.SetText(result["nutrition"]["sodium"].Value.ToString());

        row = GameObject.Find("Carbohydrate");
        valText = (TextMeshProUGUI)row.transform.Find("value").gameObject.GetComponent<TextMeshProUGUI>();
        valText.SetText(result["nutrition"]["carbohydrate"].Value.ToString());

        row = GameObject.Find("Protein");
        valText = (TextMeshProUGUI)row.transform.Find("value").gameObject.GetComponent<TextMeshProUGUI>();
        valText.SetText(result["nutrition"]["protein"].Value.ToString());

        row = GameObject.Find("Fiber");
        valText = (TextMeshProUGUI)row.transform.Find("value").gameObject.GetComponent<TextMeshProUGUI>();
        valText.SetText(result["nutrition"]["fiber"].Value.ToString());

        
        isDataLoaded = true;
      
    }

    private void setCanvasVisibility(bool isShown)
    {
        DisablezCanvas.canvas.SetActive(isShown);
    }

    private void killAllChildren()
    {
        GameObject keys = GameObject.FindWithTag("canvastag");
        if (keys != null)
        {

            int childs = keys.transform.childCount;
             Debug.LogError("Number of canvas child:" + childs.ToString());
            for (int i = childs - 1; i >= 0; i--)
            {
                GameObject.Destroy(keys.transform.GetChild(i).gameObject);
            }
            //DestroyPanel();
        }
        
    }

    private void DestroyPanel()
    {
        if (GameObject.Find("Canvas(Clone)") != null)
        {
            Destroy(GameObject.Find("Canvas(Clone)").gameObject);
        }
    }

    protected virtual void OnTrackingLost()
    {
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);
        if (isDataLoaded)
        {
            setCanvasVisibility(false);
            isDataLoaded = false;
            SpeechDetector.currentScannedItemId = "";
        }
       
        // Disable rendering:
        foreach (var component in rendererComponents)
            component.enabled = false;

        // Disable colliders:
        foreach (var component in colliderComponents)
            component.enabled = false;

        // Disable canvas':
        foreach (var component in canvasComponents)
            component.enabled = false;

    }

    #endregion // PRIVATE_METHODS
}
