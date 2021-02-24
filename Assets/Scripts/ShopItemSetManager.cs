﻿using System.Collections;
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
        SetShopItem();

        // ショップアイテムファイルをclassに読み込む
        ShopItem[] loadedShopItemArray = LoadShopItemJsonFile();

        //読み込んだclassファイルをショップUIにセットする
        SetShopItemUI(loadedShopItemArray);
    }

    public void SetShopItemUI(ShopItem[] loadedShopItemArray)
    {
        int shopItemCount = loadedShopItemArray.Length;
        Transform menuBackTransform = GameObject.Find("Canvas").transform.Find("CafeMenuCanvas").transform.Find("CafeMenuScrollView").transform.Find("Viewport").transform.Find("menuBack").transform;
        Vector3 menuBackPos = menuBackTransform.position;

        Debug.Log("shopItemCount: " + shopItemCount);

        // アイテム数が4以上ならアイテムバッググラウンドをアイテムかずほど伸ばす
        if(shopItemCount > 3)
        {
            GameObject.Find("Canvas").transform.Find("CafeMenuCanvas").transform.Find("CafeMenuScrollView").transform.Find("Viewport").transform.Find("menuBack").GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 200 * shopItemCount);
        }

        // itemBoxObject
        GameObject itemBox = GameObject.Find("Canvas").transform.Find("CafeMenuCanvas").transform.Find("CafeMenuScrollView").transform.Find("Viewport").transform.Find("menuBack").transform.Find("itemBox0").gameObject;

        // アイテムの数だけオブジェクトを作る
        for(int i=0; i< shopItemCount; i++)
        {
            // 最初のdefaultオブジェクトにはアイテム情報をセット
            if (i == 0 && 4 > shopItemCount)
            {
                itemBox.transform.Find("itemNameBox").transform.Find("Text").GetComponent<Text>().text = loadedShopItemArray[i].itemName;
                itemBox.transform.Find("itemPriceBox").transform.Find("Text").GetComponent<Text>().text = loadedShopItemArray[i].price.ToString()+"円";
                itemBox.transform.Find("itemDescription").GetComponent<Text>().text = loadedShopItemArray[i].description;
            }
            // 最初のdefaultオブジェクトにはアイテム情報をセット(バッググラウンドが伸びることによって最後のアイテムボックスの位置を調整する)
            else if (i == 0 && shopItemCount > 3)
            {
                itemBox.transform.Find("itemNameBox").transform.Find("Text").GetComponent<Text>().text = loadedShopItemArray[i].itemName;
                itemBox.transform.Find("itemPriceBox").transform.Find("Text").GetComponent<Text>().text = loadedShopItemArray[i].price.ToString() + "円";
                itemBox.transform.Find("itemDescription").GetComponent<Text>().text = loadedShopItemArray[i].description;
                itemBox.transform.Translate(0, (shopItemCount-3)*100, 0);
            }
            else
            {
                // オブジェクトを作る
                itemBox = Instantiate(itemBox, new Vector3(itemBox.transform.position.x, itemBox.transform.position.y - 200, itemBox.transform.position.z), Quaternion.identity);
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

        // 
    }

    public void SetShopItem()
    {
        ShopItem[] shopItemArray = new ShopItem[4];

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

        shopItemArray[2] = new ShopItem();
        shopItemArray[2].itemName = "アイスティー";
        shopItemArray[2].price = 350;
        shopItemArray[2].quantity = 5;
        shopItemArray[2].description = "冷たくて甘い";
        shopItemArray[2].sale = false;

        shopItemArray[3] = new ShopItem();
        shopItemArray[3].itemName = "ハムサンド";
        shopItemArray[3].price = 500;
        shopItemArray[3].quantity = 5;
        shopItemArray[3].description = "定番のハムサンド";
        shopItemArray[3].sale = false;

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
