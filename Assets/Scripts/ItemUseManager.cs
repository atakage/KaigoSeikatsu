using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ItemUseManager : MonoBehaviour
{
    private PlayerSaveDataManager playerSaveDataManager;
    public BuildManager buildManager;
    public PlayTimeManager playTimeManager;

    private void Start()
    {
        buildManager = GameObject.Find("BuildManager").GetComponent("BuildManager") as BuildManager;
        playTimeManager = GameObject.Find("PlayTimeManager").GetComponent("PlayTimeManager") as PlayTimeManager;
    }

    public void DropItem(string dropItemName, string dropItemQty, ItemListData[] allItemListData)
    {
        ArrayList itemListDataList = new ArrayList();

        // 現在プレイヤーが持ってるアイテム数くらい繰り返す
        foreach(ItemListData item in allItemListData)
        {
            // 捨てるアイテムの名前と一致したら
            if (item.itemName.Equals(dropItemName))
            {
                // dropItemQtyだけ抜く
                item.quantity -= Int32.Parse(dropItemQty);
            }

            // itemQtyが1以上ならリストに追加する
            if(item.quantity > 0) itemListDataList.Add(item);
        }

        playerSaveDataManager = new PlayerSaveDataManager();
        playerSaveDataManager.SavePlayerItemList((ItemListData[])itemListDataList.ToArray(typeof(ItemListData)));
    }

    public void UseItem(string useItemName, Dictionary<string, Dictionary<string, object>> allItemDic)
    {
        // 使うアイテムの情報を読み出す
        Dictionary<string, object> useItemDic = allItemDic[useItemName];
        // アイテムの効果を読み出す
        string useItemEffect = (string)useItemDic["itemEffect"];
        // アイテム効果を適用
        ApplyItem(useItemEffect);

    }

    public void UseItem(List<string> useItemNameList, Dictionary<string, Dictionary<string, object>> allItemDic)
    {
        // アイテム数だけ繰り返す
        foreach(string useItemName in useItemNameList)
        {
            // 使うアイテムの情報を読み出す
            Dictionary<string, object> useItemDic = allItemDic[useItemName];
            // アイテムの効果を読み出す
            string useItemEffect = (string)useItemDic["itemEffect"];
            // アイテム効果を適用
            ApplyItem(useItemEffect);
        }
    }

    public void ApplyItem(string useItemEffectParam)
    {
        string[] useItemEffectArray = useItemEffectParam.Split('|');
        // アイテムの効果を次第に適用
        foreach(string useItemEffect in useItemEffectArray)
        {
            Debug.Log("useItemEffect: " + useItemEffect);

            // プレイヤーの情報を読み出す
            PlayerSaveDataManager playerSaveDataManager = new PlayerSaveDataManager();
            PlayerData playerData = playerSaveDataManager.LoadPlayerData();

            // itemEffectを分ける
            string itemEffect = useItemEffect.Substring(0, 2); // SA:satisfaction, FA:fatigue
            string itemOperator = useItemEffect.Substring(2, 1); // add or subtract
            int itemValue = Convert.ToInt32(useItemEffect.Substring(3)); // value

            // satisfactionなら
            if ("SA".Equals(itemEffect))
            {
                // plus(+)なら
                if ("+".Equals(itemOperator))
                {
                    // satisfactionをvalueくらい足す
                    playerData.satisfaction += itemValue;
                }
                // それ以外(minus)なら
                else
                {
                    playerData.satisfaction -= itemValue;
                }
            }
            // fatigueなら
            else if ("FA".Equals(itemEffect))
            {
                // plus(+)なら
                if ("+".Equals(itemOperator))
                {
                    // fatigueをvalueくらい足す
                    playerData.fatigue += itemValue;
                }
                // それ以外(minus)なら
                else
                {
                    playerData.fatigue -= itemValue;
                }
            }
            // feelingなら
            else if ("FL".Equals(itemEffect))
            {
                // plus(+)なら
                if ("+".Equals(itemOperator))
                {
                    // feelingをvalueくらい足す
                    playerData.feeling += itemValue;
                }
                // それ以外(minus)なら
                else
                {
                    playerData.feeling -= itemValue;
                }
            }

            // 2021.11.11 追加
            // プレイ時間
            playTimeManager = GameObject.Find("PlayTimeManager").GetComponent("PlayTimeManager") as PlayTimeManager;
            playerData.playTime = playTimeManager.playTime;

            // 変更されたプレイヤーの情報をセーブする
            playerSaveDataManager.SavePlayerData(playerData);
        }
    }
}
