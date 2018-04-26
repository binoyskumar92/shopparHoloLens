using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;

public class SetCurrentItem : MonoBehaviour {

    // Use this for initialization
    private string host = "http://shoppar-env.us-east-2.elasticbeanstalk.com/";
    public TextMesh currentItemText;
    public Sprite purchased;
    public Sprite notPurchased;
    public static CheckListItem[] checkListItems;
    void Start () {
      
        StartCoroutine(getChecklistItems());
    }
    private IEnumerator getChecklistItems()
    {
        string uri = host + "checklist";
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While getting checklist items: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);

            parseCheckListResponse(uwr.downloadHandler.text);
        }
    }
    public class CheckListItem
    {
        public string name;
        public string isPurchased;
        public string id;
    }

    private void parseCheckListResponse(string JsonInput)
    {
        var result = JSON.Parse(JsonInput);
        checkListItems = new CheckListItem[result.Count];
        for (int i = 0; i < result.Count; i++)
        {
            checkListItems[i] = new CheckListItem();
            checkListItems[i].id = result[i]["product"]["_id"] + "";
            checkListItems[i].name = result[i]["product"]["name"] + "";
            checkListItems[i].isPurchased = result[i]["isPurchased"] + "";
        }
        Debug.Log("Checklist item name 1: "+result[0]["product"]["name"]);
        string id= result.Count > 0 ? result[0]["product"]["_id"] + "" : "";
        //set currentItem id for reference in speech detector class
        SpeechDetector.currentItemId = id;
        string name = result.Count > 0 ? result[0]["product"]["name"]+"" : "No item in checklist";
        setCurrentItemText(name);
        if (result.Count > 0)
        {
            setPurchasedImage(result[0]["isPurchased"] + "");
        }
       
        
    }

    private void setPurchasedImage(string isPurchased)
    {
        if (isPurchased.Equals("0"))
        {
            gameObject.GetComponentInChildren<Image>().sprite = notPurchased;
        }
        else
        {
            gameObject.GetComponentInChildren<Image>().sprite = purchased;
        }
    }

    private void setCurrentItemText(string name)
    {
        currentItemText.text = name;
    }

    
}
