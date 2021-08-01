using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class MainEventSetManager : MonoBehaviour
{
    public void CreateMainEventJson(Dictionary<string, Dictionary<string, object>> mainEventListDic)
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
            CreateMainEventJsonFile(mainEventModelList);
        }
        catch (Exception e)
        {
            Debug.Log("ERROR: " + e.ToString());
        }
    }

    public MainEventModel[] GetMainEventJsonFile()
    {
        string jsonStr = File.ReadAllText(Application.dataPath + "/Resources/saveData/mainEvent.json");
        Debug.Log("jsonStr mainEvent: " + jsonStr);
        MainEventModel[] mainEventModelArray = JsonHelper.FromJson<MainEventModel>(jsonStr);

        return mainEventModelArray;
    }

    public void CreateMainEventJsonFile(List<MainEventModel> mainEventModelList)
    {
        string jsonStr = JsonHelper.ToJson(mainEventModelList.ToArray(), true);
        Debug.Log("jsonStr: " + jsonStr);
        File.WriteAllText(Application.dataPath + "/Resources/saveData/mainEvent.json", jsonStr);
    }
}
