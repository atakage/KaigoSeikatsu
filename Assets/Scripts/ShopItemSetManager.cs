using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        // タイトルで
        //SetShopItem();

        // ショップアイテムファイルをclassに読み込む
        ShopItem[] loadedShopItemArray = LoadShopItemJsonFile();

        //読み込んだclassファイルをショップUIにセットする
        SetShopItemUI(loadedShopItemArray);
    }

    public void SetShopItemUI(ShopItem[] loadedShopItemArray)
    {
        int shopItemCount = loadedShopItemArray.Length;
        Debug.Log("shopItemCount: " + shopItemCount);

        // itemBoxObject
        GameObject itemBox = GameObject.Find("Canvas").transform.Find("CafeMenuCanvas").transform.Find("CafeMenuScrollView").transform.Find("Viewport").transform.Find("menuBack").transform.Find("itemBox0").gameObject;

        // アイテムの数だけオブジェクトを作る
        for(int i=0; i< shopItemCount; i++)
        {
            // 最初のdefaultオブジェクトにはアイテム情報をセット
            if (i == 0)
            {
                itemBox.transform.Find("itemNameBox").transform.Find("Text").GetComponent<Text>().text = loadedShopItemArray[i].itemName;
                itemBox.transform.Find("itemPriceBox").transform.Find("Text").GetComponent<Text>().text = loadedShopItemArray[i].price.ToString()+"円";
                itemBox.transform.Find("itemDescription").GetComponent<Text>().text = loadedShopItemArray[i].description;
            }
            else
            {
                // オブジェクトを作る
                itemBox = Instantiate(itemBox, new Vector3(itemBox.transform.position.x, itemBox.transform.position.y - 190, itemBox.transform.position.z), Quaternion.identity);
                // オブジェクトネームを変更する
                itemBox.gameObject.name = "itemBox"+i;
                // オブジェクトの位置を決める
                itemBox.transform.SetParent(GameObject.Find("Canvas").transform.Find("CafeMenuCanvas").transform.Find("CafeMenuScrollView").transform.Find("Viewport").transform.Find("menuBack").transform);
                // アイテム情報をセット
                itemBox.transform.Find("itemNameBox").transform.Find("Text").GetComponent<Text>().text = loadedShopItemArray[i].itemName;
                itemBox.transform.Find("itemPriceBox").transform.Find("Text").GetComponent<Text>().text = loadedShopItemArray[i].price.ToString() + "円";
                itemBox.transform.Find("itemDescription").GetComponent<Text>().text = loadedShopItemArray[i].description;
            }
        }
    }

    public void SetShopItem()
    {
        ShopItem[] shopItemArray = new ShopItem[2];
        shopItemArray[0] = new ShopItem();
        shopItemArray[0].itemName = "コーヒー";
        shopItemArray[0].price = 400;
        shopItemArray[0].quantity = 5;
        shopItemArray[0].description = "温かいコーヒーだ";
        shopItemArray[0].sale = false;

        shopItemArray[1] = new ShopItem();
        shopItemArray[1].itemName = "カフェ・ラッテ";
        shopItemArray[1].price = 550;
        shopItemArray[1].quantity = 5;
        shopItemArray[1].description = "ミルクたっぷりのカフェ・ラッテ";
        shopItemArray[1].sale = false;

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

        // アイテムのsaleがfalseなら除いて詰める


        return loadedShopItemArray;
    }

}
