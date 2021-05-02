using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConvenienceUIManager : MonoBehaviour
{
    public ConvenienceItemSetManager convenienceItemSetManager;

    private void Start()
    {
        convenienceItemSetManager = new ConvenienceItemSetManager();
        // コンビニで販売するアイテムリストを読み込む(json)
        ConvenienceItemData[] convenienceItemDataArray = convenienceItemSetManager.GetConvenienceJsonFile();
        // 最初のUIセット
        FirstUISetting(convenienceItemDataArray);
    }
    public void Update()
    {
        if (Input.GetMouseButton(0))
        {
            ItemClickPanelUISetting(false);
        }
    }

    public void ItemClickPanelUISetting(bool beginItem)
    {
        Debug.Log("Call ItemClickPanelUISetting");

        // 最初のアイテムの情報を呼び出すのか?
        if (!beginItem)
        {
            Camera uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
            Ray ray = uiCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
            // クリックした位置に物体があったら
            if (hit.collider != null)
            {
                Debug.Log("hit.transform.gameObject.name: " + hit.transform.gameObject.name);
                // クリックしたオブジェクトの親が'Content'なら
                if (hit.transform.parent.name.Equals("Content"))
                {
                    // UI Clear
                    ClearSeletedUI();
                    // 洗濯表示
                    hit.transform.Find("itemChecked").GetComponent<Text>().text = "Y";
                    // set item outline
                    hit.transform.GetComponent<Outline>().effectDistance = new Vector2(10, 10);
                    // Panelにアイテムの情報を移す
                    GameObject.Find("Canvas").transform.Find("Panel").transform.Find("Text").GetComponent<Text>().text =
                        "[" + "<color=#93DAFF>" + hit.transform.Find("itemName").GetComponent<Text>().text + "</color>" + "]" + "\n" +
                        "販売価格:" + "<color=#93DAFF>" + hit.transform.Find("itemPrice").GetComponent<Text>().text + "</color>" + "    " + "残り:" + "<color=#93DAFF>" + hit.transform.Find("itemQuantity").GetComponent<Text>().text + "</color>" + "\n" +
                        "-" + hit.transform.Find("itemDescription").GetComponent<Text>().text;


                }
            }
        }
        // 最初のアイテムなら
        else
        {
            GameObject itemBox0 = GameObject.Find("Canvas").transform.Find("menuBox").transform.Find("Scroll View").transform.Find("Viewport").transform.Find("Content").transform.Find("itemBox0").gameObject;

            // UI Clear
            ClearSeletedUI();

            // 洗濯表示
            itemBox0.transform.Find("itemChecked").GetComponent<Text>().text = "Y";
            // set item outline
            itemBox0.transform.GetComponent<Outline>().effectDistance = new Vector2(10, 10);
            // Panelにアイテムの情報を移す
            GameObject.Find("Canvas").transform.Find("Panel").transform.Find("Text").GetComponent<Text>().text =
                "[" + "<color=#93DAFF>" + itemBox0.transform.Find("itemName").GetComponent<Text>().text + "</color>" + "]" + "\n" +
                "販売価格:" + "<color=#93DAFF>" + itemBox0.transform.Find("itemPrice").GetComponent<Text>().text + "</color>" + "    " + "残り:" + "<color=#93DAFF>" + itemBox0.transform.Find("itemQuantity").GetComponent<Text>().text + "</color>" + "\n" +
                "-" + itemBox0.transform.Find("itemDescription").GetComponent<Text>().text;
        }
    }

    public void ClearSeletedUI()
    {
        GameObject contentGameObj = GameObject.Find("Canvas").transform.Find("menuBox").transform.Find("Scroll View").transform.Find("Viewport").transform.Find("Content").gameObject;

        // アイテム数
        int childCount = GameObject.Find("Canvas").transform.Find("menuBox").transform.Find("Scroll View").transform.Find("Viewport").transform.Find("Content").childCount;
        // アイテム数くらい繰り返す
        for(int i=0; i<childCount; i++)
        {
            // 選択されたアイテムなら
            if (contentGameObj.transform.Find("itemBox" + i).transform.Find("itemChecked").GetComponent<Text>().text.Equals("Y"))
            {
                // 洗濯表示初期化
                contentGameObj.transform.Find("itemBox" + i).transform.Find("itemChecked").GetComponent<Text>().text = "";
                // UI clear
                contentGameObj.transform.Find("itemBox" + i).GetComponent<Outline>().effectDistance = Vector2.zero;
                // UI Clear(panel)
                GameObject.Find("Panel").transform.Find("Text").GetComponent<Text>().text = null;
                break;
            }
        }
    }

public void FirstUISetting(ConvenienceItemData[] convenienceItemDataArray)
    {
        GameObject canvasGameObj = GameObject.Find("Canvas");
        GameObject contentGameObj = GameObject.Find("Canvas").transform.Find("menuBox").transform.Find("Scroll View").transform.Find("Viewport").transform.Find("Content").gameObject;

        // itemBoxの基準になるオブジェクトを蓄える
        GameObject itemBox = canvasGameObj.transform.Find("menuBox")
            .transform.Find("Scroll View").transform.Find("Viewport").transform.Find("Content")
            .transform.Find("itemBox0").gameObject;

        // アイテム数(-1)くらい繰り返す
        for (int i=0; i < convenienceItemDataArray.Length; i++)
        {
            // 最初のアイテムなら && アイテムがセール状態なら
            if (i == 0 && convenienceItemDataArray[i].itemSale.Equals("Y"))
            {
                // オブジェクトを作らないで情報だけ移す(itemBox0)
                Texture2D texture = Resources.Load(convenienceItemDataArray[i].itemImagePath, typeof(Texture2D)) as Texture2D;
                Rect rect = new Rect(0, 0, texture.width, texture.height);
                contentGameObj.transform.Find("itemBox" + i).transform.Find("itemImage").GetComponent<Image>().sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));

                contentGameObj.transform.Find("itemBox" + i).transform.Find("itemName").GetComponent<Text>().text = convenienceItemDataArray[i].itemName;
                contentGameObj.transform.Find("itemBox" + i).transform.Find("itemDescription").GetComponent<Text>().text = convenienceItemDataArray[i].itemDescription;
                contentGameObj.transform.Find("itemBox" + i).transform.Find("itemQuantity").GetComponent<Text>().text = convenienceItemDataArray[i].itemQuantity.ToString();
                contentGameObj.transform.Find("itemBox" + i).transform.Find("itemPrice").GetComponent<Text>().text = convenienceItemDataArray[i].itemPrice.ToString();

                // itemAddButtonに注文追加イベントをつける
                contentGameObj.transform.Find("itemBox" + i).transform.Find("itemAddButton").GetComponent<Button>().onClick.AddListener(ClickItemAddButton);
            }
            //最初のアイテムじゃないなら
            else if(i != 0 && convenienceItemDataArray[i].itemSale.Equals("Y"))
            {
                // アイテム数くらいオブジェクトを作る
                itemBox = Instantiate(itemBox);
                itemBox.name = "itemBox" + i;
                itemBox.transform.SetParent(canvasGameObj.transform.Find("menuBox").transform.Find("Scroll View").transform.Find("Viewport").transform.Find("Content"));

                // アイテム情報を移す
                Texture2D texture = Resources.Load(convenienceItemDataArray[i].itemImagePath, typeof(Texture2D)) as Texture2D;
                Rect rect = new Rect(0, 0, texture.width, texture.height);
                itemBox.transform.Find("itemImage").GetComponent<Image>().sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));

                itemBox.transform.Find("itemName").GetComponent<Text>().text = convenienceItemDataArray[i].itemName;
                itemBox.transform.Find("itemDescription").GetComponent<Text>().text = convenienceItemDataArray[i].itemDescription;
                itemBox.transform.Find("itemQuantity").GetComponent<Text>().text = convenienceItemDataArray[i].itemQuantity.ToString();
                itemBox.transform.Find("itemPrice").GetComponent<Text>().text = convenienceItemDataArray[i].itemPrice.ToString();

                // itemAddButtonに注文追加イベントをつける
                itemBox.transform.Find("itemAddButton").GetComponent<Button>().onClick.AddListener(ClickItemAddButton);
            }
        }
    }

    public void ClickItemAddButton()
    {
        GameObject canvasGameObj = GameObject.Find("Canvas");
        Transform addItemAlertBoxTransform = canvasGameObj.transform.Find("addItemAlertBox");
        // クリックしたオブジェクトを取り出す
        Transform selectedItemBox = EventSystem.current.currentSelectedGameObject.transform.parent;
        Debug.Log("itemBoxTransform: " + selectedItemBox);

        canvasGameObj.transform.Find("menuBox").gameObject.SetActive(false);
        canvasGameObj.transform.Find("orderBox").gameObject.SetActive(false);
        canvasGameObj.transform.Find("orderConfirmButton").gameObject.SetActive(false);
        canvasGameObj.transform.Find("specificationBox").gameObject.SetActive(false);
        addItemAlertBoxTransform.gameObject.SetActive(true);

        // 選択されたアイテムの情報を移す
        addItemAlertBoxTransform.transform.Find("itemImage").GetComponent<Image>().sprite = selectedItemBox.Find("itemImage").GetComponent<Image>().sprite;
        addItemAlertBoxTransform.transform.Find("itemName").GetComponent<Text>().text = selectedItemBox.Find("itemName").GetComponent<Text>().text;
        addItemAlertBoxTransform.transform.Find("itemPrice").GetComponent<Text>().text = selectedItemBox.Find("itemPrice").GetComponent<Text>().text+"円";
        addItemAlertBoxTransform.transform.Find("itemQuantity").GetComponent<Text>().text = "1";

        // quantityのminus, plusButtonAddListener
        addItemAlertBoxTransform.transform.Find("minusButton").GetComponent<Button>().onClick.AddListener(() => ClickMinusButton(addItemAlertBoxTransform, selectedItemBox));
        addItemAlertBoxTransform.transform.Find("plusButton").GetComponent<Button>().onClick.AddListener(() => ClickPlusButton(addItemAlertBoxTransform, selectedItemBox));

        // addButtonにAddListener
        addItemAlertBoxTransform.transform.Find("addButton").GetComponent<Button>().onClick.AddListener(() => ClickAddButton(selectedItemBox));
        // cancelButtonにAddListener
        addItemAlertBoxTransform.transform.Find("cancelButton").GetComponent<Button>().onClick.AddListener(() => ClickCancelButton(selectedItemBox));

    }

    public void ClickAddButton(Transform selectedItemBox)
    {
        ClickCancelButton(selectedItemBox);
        GameObject canvasGameObj = GameObject.Find("Canvas");
        GameObject addItemGameObj = canvasGameObj.transform.Find("addItemAlertBox").gameObject;
        
        // menuBoxからアイテムaddButton機能を防ぐ
        selectedItemBox.transform.Find("itemAddButton").GetComponent<Button>().interactable = false;
        selectedItemBox.transform.Find("itemAddButton").transform.Find("Text").GetComponent<Text>().text = "選択中";

        // orderBoxにアイテムを追加する
        
        GameObject ContentGameObj = canvasGameObj.transform.Find("orderBox").transform.Find("Scroll View").transform.Find("Viewport").transform.Find("Content").gameObject;
        // orderBoxのContentのchildCountを取り出す
        int contentChildCount = ContentGameObj.transform.childCount;
        Debug.Log("contentChildCount: " + contentChildCount);

        // default itemBoxを取り出す
        Transform defaultItemBox = ContentGameObj.transform.GetChild(0);

        // 新しいitemBoxよ作る
        GameObject itemBoxGameObj = Instantiate(defaultItemBox.gameObject);
        itemBoxGameObj.name = "itemBox" + (contentChildCount).ToString();
        itemBoxGameObj.transform.SetParent(ContentGameObj.transform);
        itemBoxGameObj.SetActive(true);

        // default　objectに情報セット
        itemBoxGameObj.transform.Find("itemName").GetComponent<Text>().text = addItemGameObj.transform.Find("itemName").GetComponent<Text>().text;
        itemBoxGameObj.transform.Find("itemQty").GetComponent<Text>().text = addItemGameObj.transform.Find("itemQuantity").GetComponent<Text>().text;
        itemBoxGameObj.transform.Find("itemPrice").GetComponent<Text>().text = addItemGameObj.transform.Find("itemPrice").GetComponent<Text>().text;


        // onClick.AddListenerのイベントが積もるのを防止する
        RemoveButtonListeners();

        // specificationBox !!
    }

    public void ClickCancelButton(Transform selectedItemBox)
    {
        GameObject canvasGameObj = GameObject.Find("Canvas");
        Transform addItemAlertBoxTransform = canvasGameObj.transform.Find("addItemAlertBox");

        // onClick.AddListenerのイベントが積もるのを防止する
        RemoveButtonListeners();

        canvasGameObj.transform.Find("menuBox").gameObject.SetActive(true);
        canvasGameObj.transform.Find("orderBox").gameObject.SetActive(true);
        canvasGameObj.transform.Find("orderConfirmButton").gameObject.SetActive(true);
        canvasGameObj.transform.Find("specificationBox").gameObject.SetActive(false);
        addItemAlertBoxTransform.gameObject.SetActive(false);
    }

    public void RemoveButtonListeners()
    {
        GameObject canvasGameObj = GameObject.Find("Canvas");
        Transform addItemAlertBoxTransform = canvasGameObj.transform.Find("addItemAlertBox");

        // onClick.AddListenerのイベントが積もるのを防止する
        addItemAlertBoxTransform.transform.Find("minusButton").GetComponent<Button>().onClick.RemoveAllListeners();
        addItemAlertBoxTransform.transform.Find("plusButton").GetComponent<Button>().onClick.RemoveAllListeners();
        addItemAlertBoxTransform.transform.Find("cancelButton").GetComponent<Button>().onClick.RemoveAllListeners();
        addItemAlertBoxTransform.transform.Find("addButton").GetComponent<Button>().onClick.RemoveAllListeners();
    }

    public void ClickPlusButton(Transform addItemAlertBoxTransform, Transform selectedItemBox)
    {
        int presentQty = Int32.Parse(addItemAlertBoxTransform.transform.Find("itemQuantity").GetComponent<Text>().text);
        Debug.Log("presentQty: " + presentQty);

        int maxQty = Int32.Parse(selectedItemBox.transform.Find("itemQuantity").GetComponent<Text>().text);
        // もしalertのquantityがmaxQuantity未満なら
        if (maxQty > presentQty)
        {
            // 数を1増やす
            addItemAlertBoxTransform.transform.Find("itemQuantity").GetComponent<Text>().text = (presentQty + 1).ToString();
        }

        // 価格をセッティング
        int itemPrice = Int32.Parse(selectedItemBox.transform.Find("itemPrice").GetComponent<Text>().text.Replace("円", ""));
        presentQty = Int32.Parse(addItemAlertBoxTransform.transform.Find("itemQuantity").GetComponent<Text>().text);
        addItemAlertBoxTransform.transform.Find("itemPrice").GetComponent<Text>().text = (itemPrice * presentQty).ToString()+"円";
    }

    public void ClickMinusButton(Transform addItemAlertBoxTransform, Transform selectedItemBox)
    {
        int presentQty = Int32.Parse(addItemAlertBoxTransform.transform.Find("itemQuantity").GetComponent<Text>().text);
        Debug.Log("presentQty: " + presentQty);
        // もしalertのquantityが2以上なら
        if (presentQty > 1)
        {
            // 数を1減らす
            addItemAlertBoxTransform.transform.Find("itemQuantity").GetComponent<Text>().text = (presentQty - 1).ToString();
        }

        // 価格をセッティング
        int itemPrice = Int32.Parse(selectedItemBox.transform.Find("itemPrice").GetComponent<Text>().text.Replace("円",""));
        presentQty = Int32.Parse(addItemAlertBoxTransform.transform.Find("itemQuantity").GetComponent<Text>().text);
        addItemAlertBoxTransform.transform.Find("itemPrice").GetComponent<Text>().text = (itemPrice * presentQty).ToString() + "円";
    }
}
