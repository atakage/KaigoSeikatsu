using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class JobEventSetManager : MonoBehaviour
{
    public void CreateJobEventJson(Dictionary<string, Dictionary<string, object>> jobEventListDic, string buildMode)
    {
        JobEventModel jobEventModel;
        List<JobEventModel> jobEventModelList = new List<JobEventModel>();

        try
        {
            // Dictionary数くらい繰り返す
            foreach(KeyValuePair<string, Dictionary<string, object>> jobEventDic in jobEventListDic)
            {
                jobEventModel = new JobEventModel();

                Dictionary<string, object> jobEventItemDic = jobEventDic.Value;
                jobEventModel.eventCode = (string)jobEventItemDic["eventCode"];
                jobEventModel.eventScript = (string)jobEventItemDic["eventScript"];
                jobEventModel.choiceA = (string)jobEventItemDic["choiceA"];
                jobEventModel.choiceB = (string)jobEventItemDic["choiceB"];
                jobEventModel.choiceC = (string)jobEventItemDic["choiceC"];
                jobEventModel.choiceAEffect = (string)jobEventItemDic["choiceAEffect"];
                jobEventModel.choiceBEffect = (string)jobEventItemDic["choiceBEffect"];
                jobEventModel.choiceCEffect = (string)jobEventItemDic["choiceCEffect"];
                jobEventModel.eventActive = true;

                jobEventModelList.Add(jobEventModel);
            }

            // jsonファイルを作る
            Debug.Log("jobEventModelList.Count: " + jobEventModelList.Count);
            CreateJobEventJsonFile(jobEventModelList, buildMode);
        }
        catch (Exception e)
        {
            Debug.Log("ERROR: " + e.ToString());
        }
    }

    public JobEventModel[] GetJobEventJsonFile()
    {
        string jsonStr = File.ReadAllText(Application.dataPath + "/Resources/saveData/jobEvent.json");
        Debug.Log("jsonStr jobEvent: " + jsonStr);
        JobEventModel[] jobEventModelArray = JsonHelper.FromJson<JobEventModel>(jsonStr);

        return jobEventModelArray;
    }

    public void CreateJobEventJsonFile(List<JobEventModel> jobEventModelList, string buildMode)
    {
        string jsonStr = JsonHelper.ToJson(jobEventModelList.ToArray(), true);
        Debug.Log("jsonStr: " + jsonStr);

        if ("window".Equals(buildMode))
        {
            File.WriteAllText(Application.dataPath + "/Resources/saveData/jobEvent.json", jsonStr);
        }
        else if ("android".Equals(buildMode))
        {
            File.WriteAllText(Directory.CreateDirectory(Application.persistentDataPath + "/Resources/saveData/").FullName + "jobEvent.json", jsonStr);
        }
    }
}
