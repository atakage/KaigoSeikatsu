using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class ClearData
{
    public bool clear;
    public List<PlayerData> clearPlayerDataList;
}

public class GameClearFileManager : MonoBehaviour
{
    public void SaveGameClearFile(PlayerData playerData)
    {
        ClearData clearData = new ClearData();
        clearData.clear = true;

        // クリアファイルが存在すると
        if (CheckExistClearFile())
        {
            Debug.Log("CHECKED directory /Resources/saveData/ IN SaveGameClearFile");
            // プレイヤーデータを追加
            clearData = LoadClearData();
            clearData.clearPlayerDataList.Add(playerData);
        }
        // クリアファイルが存在しないと
        else
        {
            Debug.Log("NOT FOUND directory /Resources/saveData/ IN SaveGameClearFile");
            // 新しいクリアリストを作る
            List<PlayerData> clearPlayerDataList = new List<PlayerData>();
            clearPlayerDataList.Add(playerData);
            clearData.clearPlayerDataList = clearPlayerDataList;
        }

        // 2021.11.17 修正
        string folderPath = (Application.platform == RuntimePlatform.Android ? Application.persistentDataPath : Application.dataPath) + "/Resources/saveData/";
        string filePath = folderPath + "clearFile.json";

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log("CREATING directory /Resources/saveData/ IN SaveGameClearFile");
        }

        string jsonStr = JsonConvert.SerializeObject(clearData);
        File.WriteAllText(filePath, jsonStr);
    }

    public ClearData LoadClearData()
    {
        ClearData clearData = null;
        string jsonStr = null;

        try
        {
            string folderPath = (Application.platform == RuntimePlatform.Android ? Application.persistentDataPath : Application.dataPath) + "/Resources/saveData/";
            string filePath = folderPath + "clearFile.json";

            if (!Directory.Exists(folderPath))
            {
                Debug.Log("NOT FOUND directory /Resources/saveData/ IN LoadClearData");
                Directory.CreateDirectory(folderPath);
            }

            jsonStr = File.ReadAllText(filePath);
        }
        catch(FileNotFoundException e)
        {
            return clearData;
        }
        

        /*
        if ("window".Equals(buildMode))
        {
            try
            {
                jsonStr = File.ReadAllText(Application.dataPath + "/Resources/saveData/clearFile.json");
            }
            catch(FileNotFoundException e)
            {
                return clearData;
            }
        }
        else if ("android".Equals(buildMode))
        {
            try
            {
                jsonStr = File.ReadAllText(Directory.CreateDirectory(Application.persistentDataPath + "/Resources/saveData/").FullName + "clearFile.json");
            }
            catch (FileNotFoundException e)
            {
                return clearData;
            }
        }
        */

        if (jsonStr != null) clearData = JsonConvert.DeserializeObject<ClearData>(jsonStr);

        return clearData;
    }

    public bool CheckExistClearFile()
    {
        bool checkFile = false;

        string folderPath = (Application.platform == RuntimePlatform.Android ? Application.persistentDataPath : Application.dataPath) + "/Resources/saveData/";
        string filePath = folderPath + "clearFile.json";

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        checkFile = File.Exists(filePath);

        /*
        if ("window".Equals(buildMode))
        {
            checkFile = File.Exists(Application.dataPath + "/Resources/saveData/clearFile.json");
        }else if ("android".Equals(buildMode))
        {
            checkFile = File.Exists(Directory.CreateDirectory(Application.persistentDataPath + "/Resources/saveData/").FullName + "clearFile.json");
        }
        */
        Debug.Log("checkFile: " + checkFile);

        return checkFile;
    }
}
