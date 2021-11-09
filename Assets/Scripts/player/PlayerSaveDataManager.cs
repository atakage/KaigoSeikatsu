﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;


[System.Serializable]
public class PlayerData
{
    public string name;
    public string currentScene;
    public string money;
    public string time;
    public EventCodeObject eventCodeObject = new EventCodeObject(); // クリアしたイベント
    public int progress; // ゲームの進行度
    public float fatigue; // 疲労、100になったらゲームオーバー
    public int satisfaction; // 仕事の満足度, endingに影響
    public int feeling; // 気分
    public Tip tip = new Tip();
    public Flag flag = new Flag();
    public float playTime;
    public string ending;
    public bool localMode;
    public string startDate;
    public string endDate;
}

[System.Serializable]
public class Flag
{
    public bool completeMainEvent; // MainEventを探して発動する 
    public bool jobEventSearchSkip; // メインイベントを実行したらこの時間にはjobEventを探さない
    public bool jobEventDayCompletedBool; // 他の時間にjobEventを発動させる
}

[System.Serializable]
public class Tip
{
    public bool checkedJobDiaryTip;
}

[System.Serializable]
public class EventCodeObject
{
    public string[] completedMainEventArray;
    public string[] completedJobEventArray;
}


[System.Serializable]
public class ItemListData
{
    public string itemName;
    public string itemDescription;
    public int quantity;
    public string keyItem;
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

    public void SaveEventListData(EventListData[] eventListData, string buildMode)
    {
        string eventAsStr = JsonHelper.ToJson(eventListData, true);
        Debug.Log("dictionary to string: " + eventAsStr);

        string folderPath = (Application.platform == RuntimePlatform.Android ? Application.persistentDataPath : Application.dataPath) + "/Resources/saveData/";
        string filePath = folderPath + "eventList.json";

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        File.Create(filePath).Close();
        File.WriteAllText(filePath, eventAsStr);

        /*
        if ("window".Equals(buildMode))
        {
            File.WriteAllText(Application.dataPath + "/Resources/event/eventList.json", eventAsStr);
        }
        else if ("android".Equals(buildMode))
        {
            File.WriteAllText(Directory.CreateDirectory(Application.persistentDataPath + "/Resources/event/").FullName + "eventList.json", eventAsStr);
        }
        */
    }

    
    public EventListData[] LoadedEventListData(string buildMode)
    {
        EventListData[] returnEventListData = null;
        try
        {
            string eventAsStr = null;

            string folderPath = (Application.platform == RuntimePlatform.Android ? Application.persistentDataPath : Application.dataPath) + "/Resources/saveData/";
            string filePath = folderPath + "eventList.json";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            eventAsStr = File.ReadAllText(filePath);

            /*
            // 2021.11.04 追加
            if ("window".Equals(buildMode))
            {
                eventAsStr = File.ReadAllText(Application.dataPath + "/Resources/event/eventList.json");
                Debug.Log("eventAsStr: " + eventAsStr);
            }
            else if ("android".Equals(buildMode))
            {
                eventAsStr = File.ReadAllText(Directory.CreateDirectory(Application.persistentDataPath + "/Resources/event/").FullName + "eventList.json");
                Debug.Log("eventAsStr: " + eventAsStr);
            }
            */
            returnEventListData = JsonHelper.FromJson<EventListData>(eventAsStr);
        }
        catch(Exception e)
        {
            Debug.Log("EXCEPTION: " +e);
            returnEventListData = null;
        }
        return returnEventListData;
    }

    public void SavePlayerItemList(ItemListData[] itemListDataArray, string buildMode)
    {
        string itemListDataJson = JsonHelper.ToJson(itemListDataArray, true);
        Debug.Log("SAVEDATA: " + itemListDataJson);

        string folderPath = (Application.platform == RuntimePlatform.Android ? Application.persistentDataPath : Application.dataPath) + "/Resources/saveData/";
        string filePath = folderPath + "testPlayerItem.json";

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        File.Create(filePath).Close();
        File.WriteAllText(filePath, itemListDataJson);

        /*
        if ("window".Equals(buildMode))
        {
            File.WriteAllText(Application.dataPath + "/Resources/saveData/testPlayerItem.json", itemListDataJson);
        }
        else if ("android".Equals(buildMode))
        {
            File.WriteAllText(Directory.CreateDirectory(Application.persistentDataPath + "/Resources/saveData/").FullName + "testPlayerItem.json", itemListDataJson);
        }
        */
    }


    // 2021.05.06使用
    public void SaveItemListData(ItemListData[] itemListData, string buildMode)
    {
        // セーブ前にデータをロードする
        ItemListData[] loadedItemListData = LoadItemListData(buildMode);
        
        // もしロードデータがなかったら新しいアイテムリストをセーブする
        if(loadedItemListData == null)
        {
            Debug.Log("LOADED ITEM LIST NULL");
            Debug.Log("ITEMLISTDATAPARAM LENGTH: " + itemListData.Length);

            string itemAsStr = JsonHelper.ToJson(itemListData, true);
            Debug.Log("SAVEDATA: " + itemAsStr);

            string folderPath = (Application.platform == RuntimePlatform.Android ? Application.persistentDataPath : Application.dataPath) + "/Resources/saveData/";
            string filePath = folderPath + "testPlayerItem.json";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            File.Create(filePath).Close();
            File.WriteAllText(filePath, itemAsStr);

            /*
            // 2021.11.04 追加
            if ("window".Equals(buildMode))
            {
                File.WriteAllText(Application.dataPath + "/Resources/saveData/testPlayerItem.json", itemAsStr);
            }
            else if ("android".Equals(buildMode))
            {
                File.WriteAllText(Directory.CreateDirectory(Application.persistentDataPath + "/Resources/saveData/").FullName + "testPlayerItem.json", itemAsStr);
            }
            */
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
                        loadedItemListData[i].quantity += itemListData[j].quantity;
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

            string folderPath = (Application.platform == RuntimePlatform.Android ? Application.persistentDataPath : Application.dataPath) + "/Resources/saveData/";
            string filePath = folderPath + "testPlayerItem.json";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            File.Create(filePath).Close();
            File.WriteAllText(filePath, itemAsStr);

            /*
            // 2021.11.04 追加
            if ("window".Equals(buildMode))
            {
                File.WriteAllText(Application.dataPath + "/Resources/saveData/testPlayerItem.json", itemAsStr);
            }
            else if ("android".Equals(buildMode))
            {
                File.WriteAllText(Directory.CreateDirectory(Application.persistentDataPath + "/Resources/saveData/").FullName + "testPlayerItem.json", itemAsStr);
            }
            */
        }
    }

    public ItemListData[] LoadItemListData(int page, string buildMode)
    {
        ItemListData[] itemListData;
        ItemListData[] returnItemListData = null;
        Debug.Log("PAGEPARAM: " + page);

        try
        {
            string itemAsStr = null;

            string folderPath = (Application.platform == RuntimePlatform.Android ? Application.persistentDataPath : Application.dataPath) + "/Resources/saveData/";
            string filePath = folderPath + "testPlayerItem.json";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            itemAsStr = File.ReadAllText(filePath);

            /*
            if ("window".Equals(buildMode))
            {
                itemAsStr = File.ReadAllText(Application.dataPath + "/Resources/saveData/testPlayerItem.json");
            }
            else if ("android".Equals(buildMode))
            {
                itemAsStr = File.ReadAllText(Directory.CreateDirectory(Application.persistentDataPath + "/Resources/saveData/").FullName + "testPlayerItem.json");
            }
            */
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
    public ItemListData[] LoadItemListData(string buildMode)
    {
        ItemListData[] itemListData;
        try
        {
            string itemAsStr = null;

            string folderPath = (Application.platform == RuntimePlatform.Android ? Application.persistentDataPath : Application.dataPath) + "/Resources/saveData/";
            string filePath = folderPath + "testPlayerItem.json";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            itemAsStr = File.ReadAllText(filePath);

            /*
            // 2021.11.04 追加
            if ("window".Equals(buildMode))
            {
                itemAsStr = File.ReadAllText(Application.dataPath + "/Resources/saveData/testPlayerItem.json");
            }else if ("android".Equals(buildMode))
            {
                itemAsStr = File.ReadAllText(Directory.CreateDirectory(Application.persistentDataPath + "/Resources/saveData/").FullName + "testPlayerItem.json");
            }
            */

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

    public void RemoveItemListDataJsonFile(string buildMode)
    {

        string folderPath = (Application.platform == RuntimePlatform.Android ? Application.persistentDataPath : Application.dataPath) + "/Resources/saveData/";
        string filePath = folderPath + "testPlayerItem.json";

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        File.Delete(filePath);

        /*
        // window
        if ("window".Equals(buildMode))
        {
            File.Delete(Application.dataPath + "/Resources/saveData/testPlayerItem.json");
        }
        // android
        else if ("android".Equals(buildMode))
        {
            // android
            File.Delete(Directory.CreateDirectory(Application.persistentDataPath + "/Resources/saveData/").FullName + "testPlayerItem.json");
        }
        */
    }

    public void SavePlayerData(PlayerData playerData, string buildMode)
    {
        // 2021.08.02 修正
        // statusが0未満なら0でセーブする
        playerData = CheckStatusValueZero(playerData);
       
        string strPlayerData = JsonConvert.SerializeObject(playerData);
        Debug.Log("SAVEDATA: " + strPlayerData.ToString());

        string folderPath = (Application.platform == RuntimePlatform.Android ? Application.persistentDataPath : Application.dataPath) + "/Resources/saveData/";
        string filePath = folderPath + "testPlayerData.json";

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        File.Create(filePath).Close();
        File.WriteAllText(filePath, strPlayerData);

        /*
        // 2021.11.04 追加
        if ("window".Equals(buildMode))
        {
            File.WriteAllText(Application.dataPath + "/Resources/saveData/testPlayerData.json", strPlayerData);
        }
        else if ("android".Equals(buildMode))
        {
            File.WriteAllText(Directory.CreateDirectory(Application.persistentDataPath + "/Resources/saveData/").FullName + "testPlayerData.json", strPlayerData);
        }
        */
    }

    public string[] SaveCompletedEvent(string[] eventCodeArray, string completedEventCode)
    {
        List<string> eventCodelist = null;

        // 完了した全体イベントリストに追加する
        if (eventCodeArray != null && eventCodeArray.Length > 0)
        {
            eventCodelist = new List<string>(eventCodeArray);
        }
        // 最初に追加する完了イベントなら新しくリストを作る
        else
        {
            eventCodelist = new List<string>();
        }
        eventCodelist.Add(completedEventCode);

        return eventCodelist.ToArray();
    }

    public PlayerData LoadPlayerData()
    {
        PlayerData playerData = null;
        try
        {
            string folderPath = (Application.platform == RuntimePlatform.Android ? Application.persistentDataPath : Application.dataPath) + "/Resources/saveData/";
            string filePath = folderPath + "testPlayerData.json";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            //File.Create(filePath).Close();

            string dataStr = File.ReadAllText(filePath);
            playerData = JsonConvert.DeserializeObject<PlayerData>(dataStr);

            /*
            string dataStr = File.ReadAllText(Application.dataPath + "/Resources/saveData/testPlayerData.json");
            playerData = JsonConvert.DeserializeObject<PlayerData>(dataStr);
            */
            Debug.Log("LOADDATA playerData.time: " + playerData.time);
        }
        catch(Exception e)
        {

        }
        return playerData;
    }

    public void DeletePlayerDataJsonFile()
    {
        File.Delete(Application.dataPath + "/Resources/saveData/testPlayerData.json");
    }

    public PlayerData CheckStatusValueZero(PlayerData playerData)
    {
        if (playerData.fatigue < 0) playerData.fatigue = 0;
        if (playerData.feeling < 0) playerData.feeling = 0;
        if (playerData.satisfaction < 0) playerData.satisfaction = 0;

        return playerData;
    }
}
