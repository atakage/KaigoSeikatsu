using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ItemCheckManager : MonoBehaviour
{
    public PlayerSaveDataManager playerSaveDataManager;
    public PlayerItemUpdateManager playerItemUpdateManager;
    public ItemSelectManager itemSelectManager;
    public SceneTransitionManager sceneTransitionManager;
    public CSVManager csvManager;
    public ItemUseManager itemUseManager;
    public PlayerData playerData;
    public ItemListData[] allItemListData;
    public ItemListData[] itemListData;

    public int allItemQuantity;
    public int itemQuantity;
    public int itmeSlotIndex;
    public int onePageItemQty;
    public int itemSlotPage;        // 全体ページ
    public static int itemSelectIndex;
    public int loadItemPage;        // 現在ページ
  
    // Start is called before the first frame update
    void Start()
    {

        playerSaveDataManager = new PlayerSaveDataManager();
        sceneTransitionManager = new SceneTransitionManager();
        playerItemUpdateManager = new PlayerItemUpdateManager();
        csvManager = new CSVManager();
        itemUseManager = new ItemUseManager();

        ItemListData[] itemListData2 = new ItemListData[2];
        itemListData2[0] = new ItemListData();
        itemListData2[0].itemName = "エナジードリンク";
        itemListData2[0].itemDescription = "飲むと少しだけ元気が出る";
        itemListData2[0].quantity = 30;
        itemListData2[0].keyItem = "N";
        
        itemListData2[1] = new ItemListData();
        itemListData2[1].itemName = "名刺";
        itemListData2[1].itemDescription = "介護福祉士の名刺だ";
        itemListData2[1].quantity = 1;
        itemListData2[1].keyItem = "Y";
        /*
        itemListData2[2] = new ItemListData();
        itemListData2[2].itemName = "カタリナ";
        itemListData2[2].itemDescription = "いいところ";
        itemListData2[2].quantity = 1;
        itemListData2[2].keyItem = "N";

        itemListData2[3] = new ItemListData();
        itemListData2[3].itemName = "財布";
        itemListData2[3].itemDescription = "お金を保つ";
        itemListData2[3].quantity = 2;
        itemListData2[3].keyItem = "N";

        
        itemListData2[4] = new ItemListData();
        itemListData2[4].itemName = "侍";
        itemListData2[4].itemDescription = "誇り高い";
        itemListData2[4].quantity = 2;
        itemListData2[4].keyItem = "Y";

        itemListData2[5] = new ItemListData();
        itemListData2[5].itemName = "キーブレード";
        itemListData2[5].itemDescription = "勇気の象徴";
        itemListData2[5].quantity = 2;
        itemListData2[5].keyItem = "N";

        itemListData2[6] = new ItemListData();
        itemListData2[6].itemName = "花束";
        itemListData2[6].itemDescription = "きれいだ";
        itemListData2[6].quantity = 2;
        itemListData2[6].keyItem = "N";
        */
        
        playerSaveDataManager.SaveItemListData(itemListData2);

        Debug.Log("ItemCheckManager START");
        playerSaveDataManager = new PlayerSaveDataManager();
        itemSelectManager = new ItemSelectManager();

        onePageItemQty = 6;
        loadItemPage = 1;

        // 戻るボタンの目的地を設定
        if(GameObject.Find("SceneChangeManager") != null)
        {
            string goBackScene = GameObject.Find("SceneChangeManager").transform.Find("SceneChangeCanvas").transform.Find("destinationFrom-toItemCheckScene").GetComponent<Text>().text;
            GameObject.Find("Canvas").transform.Find("returnButton").GetComponent<Button>().onClick.AddListener(delegate { ClickReturnButton(goBackScene); });
        }


        // ボタンにmethodをつける
        GameObject.Find("Canvas").transform.Find("itemUseButton").GetComponent<Button>().onClick.AddListener(ItemUseButtonClick);
        GameObject.Find("Canvas").transform.Find("itemUseAlertBox").transform.Find("useButton").GetComponent<Button>().onClick.AddListener(ClickAlertUseButton);
        GameObject.Find("Canvas").transform.Find("itemUseAlertBox").transform.Find("cancelButton").GetComponent<Button>().onClick.AddListener(ClickAlertCancelButton);
        GameObject.Find("Canvas").transform.Find("CanNotUseAlertBox").transform.Find("cancelButton").GetComponent<Button>().onClick.AddListener(ClickAlertCancelButton);
        GameObject.Find("Canvas").transform.Find("itemDropButton").GetComponent<Button>().onClick.AddListener(ClickItemDropButton);
        GameObject.Find("Canvas").transform.Find("itemDropAlertBox").transform.Find("itemQtyMinusBtn").GetComponent<Button>().onClick.AddListener(ClickItemQtyMinusBtn);
        GameObject.Find("Canvas").transform.Find("itemDropAlertBox").transform.Find("itemQtyPlusBtn").GetComponent<Button>().onClick.AddListener(ClickItemQtyPlusBtn);
        GameObject.Find("Canvas").transform.Find("itemDropAlertBox").transform.Find("useButton").GetComponent<Button>().onClick.AddListener(ClickItemDropUseButton);
        GameObject.Find("Canvas").transform.Find("itemDropAlertBox").transform.Find("cancelButton").GetComponent<Button>().onClick.AddListener(ClickAlertCancelButton);
        GameObject.Find("Canvas").transform.Find("CanNotDropAlertBox").transform.Find("cancelButton").GetComponent<Button>().onClick.AddListener(ClickAlertCancelButton);
        GameObject.Find("itemPageCanvas").transform.Find("nextButton").GetComponent<Button>().onClick.AddListener(ClickNextPage);
        GameObject.Find("itemPageCanvas").transform.Find("prevButton").GetComponent<Button>().onClick.AddListener(ClickPrevPage);

        // 全体アイテムリスト
        allItemListData = playerSaveDataManager.LoadItemListData();

        // 現在ページのアイテムリスト
        itemListData = playerSaveDataManager.LoadItemListData(loadItemPage);

        if(itemListData != null && itemListData.Length > 0) ItemSlotPage();

        // アイテムスロットUI初期化
        itemSelectManager.DisplayItemSlotUI(true);
        Debug.Log("SELECTED ITEM INDEX: " + itemSelectIndex);

    }

    public void ClickItemDropUseButton()
    {
        // 全体アイテムリスト
        allItemListData = playerSaveDataManager.LoadItemListData();

            // 選択されたitemNameとitemQtyを移す
        string dropItemName = GameObject.Find("Canvas").transform.Find("itemDropAlertBox").transform.Find("itemName").GetComponent<Text>().text;
        string dropItemQty = GameObject.Find("Canvas").transform.Find("itemDropAlertBox").transform.Find("itemQty").GetComponent<Text>().text;

        // アイテムを捨てる
        itemUseManager.DropItem(dropItemName, dropItemQty, allItemListData);

        // UIをセットする
        GameObject.Find("Canvas").transform.Find("itemDropAlertBox").gameObject.SetActive(false);
        UseItemAndRefreshItemSlotUI();

        // テキスト適用
        GameObject.Find("Canvas").transform.Find("Panel").transform.Find("Text").GetComponent<Text>().text = dropItemName + "を" + dropItemQty + "個捨てた!";
    }

    public void ClickItemDropButton()
    {
        

        // 選択されたアイテムがkeyじゃないなら
        if (GameObject.Find("Canvas").transform.Find("selectedItem").transform.Find("keyItem").GetComponent<Text>().text.Equals("N"))
        {

            // ray(collider)クリックを防止する
            GameObject.Find("Canvas").transform.Find("preventClickScreen").gameObject.SetActive(true);

            GameObject.Find("Canvas").transform.Find("itemDropAlertBox").gameObject.SetActive(true);
            // sceneにあるボタンクリック機能を防ぐ
            ItemCheckSceneButtonInteractable(false);

            // 選択されたアイテムの情報を移す
            GameObject.Find("Canvas").transform.Find("itemDropAlertBox").transform.Find("itemName").GetComponent<Text>().text
                = GameObject.Find("Canvas").transform.Find("selectedItem").transform.Find("itemName").GetComponent<Text>().text;
            // 選択されたアイテムの情報を移す
            GameObject.Find("Canvas").transform.Find("itemDropAlertBox").transform.Find("itemQty").GetComponent<Text>().text = "1";


        }
        // 選択されたアイテムがkeyなら
        else
        {
            // ray(collider)クリックを防止する
            GameObject.Find("Canvas").transform.Find("preventClickScreen").gameObject.SetActive(true);

            GameObject.Find("Canvas").transform.Find("CanNotDropAlertBox").gameObject.SetActive(true);
            // sceneにあるボタンクリック機能を防ぐ
            ItemCheckSceneButtonInteractable(false);
        }
        
    }

    public void ClickItemQtyMinusBtn()
    {
        int itemQtyInt = Int32.Parse(GameObject.Find("Canvas").transform.Find("itemDropAlertBox").transform.Find("itemQty").GetComponent<Text>().text);

        // alertのitemQtyが2以上なら
        if (itemQtyInt > 1)
        {
            // itemQtyを一つ減らす
            itemQtyInt -= 1;
            GameObject.Find("Canvas").transform.Find("itemDropAlertBox").transform.Find("itemQty").GetComponent<Text>().text = itemQtyInt.ToString();
        }
    }

    public void ClickItemQtyPlusBtn()
    {
        int itemQtyMax = Int32.Parse(GameObject.Find("Canvas").transform.Find("selectedItem").transform.Find("itemQty").GetComponent<Text>().text);
        int itemQtyInt = Int32.Parse(GameObject.Find("Canvas").transform.Find("itemDropAlertBox").transform.Find("itemQty").GetComponent<Text>().text);

        // alertのitemQtyが最大数未満なら
        if(itemQtyMax > itemQtyInt)
        {
            // itemQtyを一つ足す
            itemQtyInt += 1;
            GameObject.Find("Canvas").transform.Find("itemDropAlertBox").transform.Find("itemQty").GetComponent<Text>().text = itemQtyInt.ToString();
        }
    }

    public void ClickAlertUseButton()
    {
        // 使うアイテムの名前をとる
        string useItemName = GameObject.Find("Canvas").transform.Find("selectedItem").transform.Find("itemName").GetComponent<Text>().text;
        if(useItemName != null && !useItemName.Equals(""))
        {
            // 全体アイテムリスト(allItemListData)で使うアイテムのquantityを探して一つ減らす
            playerItemUpdateManager.UpdateItemQty(allItemListData, useItemName);

            // ゲーム内全体アイテムリストを読み出す(key=itemName, value=itemName,itemDescription,itemEffect,key)
            Dictionary<string, Dictionary<string, object>> allItemDic = csvManager.GetTxtItemList("AllItem");

            // 使うアイテムの効果を全体アイテムリストから探して適用する
            itemUseManager.UseItem(useItemName, allItemDic);

            // UIをセットする
            GameObject.Find("Canvas").transform.Find("itemUseAlertBox").gameObject.SetActive(false);
            UseItemAndRefreshItemSlotUI();

            // アイテムを使うサウンド++

            // アイテム使用テキスト
            GameObject.Find("Canvas").transform.Find("Panel").transform.Find("Text").GetComponent<Text>().text = useItemName + "を使った!";
        }


    }

    // アイテム使用後
    public void UseItemAndRefreshItemSlotUI()
    {
        // 全体アイテムリスト
        allItemListData = playerSaveDataManager.LoadItemListData();

        // 現在ページのアイテムリスト
        itemListData = playerSaveDataManager.LoadItemListData(1);

        // 1ページから表示する
        loadItemPage = 1;

        // itemSlotCanvasのitemオブジェクトを初期化
        RefreshItemSlot();
        // アイテムクリックUIを初期化
        itemSelectManager.CleanItemSlotUI();

        // アイテム表示
        if (itemListData != null && itemListData.Length > 0) ItemSlotPage();

        // アイテムスロットUI初期化
        //itemSelectManager.DisplayItemSlotUI(true);

        // ボタンをいかす
        ItemCheckSceneButtonInteractable(true);
        // ray(collider)クリックを防止するオブジェクトを隠す
        GameObject.Find("Canvas").transform.Find("preventClickScreen").gameObject.SetActive(false);
    }

    public void ClickAlertCancelButton()
    {
        // クリックしたオブジェクトの母親を探して隠す
        EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false);
        // ray(collider)クリックを防止するオブジェクトを隠す
        GameObject.Find("Canvas").transform.Find("preventClickScreen").gameObject.SetActive(false);
        // sceneにあるボタンをもとに戻す
        ItemCheckSceneButtonInteractable(true);
    }

    public void ItemUseButtonClick()
    {
        Debug.Log("CLICK ItemUseButtonClick");
        // 選択されたアイテムがキーアイテムじゃないなら
        if (GameObject.Find("Canvas").transform.Find("selectedItem").transform.Find("keyItem").GetComponent<Text>().text.Equals("N"))
        {
            // ray(collider)クリックを防止する
            GameObject.Find("Canvas").transform.Find("preventClickScreen").gameObject.SetActive(true);
            // alert
            GameObject.Find("Canvas").transform.Find("itemUseAlertBox").gameObject.SetActive(true);
            // sceneにあるボタンクリック機能を防ぐ
            ItemCheckSceneButtonInteractable(false);
            // 選択されたアイテムの情報を移す
            GameObject.Find("Canvas").transform.Find("itemUseAlertBox").transform.Find("itemName").GetComponent<Text>().text
                = GameObject.Find("Canvas").transform.Find("selectedItem").transform.Find("itemName").GetComponent<Text>().text;
            GameObject.Find("Canvas").transform.Find("itemUseAlertBox").transform.Find("itemQty").GetComponent<Text>().text
                = GameObject.Find("Canvas").transform.Find("selectedItem").transform.Find("itemQty").GetComponent<Text>().text + "個";
        }
        // キーアイテムなら
        else
        {
            // ray(collider)クリックを防止する
            GameObject.Find("Canvas").transform.Find("preventClickScreen").gameObject.SetActive(true);
            // alert
            GameObject.Find("Canvas").transform.Find("CanNotUseAlertBox").gameObject.SetActive(true);
            // sceneにあるボタンクリック機能を防ぐ
            ItemCheckSceneButtonInteractable(false);
        }

    }

    public void ItemCheckSceneButtonInteractable(bool sw)
    {
        GameObject.Find("Canvas").transform.Find("returnButton").GetComponent<Button>().interactable = sw;
        GameObject.Find("Canvas").transform.Find("itemUseButton").GetComponent<Button>().interactable = sw;
        GameObject.Find("Canvas").transform.Find("itemDropButton").GetComponent<Button>().interactable = sw;
        if(GameObject.Find("Canvas").transform.Find("itemPageCanvas").transform.Find("prevButton") != null) GameObject.Find("Canvas").transform.Find("itemPageCanvas").transform.Find("prevButton").GetComponent<Button>().interactable = sw;
        if(GameObject.Find("Canvas").transform.Find("itemPageCanvas").transform.Find("nextButton") != null) GameObject.Find("Canvas").transform.Find("itemPageCanvas").transform.Find("nextButton").GetComponent<Button>().interactable = sw;
    }

    public void ClickReturnButton(string goBackScene)
    {
        // 破壊しない場合増加する
        Destroy(GameObject.Find("SceneChangeManager"));
        sceneTransitionManager.LoadTo(goBackScene);
    }

    // item next page
    public void ClickNextPage()
    {
        RefreshItemSlot();
        playerSaveDataManager = new PlayerSaveDataManager();

        loadItemPage += 1;
        // 現在ページのアイテムリスト
        itemListData = playerSaveDataManager.LoadItemListData(loadItemPage);
        // アイテム表示
        if (itemListData != null && itemListData.Length > 0) ItemSlotPage();
        // アイテムスロットUI初期化
        itemSelectManager.DisplayItemSlotUI(true);
        Debug.Log("SELECTED ITEM INDEX: " + itemSelectIndex);
    }

    // item prev page
    public void ClickPrevPage()
    {
        RefreshItemSlot();
        playerSaveDataManager = new PlayerSaveDataManager();

        loadItemPage -= 1;
        // 現在ページのアイテムリスト
        itemListData = playerSaveDataManager.LoadItemListData(loadItemPage);
        // アイテム表示
        if (itemListData != null && itemListData.Length > 0) ItemSlotPage();
        // アイテムスロットUI初期化
        itemSelectManager.DisplayItemSlotUI(true);
        Debug.Log("SELECTED ITEM INDEX: " + itemSelectIndex);
    }

    // アイテムスロット初期化
    public void RefreshItemSlot()
    {
        for(int i=0; i<6; i++)
        {
            GameObject.Find("itemSlotCanvas").transform.Find("item" + i).gameObject.SetActive(true);

            GameObject.Find("itemSlotCanvas").transform.Find("item" + i).GetComponent<Image>().sprite = null;

            // アイテム情報
            GameObject.Find("item" + i).transform.Find("itemName").GetComponent<Text>().text = "";
            GameObject.Find("item" + i).transform.Find("itemQty").GetComponent<Text>().text = "";
            GameObject.Find("item" + i).transform.Find("itemDesc").GetComponent<Text>().text = "";
            GameObject.Find("item" + i).transform.Find("keyItem").GetComponent<Text>().text = "";

            GameObject.Find("itemSlotCanvas").transform.Find("item" + i).gameObject.SetActive(false);
        }
    }

    // スロットでアイテムを表現するために必要な作業
    public void ItemSlotPage()
    {
        Debug.Log("ItemSlotPage START");
        // 全体アイテム数
        allItemQuantity = allItemListData.Length;
        Debug.Log("allItemQuantity: " + allItemQuantity);
        // 現在ページで持っているアイテムの数
        itemQuantity = itemListData.Length;
        Debug.Log("itemQuantity: " + itemQuantity);
        // スロットの全体ページ数を算出する、(現在持っているアイテムの数)/(1ページに表現するアイテム数)
        try
        {

            float qtyForCeil = (float)allItemQuantity / (float)onePageItemQty;
            Debug.Log("qtyForCeil: " + qtyForCeil);
            float itemSlotPageF = Mathf.Ceil(qtyForCeil);
            Debug.Log("itemSlotPageF: " + itemSlotPageF);
            itemSlotPage = (int)itemSlotPageF;
            Debug.Log("maxItemPage: " + itemSlotPage);
        }
        // dividezeroException防止 
        catch(Exception e)
        {
            itemSlotPage = 1;
            Debug.Log("Exception itemSlotPage!!  " + e);
        }
        


        Texture2D texture = null;
        //ロードしたアイテムをスロットに配置する
        for (int i=0; i< itemQuantity; i++)
        {
            //イメージをロード
            texture = new Texture2D(0, 0);
            string imgPath = "img/item/"+ itemListData[i].itemName;
            Debug.Log("IMGPATH: " + imgPath);

            texture = Resources.Load(imgPath, typeof(Texture2D)) as Texture2D;
            // イメージがないならdefaultイメージを設定
            if(texture == null)
            {
                imgPath = "img/item/unity";
                texture = Resources.Load(imgPath, typeof(Texture2D)) as Texture2D;
            }
           
            // texture to sprite
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            GameObject.Find("itemSlotCanvas").transform.Find("item" + i).GetComponent<Image>().sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));

            // アイテム情報があるオブジェクトは表示する
            GameObject.Find("itemSlotCanvas").transform.Find("item" + i).gameObject.SetActive(true);

            // アイテム情報
            GameObject.Find("itemSlotCanvas").transform.Find("item" + i).transform.Find("itemName").GetComponent<Text>().text = itemListData[i].itemName;
            GameObject.Find("itemSlotCanvas").transform.Find("item" + i).transform.Find("itemQty").GetComponent<Text>().text = itemListData[i].quantity.ToString();
            GameObject.Find("itemSlotCanvas").transform.Find("item" + i).transform.Find("itemDesc").GetComponent<Text>().text = itemListData[i].itemDescription;
            GameObject.Find("itemSlotCanvas").transform.Find("item" + i).transform.Find("keyItem").GetComponent<Text>().text = itemListData[i].keyItem;
        }

        // 最初はindex0のアイテム情報を基準にする
        GameObject.Find("Panel").transform.Find("Text").GetComponent<Text>().text =
        "[ " + "<color=#93DAFF>" + itemListData[0].itemName + "</color>" + "(" + "x" + itemListData[0].quantity + ")" + " ]" +
        "\n" +
        itemListData[0].itemDescription;

        prevNextButtonDisplay();

        // display page
        GameObject.Find("itemPageCanvas").transform.Find("displayPage").GetComponent<Text>().text =
        loadItemPage.ToString() + " / " + itemSlotPage.ToString();
        Debug.Log("itemSlotPage: " + itemSlotPage);
    }

    // 現在ページによるボタンディスプレイ設定
    public void prevNextButtonDisplay()
    {
        // 1ページならprevButtonをfalse
        if(loadItemPage == 1)
        {
            GameObject.Find("itemPageCanvas").transform.Find("prevButton").gameObject.SetActive(false);
        }
        else
        {
            GameObject.Find("itemPageCanvas").transform.Find("prevButton").gameObject.SetActive(true);
        }

        // 現在ページが最大ページと同じなら
        if(loadItemPage == itemSlotPage)
        {
            GameObject.Find("itemPageCanvas").transform.Find("nextButton").gameObject.SetActive(false);
        }
        else
        {
            GameObject.Find("itemPageCanvas").transform.Find("nextButton").gameObject.SetActive(true);
        }
    }
}
