﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JobDiarySetManager : MonoBehaviour
{
    public JobDiaryModel[] GetJobDiaryJsonFile()
    {
        string jsonStr = File.ReadAllText(Application.dataPath + "/Resources/saveData/jobDiary.json");
        Debug.Log("jsonStr jobDiary: " + jsonStr);
        JobDiaryModel[] jobDiaryModelArray = JsonHelper.FromJson<JobDiaryModel>(jsonStr);
        return jobDiaryModelArray;
    }

    public void CreateJobDiaryJsonFile(List<JobDiaryModel> jobDiaryModelList)
    {
        string jsonStr = JsonHelper.ToJson(jobDiaryModelList.ToArray(), true);
        Debug.Log("jsonStr: " + jsonStr);
        File.WriteAllText(Application.dataPath + "/Resources/saveData/jobDiary.json", jsonStr);
    }
}
