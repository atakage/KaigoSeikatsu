using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


[System.Serializable]
public class PlayerData
{
    public int money;
    public int progressWithTestA;
}


[System.Serializable]
public class ItemListData
{
    public string itemName;
    public int quantity;
}



public class PlayerSaveDataManager : MonoBehaviour
{

    public void SaveItemListData(ItemListData[] itemListData)
    {
        // セーブ前にデータをロードする
        ItemListData[] loadedItemListData = LoadItemListData();
        
        // もしロードデータがなかったら新しいアイテムリストをセーブする
        if(loadedItemListData == null)
        {
            Debug.Log("LOADED ITEM LIST NULL");
            Debug.Log("ITEMLISTDATAPARAM LENGTH: " + itemListData.Length);

            string itemAsStr = JsonHelper.ToJson(itemListData, true);
            Debug.Log("SAVEDATA: " + itemAsStr);
            File.WriteAllText(Application.dataPath + "/Resources/saveData/testPlayerItem.json", itemAsStr);
        }
        // ロードデータがあるなら既存アイテムリストに新しいアイテムリストを追加してセーブする
        else
        {
            Debug.Log("LOADED ITEM LIST LENGTH: " + loadedItemListData.Length);
            
            List<ItemListData> mergedItemList = new List<ItemListData>();


            // 新しいアイテムの中にロードしたアイテムと重なるものがあったらアイテム数だけ渡して名前はnull処理する
            for (int i = 0; i < loadedItemListData.Length; i++)
            {
                for(int j = 0; j < itemListData.Length; j++)
                {
                    if (loadedItemListData[i].itemName.Equals(itemListData[j].itemName))
                    {
                        loadedItemListData[i].quantity = itemListData[j].quantity;
                        itemListData[j].itemName = null;
                        itemListData[j].quantity = 0;
                    }
                }
            }

            foreach (ItemListData loadedData in loadedItemListData)
            {
                mergedItemList.Add(loadedData);
            }

            foreach (ItemListData loadedData in itemListData)
            {
                // 新しいアイテムの中名前がnullにできたものはリストに追加しない
                if(loadedData.itemName != null)
                {
                    mergedItemList.Add(loadedData);
                }
            }

            ItemListData[] mergedItemArr = mergedItemList.ToArray();

            string itemAsStr = JsonHelper.ToJson(mergedItemArr, true);
            Debug.Log("SAVEDATA: " + itemAsStr);
            File.WriteAllText(Application.dataPath + "/Resources/saveData/testPlayerItem.json", itemAsStr);


        }
    }

    public ItemListData[] LoadItemListData()
    {
        ItemListData[] itemListData;

        try
        {
            string itemAsStr = File.ReadAllText(Application.dataPath + "/Resources/saveData/testPlayerItem.json");
            itemListData = JsonHelper.FromJson<ItemListData>(itemAsStr);
            Debug.Log("SUCCESS LOAD");
        }
        catch
        {
            Debug.Log("LOADITEMLISTDATA ERROR");
            itemListData = null;
        }

        return itemListData;
    }

    public void SavePlayerData(PlayerData playerData)
    {
       
        string strPlayerData = JsonConvert.SerializeObject(playerData);
        Debug.Log("SAVEDATA: " + strPlayerData.ToString());
        File.WriteAllText(Application.dataPath + "/Resources/saveData/testPlayerData.json", strPlayerData);
    }

    public PlayerData LoadPlayerData()
    {
        string dataStr = File.ReadAllText(Application.dataPath + "/Resources/saveData/testPlayerData.json");
        PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(dataStr);
        Debug.Log("LOADDATA: " + playerData.money +","+ playerData.progressWithTestA);

        return playerData;
    }
}
