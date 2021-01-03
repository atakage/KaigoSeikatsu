using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;


[System.Serializable]
public class PlayerData
{
    public int money;
    public string time;
    public string[] eventCodeArray; // クリアしたイベント
    public int progress; // 進行度
    public int fatigue; // 疲労、ゼロになったらゲームオーバー
    public int satisfaction; // ending
    public int action; // 行動力
}


[System.Serializable]
public class ItemListData
{
    public string itemName;
    public string itemDescription;
    public int quantity;
}

[System.Serializable]
public class EventListData
{
    public string eventCode;
    public string script;
    public string optionSW;
}

public class EventScriptDicData
{
    public Dictionary<string, string> EVEventScript;
    public Dictionary<string, string> ETEventScript;
}



public class PlayerSaveDataManager : MonoBehaviour
{

    public EventScriptDicData GetEventScript()
    {
        Dictionary<string, string> loadedEVEventScript = new Dictionary<string, string>();
        Dictionary<string, string> loadedETEventScript = new Dictionary<string, string>();

        loadedEVEventScript = GetEventText("EV", 0);
        loadedETEventScript = GetEventText("ET", 0);

        EventScriptDicData eventScriptDicData = new EventScriptDicData();

        eventScriptDicData.EVEventScript = loadedEVEventScript;
        eventScriptDicData.ETEventScript = loadedETEventScript;

        return eventScriptDicData;
    }

    public Dictionary<string, string> GetEventText(string fileNameHeader, int fileNameIndex)
    {
        bool loadSW = true;
        string fileNameFull = "";
        Dictionary<string, string> loadedEventScriptDic = new Dictionary<string, string>();
        TextAsset eventScript = null;

        // テキストファイルをすべて読み出す
        while (loadSW)
        {
            try
            {
                fileNameFull = fileNameHeader + fileNameIndex.ToString().PadLeft(3, '0');
                Debug.Log("fileNameFull: " + fileNameFull);
                eventScript = Resources.Load("eventScript/" + fileNameFull, typeof(TextAsset)) as TextAsset;
                Debug.Log("eventScript: " + eventScript);
                loadedEventScriptDic.Add(fileNameFull, eventScript.text);
                ++fileNameIndex;
            }
            catch (Exception e)
            {
                Debug.Log("Fail loaded event script: " + e);
                loadSW = false;
            }
        }
        return loadedEventScriptDic;
    }

    public void SaveEventListData(EventListData[] eventListData)
    {
        //Dictionary<string, string> EventListDic = new Dictionary<string, string>();

        // セーブ前にデータをロードする
        //EventListData[] loadedEventListData = LoadedEventListData();
        //for(int i = 0; i < loadedEventListData.Length; i++)
        //{
        //   Debug.Log("loadedEventListData: " + loadedEventListData[i].eventCode);
        //}

        // もしロードしたデータがなかったら新しいイベントリストをセーブする
        //if (loadedEventListData == null)
        //{
        //    Debug.Log("NEW SAVE EVENTLIST: " + eventListData.ToString());
        //    string eventAsStr = JsonHelper.ToJson(eventListData, true);
        //    File.WriteAllText(Application.dataPath + "/Resources/event/eventList.json", eventAsStr);
        //}
        // ロードデータがあるなら既存イベントリストに新しいイベントリストを追加してセーブする
        //else
        //{
            //Debug.Log("loadedEventListData EVENTLIST: " + loadedEventListData.ToString());

        //    foreach (EventListData loadedEvent in loadedEventListData)
        //    {
        //        EventListDic.Add(loadedEvent.eventCode, loadedEvent.script);
        //    }
        //    foreach (EventListData eventList in eventListData)
        //    {
                // dictionary same key different value update
        //        EventListDic[eventList.eventCode] = eventList.script; 
        //    }
        //    Debug.Log("MERGED eventList: " + EventListDic.ToString());
        //    string eventAsStr = JsonConvert.SerializeObject(EventListDic);
            string eventAsStr = JsonHelper.ToJson(eventListData, true);
            Debug.Log("dictionary to string: " + eventAsStr);
            File.WriteAllText(Application.dataPath + "/Resources/event/eventList.json", eventAsStr);
        //}
    }

    
    public EventListData[] LoadedEventListData()
    {
        EventListData[] returnEventListData = null;
        try
        {
            string eventAsStr = File.ReadAllText(Application.dataPath + "/Resources/event/eventList.json");
            Debug.Log("eventAsStr: " + eventAsStr);
            returnEventListData = JsonHelper.FromJson<EventListData>(eventAsStr);
        }
        catch(Exception e)
        {
            Debug.Log("EXCEPTION: " +e);
            returnEventListData = null;
        }
        return returnEventListData;
    }
    

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
                    if (itemListData[j] != null && loadedItemListData[i].itemName.Equals(itemListData[j].itemName))
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
                if(loadedData != null && loadedData.itemName != null)
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

    public ItemListData[] LoadItemListData(int page)
    {
        ItemListData[] itemListData;
        ItemListData[] returnItemListData = null;
        Debug.Log("PAGEPARAM: " + page);

        try
        {
            string itemAsStr = File.ReadAllText(Application.dataPath + "/Resources/saveData/testPlayerItem.json");
            itemListData = JsonHelper.FromJson<ItemListData>(itemAsStr);

            int pageItemStartIndex = (page-1) * 6;
            int pageItemEndIndex = pageItemStartIndex+6;
            // 全体アイテムの数を超えるとendIndexを全体アイテムの数で設定する
            if (pageItemStartIndex + 6 > itemListData.Length)
            {
                pageItemEndIndex = itemListData.Length;
            }
            returnItemListData = new ItemListData[((pageItemEndIndex-1) - pageItemStartIndex) + 1];
            Debug.Log("pageItemStartIndex: " + pageItemStartIndex);
            Debug.Log("pageItemEndIndex: " + pageItemEndIndex);
            int returnCount = 0;
            // アイテム全体でrequestページのアイテムだけ引けだす
            for(int i= pageItemStartIndex; i<pageItemEndIndex; i++)
            {
                returnItemListData[returnCount] = itemListData[i];
                returnCount++;
            }

            Debug.Log("SUCCESS LOAD");
        }
        catch
        {
            Debug.Log("LOADITEMLISTDATA ERROR");
        }
        return returnItemListData;
    }

    // 全体アイテムロード
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
        Debug.Log("LOADDATA: " + playerData.money +","+ playerData.progress);

        return playerData;
    }
}
