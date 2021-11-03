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
    public void SaveGameClearFile(PlayerData playerData, string buildMode)
    {
        ClearData clearData = new ClearData();
        clearData.clear = true;

        // クリアファイルが存在すると
        if (CheckExistClearFile(buildMode))
        {
            // プレイヤーデータを追加
            clearData = LoadClearData(buildMode);
            clearData.clearPlayerDataList.Add(playerData);
        }
        // クリアファイルが存在しないと
        else
        {
            // 新しいクリアリストを作る
            List<PlayerData> clearPlayerDataList = new List<PlayerData>();
            clearPlayerDataList.Add(playerData);
            clearData.clearPlayerDataList = clearPlayerDataList;
        }

        string jsonStr = JsonConvert.SerializeObject(clearData);
        File.WriteAllText(Application.dataPath + "/Resources/saveData/clearFile.json", jsonStr);
    }

    public ClearData LoadClearData(string buildMode)
    {
        ClearData clearData = null;
        string jsonStr = null;
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

        if(jsonStr != null) clearData = JsonConvert.DeserializeObject<ClearData>(jsonStr);

        return clearData;
    }

    public bool CheckExistClearFile(string buildMode)
    {
        bool checkFile = false;

        if ("window".Equals(buildMode))
        {
            checkFile = File.Exists(Application.dataPath + "/Resources/saveData/clearFile.json");
        }else if ("android".Equals(buildMode))
        {
            checkFile = File.Exists(Directory.CreateDirectory(Application.persistentDataPath + "/Resources/saveData/").FullName + "clearFile.json");
        }

        Debug.Log("checkFile: " + checkFile);

        return checkFile;
    }
}
