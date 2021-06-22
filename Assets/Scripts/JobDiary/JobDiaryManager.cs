using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JobDiaryManager : MonoBehaviour
{
    public List<JobDiaryModel> AddEventToJobDiary(JobDiaryModel[] jobDiaryModelArray, string eventCode, string eventScript, string choosingText)
    {
        List<JobDiaryModel> jobDiaryModelList = new List<JobDiaryModel>();

        JobDiaryModel jobDiaryModel = new JobDiaryModel();
        jobDiaryModel.eventCode = eventCode;
        jobDiaryModel.eventScript = eventScript;
        jobDiaryModel.choosingString = choosingText;

        // jobDiaryがnullなら既存イベントをリストに追加せずに続ける
        if (jobDiaryModelArray == null || jobDiaryModelArray.Length < 1)
        {
            jobDiaryModelList.Add(jobDiaryModel);
        }
        // jobDiaryにイベントがあるなら既存リストに追加
        else
        {
            jobDiaryModelList = jobDiaryModelArray.OfType<JobDiaryModel>().ToList();
            jobDiaryModelList.Add(jobDiaryModel);
        }
        return jobDiaryModelList;
    }
}
