using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class MainEventSetManager : MonoBehaviour
{
    public void CreateMainEventJson(Dictionary<string, Dictionary<string, object>> mainEventListDic, string buildMode)
    {
        MainEventModel mainEventModel;
        List<MainEventModel> mainEventModelList = new List<MainEventModel>();

        try
        {
            // Dictionary数くらい繰り返す
            foreach(KeyValuePair<string, Dictionary<string, object>> mainEventDic in mainEventListDic)
            {
                mainEventModel = new MainEventModel();

                Dictionary<string, object> mainEventItemDic = mainEventDic.Value;
                mainEventModel.eventCode = (string)mainEventItemDic["eventCode"];
                mainEventModel.requiredProgress = (int)mainEventItemDic["requiredProgress"];
                mainEventModel.requiredSatisfaction = (int)mainEventItemDic["requiredSatisfaction"];
                mainEventModel.requiredScene = (string)mainEventItemDic["requiredScene"];
                mainEventModel.requiredCompletedMainEvent = (string)mainEventItemDic["requiredCompletedMainEvent"];
                mainEventModel.requiredCompletedJobEvent = (string)mainEventItemDic["requiredCompletedJobEvent"];
                mainEventModel.addingProgress = (int)mainEventItemDic["addingProgress"];

                mainEventModelList.Add(mainEventModel);
            }

            // jsonファイルを作る
            Debug.Log("mainEventModelList.Count: " + mainEventModelList.Count);
            CreateMainEventJsonFile(mainEventModelList, buildMode);
        }
        catch (Exception e)
        {
            Debug.Log("ERROR: " + e.ToString());
        }
    }

    public MainEventModel[] GetMainEventJsonFile(string buildMode)
    {
        string jsonStr = null;

        string folderPath = (Application.platform == RuntimePlatform.Android ? Application.persistentDataPath : Application.dataPath) + "/Resources/saveData/";
        string filePath = folderPath + "mainEvent.json";

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        jsonStr = File.ReadAllText(filePath);

        /*
        if ("window".Equals(buildMode))
        {
            jsonStr = File.ReadAllText(Application.dataPath + "/Resources/saveData/mainEvent.json");
        }
        else if ("android".Equals(buildMode))
        {
            jsonStr = File.ReadAllText(Directory.CreateDirectory(Application.persistentDataPath + "/Resources/saveData/").FullName + "mainEvent.json");
        }
        */
        Debug.Log("jsonStr mainEvent: " + jsonStr);
        MainEventModel[] mainEventModelArray = JsonHelper.FromJson<MainEventModel>(jsonStr);

        return mainEventModelArray;
    }

    public void CreateMainEventJsonFile(List<MainEventModel> mainEventModelList, string buildMode)
    {
        string jsonStr = JsonHelper.ToJson(mainEventModelList.ToArray(), true);
        Debug.Log("jsonStr: " + jsonStr);

        string folderPath = (Application.platform == RuntimePlatform.Android ? Application.persistentDataPath : Application.dataPath) + "/Resources/saveData/";
        string filePath = folderPath + "mainEvent.json";

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        File.Create(filePath).Close();
        File.WriteAllText(filePath, jsonStr);

        /*
        if ("window".Equals(buildMode))
        {
            File.WriteAllText(Application.dataPath + "/Resources/saveData/mainEvent.json", jsonStr);
        }
        else if ("android".Equals(buildMode))
        {
            File.WriteAllText(Directory.CreateDirectory(Application.persistentDataPath + "/Resources/saveData/").FullName + "mainEvent.json", jsonStr);
        }
        */
    }
}
