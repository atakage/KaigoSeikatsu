using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCheckManager : MonoBehaviour
{
    public PlayerSaveDataManager playerSaveDataManager;
    public PlayerData playerData;
    public ItemListData[] itemListData;

    public int itemQuantity;
    public int itmeSlotIndex;
    public int onePageItemQty = 6;
    public int itemSlotPage;
    public int itemSlotPageIndex;
    public int loadItemIndex = 0;
  
    // Start is called before the first frame update
    void Start()
    {

        PlayerSaveDataManager playerSaveDataManager = new PlayerSaveDataManager();
        ItemListData[] itemListData2 = new ItemListData[2];
        itemListData2[0] = new ItemListData();
         itemListData2[0].itemName = "android";
        itemListData2[0].itemDescription = "this is android";
        itemListData2[0].quantity = 30;
        playerSaveDataManager.SaveItemListData(itemListData2);

        Debug.Log("ItemCheckManager START");
        playerSaveDataManager = new PlayerSaveDataManager();

        // プレイヤーが持っているアイテムを読み出す
        //playerData = playerSaveDataManager.LoadPlayerData();
        itemListData = playerSaveDataManager.LoadItemListData();

        if(itemListData != null && itemListData.Length > 0) ItemSlotPage();


    }

    // クリックしたらアイテムを表示
    public void DisplayItem(int slotIndex)
    {

    }

    // スロットでアイテムを表現するために必要な作業
    public void ItemSlotPage()
    {
        Debug.Log("ItemSlotPage START");
        // 現在持っているアイテムの数
        itemQuantity = itemListData.Length;
        Debug.Log("itemQuantity: " + itemQuantity);
        // スロットの全体ページ数を算出する、(現在持っているアイテムの数)/(1ページに表現するアイテム数)
        try
        {
            float itemSlotPageF = Mathf.Ceil(itemQuantity / onePageItemQty);
            itemSlotPage = (int)itemSlotPageF;
        }
        // dividezeroException防止 
        catch
        {
            itemSlotPage = 1;
        }

        Texture2D texture = null;
        //ロードしたアイテムをスロットに配置する
        for (int i=0; i< itemQuantity; i++)
        {
            //イメージをロード
            texture = new Texture2D(0, 0);
            string imgPath = "img/item/"+ itemListData[i].itemName;

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

            // アイテム情報
            GameObject.Find("item" + i).transform.Find("itemName").GetComponent<Text>().text = itemListData[i].itemName;
            GameObject.Find("item" + i).transform.Find("itemQty").GetComponent<Text>().text = itemListData[i].quantity.ToString();
            GameObject.Find("item" + i).transform.Find("itemDesc").GetComponent<Text>().text = itemListData[i].itemDescription;
        }

        // 最初はindex0のアイテム情報を基準にする
        GameObject.Find("Panel").transform.Find("Text").GetComponent<Text>().text =
        "[ " + "<color=#93DAFF>" + itemListData[0].itemName + "</color>" + "(" + itemListData[0].quantity + ")" + " ]" +
        "\n" +
        itemListData[0].itemDescription;
        
        ;
        Debug.Log("itemSlotPage: " + itemSlotPage);
    }

}
