using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[System.Serializable]
public class ShopItem
{
    public string itemName;
    public int price;
    public int quantity;
    public string description;
    public bool sale;
}

public class ShopItemSetManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SetShopItem();

        // ショップアイテムファイルをclassに読み込む
        ShopItem[] loadedShopItemArray = LoadShopItemJsonFile();

        //読み込んだclassファイルをショップUIにセットする
        SetShopItemUI(loadedShopItemArray);
    }

    public void SetShopItemUI(ShopItem[] loadedShopItemArray)
    {

    }

    public void SetShopItem()
    {
        ShopItem[] shopItemArray = new ShopItem[1];
        shopItemArray[0] = new ShopItem();

        shopItemArray[0].itemName = "コーヒー";
        shopItemArray[0].price = 400;
        shopItemArray[0].quantity = 5;
        shopItemArray[0].description = "温かいコーヒーだ";
        shopItemArray[0].sale = false;

        CreateShopItemJsonFile(shopItemArray);
    }

    public void CreateShopItemJsonFile(ShopItem[] shopItemArray)
    {
        string itemAsStr = JsonHelper.ToJson(shopItemArray, true);
        Debug.Log("SHOPITEMSAVEDATA: " + itemAsStr);
        File.WriteAllText(Application.dataPath + "/Resources/saveData/shopItem.json", itemAsStr);
    }

    public ShopItem[] LoadShopItemJsonFile()
    {
        ShopItem[] loadedShopItemArray = null;
        try
        {
            string itemAsStr = File.ReadAllText(Application.dataPath + "/Resources/saveData/shopItem.json");
            Debug.Log("itemAsStr: " + itemAsStr);
            loadedShopItemArray = JsonHelper.FromJson<ShopItem>(itemAsStr);
            Debug.Log("loadedShopItemArray.Length: " + loadedShopItemArray.Length);
        }
        catch(Exception e)
        {
            Debug.Log("EXCEPTION: " + e);
            loadedShopItemArray = null;
        }
        return loadedShopItemArray;
    }

}
