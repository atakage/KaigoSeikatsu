using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class ClearData
{
    public bool clear;
    public List<PlayerData> cllearPlayerDataList;
}

public class GameClearFileManager : MonoBehaviour
{
    public void SaveGameClearFile()
    {
        ClearData clearData = new ClearData();
        clearData.clear = true;

        string jsonStr = JsonConvert.SerializeObject(clearData);
        File.WriteAllText(Application.dataPath + "/Resources/saveData/clearFile.json", jsonStr);
    }

    public ClearData LoadClearData()
    {
        string jsonStr = File.ReadAllText(Application.dataPath + "/Resources/saveData/clearFile.json");
        ClearData clearData = JsonConvert.DeserializeObject<ClearData>(jsonStr);
        return clearData;
    }

    public bool CheckExistClearFile()
    {
        return File.Exists(Application.dataPath + "/Resources/saveData/clearFile.json");
    }
}
