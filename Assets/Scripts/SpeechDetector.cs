using HoloToolkit.Unity.InputModule;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class SpeechDetector : MonoBehaviour, ISpeechHandler
{
    public GameObject destination;
    public int counter = 0;
    public int altCounter = 0;
    public int checkListCounter = 0;
    public static string currentItemId="";
    public static string currentScannedItemId = "";
    public static Dictionary<string, string> products; //for caching product infos from backend

    public void Start()
    {
        products = new Dictionary<string, string>();
    }

    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        string myWord = eventData.RecognizedText.ToLower();

        if (myWord.Equals("next item"))
        {
            counter++;
            updateDestination();
            checkListCounter++;
            if (!(checkListCounter < SetCurrentItem.checkListItems.Length)) {
                checkListCounter = 0;
            }
            currentItemId = SetCurrentItem.checkListItems[checkListCounter].id;
            setCurrentItemHelper();
            changePurchasedImageHelper();

            //getItemRequestHelper();
        }
        else if (myWord.Equals("previous item") && counter > 1)
        {
            counter--;
            updateDestination();
            checkListCounter--;
            if ((checkListCounter < 0))
            {
                checkListCounter = SetCurrentItem.checkListItems.Length - 1;
            }
            currentItemId = SetCurrentItem.checkListItems[checkListCounter].id;
            setCurrentItemHelper();
            changePurchasedImageHelper();
          //  getItemRequestHelper();
        }
        else if (myWord.Equals("start shopping"))
        {
            counter = 1;
            checkListCounter = 0;
        }
        else if (myWord.Equals("done shopping"))
        {
            TextToSpeechController.speakText("Please continue on your iPhone");

            //Done shopping voice prompt
        }
        else if (myWord.Equals("add to cart"))
        {
            if (!currentScannedItemId.Equals(""))
                StartCoroutine(addToCart());
            else
                TextToSpeechController.speakText("Please scan an item to add to cart");

        }
        else if(myWord.Equals("next option"))
        {
            showNextAlternative();
        }
        else if (myWord.Equals("help"))
        {
            TextToSpeechController.speakText("You can say any of the following. Add to Cart to add an item to the cart. Next option to view the next alternative.Previous item to shop for the previous item. Done shopping to complete your shopping flow. Help to hear this again");
           
        }
    }

    [Serializable]
    public class CartItem
    {
        public string _id;
        public string quantity;
   
    }

    private IEnumerator addToCart()
    {
        if (!currentScannedItemId.Equals("")) // to check if any item is scanned or not. Allow only addToCart on a scanned item.
        {
            if (!isItemAlreadyAdded()) //if item is already added or not
            { 
            CartItem obj = new CartItem();
            obj._id = currentScannedItemId;
            obj.quantity = 1 + "";
            string postBody = JsonUtility.ToJson(obj);

            using (UnityWebRequest www = UnityWebRequest.Put(Strings.host + "/checklist/addToCart", postBody))
            {
                www.SetRequestHeader("Content-Type", "application/json");
                www.method = "POST";
                yield return www.SendWebRequest();

                if (www.isNetworkError) //network error on post request
                {
                    Debug.Log("addToCart error response: " + www.error);
                    TextToSpeechController.speakText("Add item failed. Try again later!");
                }
                else //post successful
                {
                    var result = JSON.Parse(www.downloadHandler.text);
                    if (result["message"].ToString().Equals("\"Successfully Added\""))
                    {
                        if (currentScannedItemId.Equals(currentItemId)) //if currently scanned item id and "current item in scan" panel are same
                        {
                            SetCurrentItem.checkListItems[checkListCounter].isPurchased = "1";
                            changePurchasedImageHelper();
                            Debug.Log("Added item in checklist and it is same as the one currenlty in scan");
                        }
                        else
                        {
                            for (int i = 0; i < SetCurrentItem.checkListItems.Length; i++)
                            {
                                if (SetCurrentItem.checkListItems[i].id.Equals(currentScannedItemId))
                                {
                                    SetCurrentItem.checkListItems[i].isPurchased = "1";
                                    Debug.Log("Added item in checklist but not the one currenlty in scan panel");
                                }
                            }
                        }
                        Debug.Log("Added item not in checklist");
                        TextToSpeechController.speakText("The item was added successfully");

                    }
                    else
                    {
                        TextToSpeechController.speakText("Item could not be added. Please try again."); // some error due to which server sending bad response
                    }
                    Debug.Log("addToCart response: " + www.downloadHandler.text);
                }
            }
            }
            else
            {
                TextToSpeechController.speakText("Item is already added to cart.");

            }
        }
    }

    private bool isItemAlreadyAdded()
    {
        for (int i = 0; i < SetCurrentItem.checkListItems.Length; i++)
        {
            if (SetCurrentItem.checkListItems[i].id.Equals(currentScannedItemId))
            {
                return SetCurrentItem.checkListItems[i].isPurchased.Equals("1");
                
            }
        }
        return false;
    }

    private void changePurchasedImageHelper()
    {
        GameObject.Find("ARCamera").BroadcastMessage("setPurchasedImage", SetCurrentItem.checkListItems[checkListCounter].isPurchased);
    }

    private void setCurrentItemHelper()
    {
        GameObject.Find("ARCamera").BroadcastMessage("setCurrentItemText", SetCurrentItem.checkListItems[checkListCounter].name);
    }

    private void getItemRequestHelper()
    {
        GameObject.Find("ARCamera").BroadcastMessage("getRequest", currentItemId);
    }
    private void resetLineRendererHelper()

    {
        GameObject.Find("HumanNavigation").gameObject.SendMessage("resetLineRenderer");
    }

    private void showNextAlternative()
    {
        ShopScrollList scrollShop = GameObject.Find("Content").GetComponent<ShopScrollList>();
        int totalAlt = scrollShop.itemList.Count;
        if (totalAlt == 0)
            return;
        if (altCounter == totalAlt)
            altCounter = 0;
        Item newItem = scrollShop.itemList[altCounter++];
        currentItemId = newItem.itemId;

        Debug.Log("Next Item"+ currentItemId);
        getItemRequestHelper();
    }

    private void updateDestination()
    {
        Vector3 val1 = new Vector3(0f, -0.5f, 10f);
        Vector3 val2 = new Vector3(-5f, -0.5f, 5f);
        Vector3 val3 = new Vector3(-10f, -0.5f, 0f);
        Vector3 val4 = new Vector3(-5f, -0.5f, 10f);

        switch (counter)
        {
            case 1:
                resetLineRendererHelper();
                HumanNavigation.destinationVector = val1;
                destination.transform.position = val1;
                TextToSpeechController.speakText("Destination changed and set");
                break;

            case 2:
                resetLineRendererHelper();
                HumanNavigation.destinationVector = val2;
                destination.transform.position = val2;
                break;

            case 3:
                resetLineRendererHelper();
                HumanNavigation.destinationVector = val3;
                destination.transform.position = val3;
                break;

            case 4:
                resetLineRendererHelper();
                HumanNavigation.destinationVector = val4;
                destination.transform.position = val4;
                break;

            default:
                resetLineRendererHelper();
                counter = 2;
                HumanNavigation.destinationVector = val1;
                destination.transform.position = val1;
                break;

        }


    }
}
