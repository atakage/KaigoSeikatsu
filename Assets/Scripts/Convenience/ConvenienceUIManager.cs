using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConvenienceUIManager : MonoBehaviour
{
    public ConvenienceItemSetManager convenienceItemSetManager;
    public PlayerSaveDataManager playerSaveDataManager;
    public UtilManager utilManager;
    public EventManager eventManager;
    public ChatManager chatManager;
    public SceneTransitionManager sceneTransitionManager;
    public PlayerData playerData;
    public GameObject canvasGameObj;
    public GameObject contentGameObj;
    public GameObject menuBoxContentGameObj;
    public GameObject specificationBoxGameObj;
    public GameObject clickBlockGameObj;
    private void Start()
    {
        playerSaveDataManager = new PlayerSaveDataManager();
        playerData = playerSaveDataManager.LoadPlayerData();

        canvasGameObj = GameObject.Find("Canvas");
        menuBoxContentGameObj = canvasGameObj.transform.Find("menuBox").transform.Find("Scroll View").transform.Find("Viewport").transform.Find("Content").gameObject;
        contentGameObj = canvasGameObj.transform.Find("orderBox").transform.Find("Scroll View").transform.Find("Viewport").transform.Find("Content").gameObject;
        specificationBoxGameObj = canvasGameObj.transform.Find("specificationBox").gameObject;
        clickBlockGameObj = canvasGameObj.transform.Find("clickBlock").gameObject;

        convenienceItemSetManager = new ConvenienceItemSetManager();
        utilManager = new UtilManager();
        eventManager = new EventManager();
        chatManager = GameObject.Find("ChatManager").GetComponent("ChatManager") as ChatManager;
        sceneTransitionManager = new SceneTransitionManager();

        // コンビニで販売するアイテムリストを読み込む(json)
        ConvenienceItemData[] convenienceItemDataArray = convenienceItemSetManager.GetConvenienceJsonFile();
        // 最初のUIセット
        FirstUISetting(convenienceItemDataArray);

        canvasGameObj.transform.Find("nextButton").GetComponent<Button>().onClick.AddListener(() => ClickNextButton());
        canvasGameObj.transform.Find("goToHomeAlertBox").transform.Find("confirmButton").GetComponent<Button>().onClick.AddListener(() => ClickgoToHomeConfirmButton());
        canvasGameObj.transform.Find("goToHomeAlertBox").transform.Find("cancelButton").GetComponent<Button>().onClick.AddListener(() => ClickgoToHomeCancelButton());

        canvasGameObj.transform.Find("orderConfirmButton").GetComponent<Button>().onClick.AddListener(() => ClickOrderConfirmBtn());
        canvasGameObj.transform.Find("orderConfirmAlertBox").transform.Find("confirmButton").GetComponent<Button>().onClick.AddListener(() => ClickOrderConfirmAlertBoxConfirmBtn());
        canvasGameObj.transform.Find("orderConfirmAlertBox").transform.Find("cancelButton").GetComponent<Button>().onClick.AddListener(() => ClickOrderConfirmAlertBoxCancelBtn());
    }
    public void Update()
    {
        if (Input.GetMouseButton(0))
        {
            ItemClickPanelUISetting(false);
        }

        // orderConfirmButtonのinteractable
        // orderBox->Contentの子が1以下なら(defaultを含め)またはspecificationBox->resultMoneyが0未満なら
        if (contentGameObj.transform.childCount < 2 ||
            0 > Int32.Parse(specificationBoxGameObj.transform.Find("resultMoneyValueStr").GetComponent<Text>().text.Replace("円", "")))
        {
            // 購入ボタンを防ぐ
            canvasGameObj.transform.Find("orderConfirmButton").GetComponent<Button>().interactable = false;
        }
        else
        {
            // 購入ボタンをいかす
            canvasGameObj.transform.Find("orderConfirmButton").GetComponent<Button>().interactable = true;
        }

        // 店員さんの挨拶イベントが終わるとシーン転換
        if ("EV013".Equals(canvasGameObj.transform.Find("eventCodeSW").GetComponent<Text>().text) &&
            "Y".Equals(canvasGameObj.transform.Find("fadeOutPersistEventCheck").GetComponent<Text>().text))
        {
            sceneTransitionManager.LoadTo("AtHomeScene");
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
            if (hit.collider != null && !clickBlockGameObj.activeSelf)
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

        canvasGameObj.transform.Find("specificationBox").transform.Find("moneyValueStr").GetComponent<Text>().text = playerData.money + "円";
        canvasGameObj.transform.Find("specificationBox").transform.Find("totalPriceValueStr").GetComponent<Text>().text = "0円";
        canvasGameObj.transform.Find("specificationBox").transform.Find("resultMoneyValueStr").GetComponent<Text>().text = playerData.money + "円";



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

                // アイテムがないならアイテム追加ボタンを防ぐ
                if (convenienceItemDataArray[i].itemQuantity < 1)
                {
                    contentGameObj.transform.Find("itemBox" + i).transform.Find("itemAddButton").GetComponent<Button>().interactable = false;
                    contentGameObj.transform.Find("itemBox" + i).transform.Find("itemAddButton").transform.Find("Text").GetComponent<Text>().text = "売り切れ";
                }
                    
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

                // アイテムがないならアイテム追加ボタンを防ぐ(最初はitemBox0からのアイテム追加ボタン変更点が反映されている)
                if (convenienceItemDataArray[i].itemQuantity < 1)
                {
                   itemBox.transform.Find("itemAddButton").GetComponent<Button>().interactable = false;
                   itemBox.transform.Find("itemAddButton").transform.Find("Text").GetComponent<Text>().text = "売り切れ";
                }
                // アイテムがいるならアイテム追加ボタンをいかす
                else
                {
                   itemBox.transform.Find("itemAddButton").GetComponent<Button>().interactable = true;
                   itemBox.transform.Find("itemAddButton").transform.Find("Text").GetComponent<Text>().text = "追加";
                }
            }
        }
    }

    public void ResetUI()
    {
        // orderBox初期化
        for (int i=1; i< contentGameObj.transform.childCount; i++)
        {
            Destroy(contentGameObj.transform.GetChild(i).gameObject);
        }

        // specificationBox初期化
        PlayerData playerData = playerSaveDataManager.LoadPlayerData();
        specificationBoxGameObj.transform.Find("moneyValueStr").GetComponent<Text>().text = playerData.money+"円";
        specificationBoxGameObj.transform.Find("totalPriceValueStr").GetComponent<Text>().text = "0円";
        specificationBoxGameObj.transform.Find("resultMoneyValueStr").GetComponent<Text>().text = playerData.money + "円";

        // コンビニで販売するアイテムリストを読み込む(json)
        ConvenienceItemData[] convenienceItemDataArray = convenienceItemSetManager.GetConvenienceJsonFile();


        for (int i = 0; i < convenienceItemDataArray.Length; i++)
        {
            // 最初のアイテムなら && アイテムがセール状態なら
            if (convenienceItemDataArray[i].itemSale.Equals("Y"))
            {
                Debug.Log("reset ui itemName: " + convenienceItemDataArray[i].itemName);
                Debug.Log("reset ui itemqty: " + convenienceItemDataArray[i].itemQuantity);

                // オブジェクトを作らないで情報だけ移す(itemBox0)
                Texture2D texture = Resources.Load(convenienceItemDataArray[i].itemImagePath, typeof(Texture2D)) as Texture2D;
                Rect rect = new Rect(0, 0, texture.width, texture.height);
                menuBoxContentGameObj.transform.Find("itemBox" + i).transform.Find("itemImage").GetComponent<Image>().sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));

                menuBoxContentGameObj.transform.Find("itemBox" + i).transform.Find("itemName").GetComponent<Text>().text = convenienceItemDataArray[i].itemName;
                menuBoxContentGameObj.transform.Find("itemBox" + i).transform.Find("itemDescription").GetComponent<Text>().text = convenienceItemDataArray[i].itemDescription;
                menuBoxContentGameObj.transform.Find("itemBox" + i).transform.Find("itemQuantity").GetComponent<Text>().text = convenienceItemDataArray[i].itemQuantity.ToString();
                menuBoxContentGameObj.transform.Find("itemBox" + i).transform.Find("itemPrice").GetComponent<Text>().text = convenienceItemDataArray[i].itemPrice.ToString();

                // ボタンクリックイベントは最初のときすでに設定されているので追加しない

                // アイテムがないならアイテム追加ボタンを防ぐ
                if (convenienceItemDataArray[i].itemQuantity < 1)
                {
                    menuBoxContentGameObj.transform.Find("itemBox" + i).transform.Find("itemAddButton").GetComponent<Button>().interactable = false;
                    menuBoxContentGameObj.transform.Find("itemBox" + i).transform.Find("itemAddButton").transform.Find("Text").GetComponent<Text>().text = "売り切れ";
                }
                // アイテムがないならアイテム追加ボタンを防ぐ
                else
                {
                    menuBoxContentGameObj.transform.Find("itemBox" + i).transform.Find("itemAddButton").GetComponent<Button>().interactable = true;
                    menuBoxContentGameObj.transform.Find("itemBox" + i).transform.Find("itemAddButton").transform.Find("Text").GetComponent<Text>().text = "追加";
                }
            }
        }

        // cursorとPanelテキスト初期化
        ItemClickPanelUISetting(true);
    }

    public void SetActiveUI(bool sw)
    {
        canvasGameObj.transform.Find("menuButton").GetComponent<Button>().interactable = sw;
        canvasGameObj.transform.Find("nextButton").GetComponent<Button>().interactable = sw;
        canvasGameObj.transform.Find("menuBox").gameObject.SetActive(sw);
        canvasGameObj.transform.Find("orderBox").gameObject.SetActive(sw);
        canvasGameObj.transform.Find("orderConfirmButton").gameObject.SetActive(sw);
        canvasGameObj.transform.Find("specificationBox").gameObject.SetActive(sw);
    }

    public void LoadEventAndShow(string eventCode)
    {
        EventListData[] loadedEventListData = playerSaveDataManager.LoadedEventListData();
        EventListData eventItem = eventManager.FindEventByCode(loadedEventListData, eventCode);
        List<string[]> scriptList = eventManager.ScriptSaveToList(eventItem);
        chatManager.ShowDialogue(scriptList, eventCode);
    }

    public void ClickgoToHomeConfirmButton()
    {
        canvasGameObj.transform.Find("goToHomeAlertBox").gameObject.SetActive(false);
        SetActiveUI(false);

        playerData = playerSaveDataManager.LoadPlayerData();
        
            // 現在プレイヤーデータの時間を変更する(add minute)
        DateTime addedDateTime = utilManager.TimeCal(playerData.time, 45);
        playerData.time = addedDateTime.Hour.ToString("D2") + ":" + addedDateTime.Minute.ToString("D2");
        playerSaveDataManager.SavePlayerData(playerData);

        // イベントを呼び出す(店員さん挨拶イベント)
        canvasGameObj.transform.Find("eventCodeSW").GetComponent<Text>().text = "EV013";
        LoadEventAndShow("EV013");
    }

    public void ClickNextButton()
    {
        canvasGameObj.transform.Find("goToHomeAlertBox").gameObject.SetActive(true);
        SetActiveUI(false);
    }

    public void ClickgoToHomeCancelButton()
    {
        canvasGameObj.transform.Find("goToHomeAlertBox").gameObject.SetActive(false);
        SetActiveUI(true);
    }

    public void ClickOrderConfirmBtn()
    {
        ClickBlockAndAlertBoxSetActive(true);
    }

    public void ClickOrderConfirmAlertBoxConfirmBtn()
    {
        ClickBlockAndAlertBoxSetActive(false);
        // ★itemListDataを共有することでitemListData(オブジェクト)が変更されたりしたらあとの作業で影響があるので
        // ★別のオブジェクトとリストに分けて詰める
        ItemListData itemListDataForPlayer = null;
        ItemListData itemListDataForConvenience = null;
        List<ItemListData> itemListDataList = new List<ItemListData>();
        List<ItemListData> itemListDataListForConvenience = new List<ItemListData>();
        // orderBoxにあるアイテムをプレイヤーに移す(json)
        // orderBox -> Contentの子が2以上なら(defaultを含め)
        if(contentGameObj.transform.childCount > 1)
        {
            for(int i=1; i< contentGameObj.transform.childCount; i++)
            {
                itemListDataForPlayer = new ItemListData();
                itemListDataForPlayer.itemName = contentGameObj.transform.Find("itemBox" + i).transform.Find("itemName").GetComponent<Text>().text;
                itemListDataForPlayer.itemDescription = contentGameObj.transform.Find("itemBox" + i).transform.Find("itemDescription").GetComponent<Text>().text;
                itemListDataForPlayer.quantity = Int32.Parse(contentGameObj.transform.Find("itemBox" + i).transform.Find("itemQty").GetComponent<Text>().text);
                itemListDataForPlayer.keyItem = "N";

                itemListDataList.Add(itemListDataForPlayer);

                itemListDataForConvenience = new ItemListData();
                itemListDataForConvenience.itemName = contentGameObj.transform.Find("itemBox" + i).transform.Find("itemName").GetComponent<Text>().text;
                itemListDataForConvenience.itemDescription = contentGameObj.transform.Find("itemBox" + i).transform.Find("itemDescription").GetComponent<Text>().text;
                itemListDataForConvenience.quantity = Int32.Parse(contentGameObj.transform.Find("itemBox" + i).transform.Find("itemQty").GetComponent<Text>().text);
                itemListDataForConvenience.keyItem = "N";

                itemListDataListForConvenience.Add(itemListDataForConvenience);

                Debug.Log("added itemName: " + itemListDataForPlayer.itemName);
                
                
            }
            playerSaveDataManager.SaveItemListData(itemListDataList.ToArray());

            // specificationBoxのresultMoneyValueStrをプレイヤー所持金に反映する
            string resultMoney = specificationBoxGameObj.transform.Find("resultMoneyValueStr").GetComponent<Text>().text.Replace("円", "");
            playerData.money = resultMoney;
            playerSaveDataManager.SavePlayerData(playerData);

            // 購買したアイテムの数反映(convenienceItem.json)
            convenienceItemSetManager.SetConvenienceJsonFile(itemListDataListForConvenience.ToArray());

            // UIをリセットする
            ResetUI();

        }
    }

    public void ClickOrderConfirmAlertBoxCancelBtn()
    {
        ClickBlockAndAlertBoxSetActive(false);
    }

    public void ClickBlockAndAlertBoxSetActive(bool sw)
    {
        // 外部クリック防ぐオブジェクトを管理
        canvasGameObj.transform.Find("clickBlock").gameObject.SetActive(sw);
        canvasGameObj.transform.Find("orderConfirmAlertBox").gameObject.SetActive(sw);
    }

    public void ClickItemAddButton()
    {
        GameObject canvasGameObj = GameObject.Find("Canvas");
        Transform addItemAlertBoxTransform = canvasGameObj.transform.Find("addItemAlertBox");
        // クリックしたオブジェクトを取り出す
        Transform selectedItemBox = EventSystem.current.currentSelectedGameObject.transform.parent;
        Debug.Log("itemBoxTransform: " + selectedItemBox);

        SetActiveUI(false);
        addItemAlertBoxTransform.gameObject.SetActive(true);

        // 選択されたアイテムの情報を移す
        addItemAlertBoxTransform.transform.Find("itemImage").GetComponent<Image>().sprite = selectedItemBox.Find("itemImage").GetComponent<Image>().sprite;
        addItemAlertBoxTransform.transform.Find("itemName").GetComponent<Text>().text = selectedItemBox.Find("itemName").GetComponent<Text>().text;
        addItemAlertBoxTransform.transform.Find("itemDescription").GetComponent<Text>().text = selectedItemBox.Find("itemDescription").GetComponent<Text>().text;
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
        GameObject specificationBoxGameObj = canvasGameObj.transform.Find("specificationBox").gameObject;

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
        itemBoxGameObj.transform.Find("itemDescription").GetComponent<Text>().text = addItemGameObj.transform.Find("itemDescription").GetComponent<Text>().text;
        itemBoxGameObj.transform.Find("itemQty").GetComponent<Text>().text = addItemGameObj.transform.Find("itemQuantity").GetComponent<Text>().text;
        itemBoxGameObj.transform.Find("itemPrice").GetComponent<Text>().text = addItemGameObj.transform.Find("itemPrice").GetComponent<Text>().text;

        // specificationBoxに金額を反映する
        int orderedPrice = Int32.Parse(itemBoxGameObj.transform.Find("itemPrice").GetComponent<Text>().text.Replace("円", ""));
        orderedPrice += Int32.Parse(specificationBoxGameObj.transform.Find("totalPriceValueStr").GetComponent<Text>().text.Replace("円", ""));
        specificationBoxGameObj.transform.Find("totalPriceValueStr").GetComponent<Text>().text = orderedPrice.ToString() + "円";
        int money = Int32.Parse(specificationBoxGameObj.transform.Find("moneyValueStr").GetComponent<Text>().text.Replace("円", ""));
        int resultMoney = (money - orderedPrice);
        specificationBoxGameObj.transform.Find("resultMoneyValueStr").GetComponent<Text>().text = resultMoney.ToString() + "円";
        
        // itemDeleteButton AddListener(orderBox->itemBox削除)
        itemBoxGameObj.transform.Find("itemDeleteButton").GetComponent<Button>().onClick.AddListener(() => ClickItemDeleteButton(itemBoxGameObj, selectedItemBox, specificationBoxGameObj));

        // onClick.AddListenerのイベントが積もるのを防止する
        RemoveButtonListeners();


        

    }

    public void ClickItemDeleteButton(GameObject itemBoxGameObj, Transform selectedItemBox, GameObject specificationBoxGameObj)
    {
        // orderBoxのアイテムを削除する
        Destroy(itemBoxGameObj);
        // menuBoxのアイテム追加ボタンをいかす
        selectedItemBox.transform.Find("itemAddButton").GetComponent<Button>().interactable = true;
        selectedItemBox.transform.Find("itemAddButton").transform.Find("Text").GetComponent<Text>().text = "追加";
        // specificationBoxで金額を減らす
        int totalPrice = Int32.Parse(specificationBoxGameObj.transform.Find("totalPriceValueStr").GetComponent<Text>().text.Replace("円", ""));
        int orderedPrice = Int32.Parse(itemBoxGameObj.transform.Find("itemPrice").GetComponent<Text>().text.Replace("円", ""));
        int newTotalPrice = (totalPrice - orderedPrice);
        specificationBoxGameObj.transform.Find("totalPriceValueStr").GetComponent<Text>().text = newTotalPrice.ToString() + "円";
        int resultMoney = Int32.Parse(specificationBoxGameObj.transform.Find("resultMoneyValueStr").GetComponent<Text>().text.Replace("円", ""));
        specificationBoxGameObj.transform.Find("resultMoneyValueStr").GetComponent<Text>().text = (resultMoney += orderedPrice).ToString() + "円";
    }

    public void ClickCancelButton(Transform selectedItemBox)
    {
        GameObject canvasGameObj = GameObject.Find("Canvas");
        Transform addItemAlertBoxTransform = canvasGameObj.transform.Find("addItemAlertBox");

        // onClick.AddListenerのイベントが積もるのを防止する
        RemoveButtonListeners();

        SetActiveUI(true);
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
