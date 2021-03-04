using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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
        int addmenuBackSizeCount = shopItemCount - 3;
        Transform menuBackTransform = GameObject.Find("Canvas").transform.Find("CafeMenuCanvas").transform.Find("CafeMenuScrollView").transform.Find("Viewport").transform.Find("menuBack").transform;

        Debug.Log("shopItemCount: " + shopItemCount);

        
             // アイテム数が4以上ならアイテムバッググラウンドをアイテムかずほど伸ばす
        if(shopItemCount > 3)
        {
            GameObject.Find("Canvas").transform.Find("CafeMenuCanvas").transform.Find("CafeMenuScrollView").transform.Find("Viewport").transform.Find("menuBack").GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 200 * shopItemCount);
        }
        
        Vector3 menuBackPos = menuBackTransform.position;

        // itemBoxObject
        GameObject itemBox = GameObject.Find("Canvas").transform.Find("CafeMenuCanvas").transform.Find("CafeMenuScrollView").transform.Find("Viewport").transform.Find("menuBack").transform.Find("itemBox0").gameObject;

        
            // アイテムの数だけオブジェクトを作る
        for (int i=0; i< shopItemCount; i++)
        {
            // 0番目アイテムの情報をセッティング
                  // アイテム数が3以下default
            if (i == 0 && 4 > shopItemCount)
            {
                itemBox.transform.Find("itemNameBox").transform.Find("Text").GetComponent<Text>().text = loadedShopItemArray[i].itemName;
                itemBox.transform.Find("itemPriceBox").transform.Find("Text").GetComponent<Text>().text = loadedShopItemArray[i].price.ToString()+"円";
                itemBox.transform.Find("itemDescription").GetComponent<Text>().text = loadedShopItemArray[i].description;
                itemBox.transform.Translate(0, menuBackPos.y - 401, 0);
            }
            // 最初のdefaultオブジェクトにはアイテム情報をセット(バッググラウンドが伸びることによって最後のアイテムボックスの位置を調整する)
            else if (i == 0 && shopItemCount > 3)
            {
                
                itemBox.transform.Find("itemNameBox").transform.Find("Text").GetComponent<Text>().text = loadedShopItemArray[i].itemName;
                itemBox.transform.Find("itemPriceBox").transform.Find("Text").GetComponent<Text>().text = loadedShopItemArray[i].price.ToString() + "円";
                itemBox.transform.Find("itemDescription").GetComponent<Text>().text = loadedShopItemArray[i].description;
                itemBox.transform.Translate(0, (addmenuBackSizeCount * 100) - 50, 0);
                //Debug.Log(" menuBackPos.y: " + cafeMenuScrollViewPos.y);
                //Debug.Log("(shopItemCount-3)*50: " + cafeMenuScrollViewPos.y / shopItemCount);
                //Debug.Log("menuBackPos.y - (shopItemCount-3)*50: " + menuBackPos.y + (menuBackPos.y * shopItemCount - 3) * 10));
                //itemBox.transform.Translate(0, menuBackPos.y - 400, 0);

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

            // オーダーボタンにイベント追加
            itemBox.transform.Find("orderButton").GetComponent<Button>().onClick.AddListener(addOrderButtonEvent);
        }

        // UIセッティング完了
    }

    public void addOrderButtonEvent()
    {
        // クリックしたオブジェクトを取り出す
        Transform itemBoxTransform = EventSystem.current.currentSelectedGameObject.transform.parent;
        Debug.Log("itemBoxTransform: " + itemBoxTransform);
        // detailItemにつけるランダム番号
        System.Random r = new System.Random();

        // ボタンを押すとdetailにメニューを入れる
        if (itemBoxTransform.Find("orderCheck").GetComponent<Text>().text.Equals("N"))
        {
            itemBoxTransform.Find("orderCheck").GetComponent<Text>().text = "Y";
            itemBoxTransform.Find("orderButton").GetComponent<Button>().interactable = false;

            Transform detailBackTransform = GameObject.Find("Canvas").transform.Find("DetailOrderCanvas").transform.Find("DetailOrderScrollView").transform.Find("Viewport").transform.Find("detailBack");
            GameObject detailItemSample = detailBackTransform.transform.Find("detailItemSample").gameObject;
            int detailItemCount = detailBackTransform.childCount;
            Debug.Log("detailItemCount: " + detailItemCount);

            // detailItemが0なら(sampleを除いて)detailItemSampleの位置に最初のdetailItemを作る
            if(detailItemCount-1 == 0)
            {
                GameObject detailItem = Instantiate(detailItemSample, new Vector3(detailItemSample.transform.position.x, detailItemSample.transform.position.y, detailItemSample.transform.position.z), Quaternion.identity);
                detailItem.gameObject.name = "detailItem"+ r.Next().ToString();
                detailItem.transform.SetParent(detailBackTransform);

                // クリックしたメニューアイテムの情報を移る
                detailItem.transform.Find("itemName").GetComponent<Text>().text = itemBoxTransform.Find("itemNameBox").transform.Find("Text").GetComponent<Text>().text;
                detailItem.transform.Find("itemPrice").GetComponent<Text>().text = itemBoxTransform.Find("itemPriceBox").transform.Find("Text").GetComponent<Text>().text;
                // detailボタン(削除)にイベントをつける
                detailItem.transform.Find("deleteButton").GetComponent<Button>().onClick.AddListener(delegate { addDetailDeleteButtonEvent(itemBoxTransform); });

                detailItem.SetActive(true);
            }
            // 一番下にdetailItemを作る
            else
            {
                Transform lastDetailItemTransform = detailBackTransform.transform.GetChild(detailBackTransform.childCount-1);
                Debug.Log("lastDetailItemTransform: " + lastDetailItemTransform.gameObject.name);
                GameObject detailItemN = Instantiate(lastDetailItemTransform.gameObject, new Vector3(lastDetailItemTransform.gameObject.transform.position.x, lastDetailItemTransform.gameObject.transform.position.y-50, lastDetailItemTransform.gameObject.transform.position.z), Quaternion.identity);
                detailItemN.gameObject.name = "detailItem" + r.Next().ToString();
                detailItemN.transform.SetParent(detailBackTransform);

                // クリックしたメニューアイテムの情報を移る
                detailItemN.transform.Find("itemName").GetComponent<Text>().text = itemBoxTransform.Find("itemNameBox").transform.Find("Text").GetComponent<Text>().text;
                detailItemN.transform.Find("itemPrice").GetComponent<Text>().text = itemBoxTransform.Find("itemPriceBox").transform.Find("Text").GetComponent<Text>().text;
                // detailボタン(削除)にイベントをつける
                detailItemN.transform.Find("deleteButton").GetComponent<Button>().onClick.AddListener(delegate { addDetailDeleteButtonEvent(itemBoxTransform); });

                detailItemN.SetActive(true);
                
            }
            
            // detailBackのchildCountが3以上ならdetailBackのサイズを伸ばす
        }
    }

    public void addDetailDeleteButtonEvent(Transform itemBoxTransform)
    {
        
        // クリックしたオブジェクトを取り出す
        Transform detailItemTransform = EventSystem.current.currentSelectedGameObject.transform.parent;
        Transform detailBack = detailItemTransform.parent;
        // メニューアイテムのボタンをいかす
        itemBoxTransform.transform.Find("orderCheck").GetComponent<Text>().text = "N"; 
        itemBoxTransform.transform.Find("orderButton").GetComponent<Button>().interactable = true;


        Debug.Log("Destroy: " + detailItemTransform.gameObject.name);
        // ★Destoryを後オブジェクトすぐに破壊されない、フレームアプデ後(1frame)OnDestroyが作用される
        Destroy(detailItemTransform.gameObject);

        Debug.Log("detailBack.childCount: " + detailBack.childCount);

        ArrayList detailItemList = new ArrayList();
   
        for (int i=1; i< detailBack.childCount; i++)
        {
            /*
            Transform detailItemNTransform = detailBack.GetChild(i);
            Debug.Log("detailItemNTransform: " + detailItemNTransform.name);

            
            if (!firstDetailItemSW && detailItemNTransform.transform.Find("itemName") != null)
            {
                Debug.Log("position reset");
                // ★Canvas(UI)を親にしている子objectのpositionを取り出すときはRectTransformのanchoredPositionを利用する
                RectTransform detailItemSample = (RectTransform)detailBack.Find("detailItemSample");
                detailItemNTransform.Translate(detailItemSample.anchoredPosition.x, detailItemSample.anchoredPosition.y, 0);
                firstDetailItemSW = true;
            }
            else
            {
                Debug.Log("detailItemNTransform: " + detailItemNTransform.name);
                detailItemNTransform.Translate(detailItemNTransform.position.x, detailItemNTransform.position.y+50, detailItemNTransform.position.z);
            }
            */

            RectTransform detailItemSample = (RectTransform)detailBack.Find("detailItemSample");
            RectTransform detailItemNTransform = (RectTransform)detailBack.GetChild(i);
            // オブジェクトを配列に移る(detailSampleと削除されたdetailItem除外)
            if (!detailItemNTransform.name.Equals(detailItemTransform.name))
            {
                Debug.Log("added detailItemNTransform: " + detailItemNTransform.name);
                detailItemList.Add(detailItemNTransform);
                // detailItemの位置を一つにまとう
                detailItemNTransform.anchoredPosition = new Vector2(detailItemSample.anchoredPosition.x, detailItemSample.anchoredPosition.y);
            }
        }


        // 配列のdetailItemの位置変更
        for(int i=0; i< detailItemList.Count; i++)
        {
            // ★Canvas(UI)を親にしている子objectのpositionを取り出すときはRectTransformのanchoredPositionを利用する
            RectTransform detailItemSample = (RectTransform)detailBack.Find("detailItemSample");
            RectTransform detailItem = (RectTransform)detailItemList[i];
            Debug.Log("detailItem.name: " + detailItem.name);
            // 二番目のdetailItemから徐々に位置を調整する
            if(i != 0)
            {
                RectTransform detailItemBefore = (RectTransform)detailItemList[i-1];
                Debug.Log("detailItemBefore: " + detailItemBefore.name);
                Debug.Log("detailItemBefore.anchoredPosition.y: " + detailItemBefore.anchoredPosition.y);
                Debug.Log("detailItem(!=0): " + detailItem);
                Debug.Log("(int)detailItemBefore.anchoredPosition.y-45f: " + ((int)detailItemBefore.anchoredPosition.y + 50));
                detailItem.anchoredPosition = new Vector2(detailItemBefore.anchoredPosition.x, detailItemBefore.anchoredPosition.y - 50);
                
            }
        }
    }

    public void SetShopItem()
    {
        ShopItem[] shopItemArray = new ShopItem[10];

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
