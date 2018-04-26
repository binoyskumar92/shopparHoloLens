using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Item
{
    public string itemName;
    public Sprite icon;
    public float price = 10f;
    public string itemId;
    public Item()
    {

    }
    public Item(string itemId,string itemName, Sprite icon, float price)
    {
        this.itemId = itemId;
        this.itemName = itemName;
        this.icon = icon;
        this.price = price;
    }
   
}
public class ShopScrollList : MonoBehaviour {

    public List<Item> itemList=new List<Item>();

    //optional
    public Transform contentPanel;
    public ShopScrollList otherShop;

    //hold reference to header text
    public Text myAltDisplay;
    //hold reference to our Object Pool
    public SimpleObjectPool buttonObjectPool;

    public float alt = 5;

	// Use this for initialization
	void Start () {
        RefreshDisplay();
	}
    public void RefreshDisplay() {
        buttonObjectPool = GameObject.Find("ButtonObjectPool").GetComponent<SimpleObjectPool>();
        myAltDisplay = GameObject.Find("AlternativesDisplayText").GetComponent<Text>();
        RemoveButtons();
        AddButtons();
    }
    
    private void AddButtons()
    {
        for(int i = 0; i < itemList.Count; i++)
        {
            Item item = itemList[i];
            GameObject newButton = buttonObjectPool.GetObject();
            newButton.transform.SetParent(contentPanel);

            //tell a button to set himself up
            SampleButton sampleButton = newButton.GetComponent<SampleButton>();
            sampleButton.Setup(item,this);
        }
    }
    private void RemoveButtons()
    {
        while (contentPanel.childCount > 0)
        {
            GameObject toRemove = transform.GetChild(0).gameObject;
            buttonObjectPool.ReturnObject(toRemove);
        }
    }
}
