using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class PlayerItemUpdateManager : MonoBehaviour
{
    public PlayerSaveDataManager playerSaveDataManager;
    public BuildManager buildManager;
    private void Start()
    {
        buildManager = GameObject.Find("BuildManager").GetComponent("BuildManager") as BuildManager;
    }
    public void UpdateItemQty(ItemListData[] allItemListData, string itemName)
    {
        playerSaveDataManager = new PlayerSaveDataManager();

        List<ItemListData> updatedItemList = new List<ItemListData>();
        ItemListData itemListData = null;

        // アイテム数くらい繰り返す
        foreach(ItemListData item in allItemListData)
        {
            itemListData = new ItemListData();

            // 使うアイテムの同じなら
            if (itemName.Equals(item.itemName))
            {
                // アイテムの数を一つ減らす
                // 減らした数が0ならアイテム込みを中断する
                if( 1 > item.quantity - 1)
                {
                    continue;
                }
                // 減らした数が1以上ならアイテムを込める
                else
                {
                    itemListData.itemName = item.itemName;
                    itemListData.itemDescription = item.itemDescription;
                    itemListData.quantity = item.quantity - 1;
                    itemListData.keyItem = item.keyItem;

                    updatedItemList.Add(itemListData);
                }
            }
            //使うアイテムのじゃないなら
            else
            {
                // アイテムをそのまま込める
                itemListData.itemName = item.itemName;
                itemListData.itemDescription = item.itemDescription;
                itemListData.quantity = item.quantity;
                itemListData.keyItem = item.keyItem;

                updatedItemList.Add(itemListData);
            }
        }

        // 新しいアイテムリストファイルをセーブ
        if (updatedItemList != null && updatedItemList.Count > 0) playerSaveDataManager.SavePlayerItemList(updatedItemList.ToArray());
    }
}
