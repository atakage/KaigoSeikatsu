using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        ItemSlotPage();

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

        //ロードしたアイテムをスロットに配置する
        for(int i=0; i< )


        Debug.Log("itemSlotPage: " + itemSlotPage);
    }

}
